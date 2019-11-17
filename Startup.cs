using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using SM.SMS.Web.Api.Infrastructure.Connection;
using SM.SMS.Web.Api.Infrastructure.Connection.Contracts;
using SM.SMS.WebApi.Controllers.Swagger;
using SM.SMS.WebApi.Infrastructure.Configurations;
using SM.SMS.WebApi.Infrastructure.Configurations.ConfigModels;
using SM.SMS.WebApi.Infrastructure.Configurations.Contracts;
using SM.SMS.WebApi.Infrastructure.HttpClients;
using SM.SMS.WebApi.Infrastructure.HttpClients.Contracts;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Infrastructure.OriginSQLPassword;
using SM.SMS.WebApi.Infrastructure.OriginSQLUser;
using SM.SMS.WebApi.Infrastructure.Repositories;
using SM.SMS.WebApi.Infrastructure.Repositories.Contracts;
using SM.SMS.WebApi.Infrastructure.Tracking;
using SM.SMS.WebApi.Middleware;
using SM.SMS.WebApi.Service;
using SM.SMS.WebApi.Service.Contracts;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApi
{
  public class Startup
  {
    public Startup(IConfiguration configuration,
      IHostingEnvironment env)
    {
      LogManager.Configuration = new XmlLoggingConfiguration($"./nlog/nlog.config");

      var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"./appsettings/appsettings.configmap.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"version.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()// TODO Note fix to specific origins
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
      });

      services.AddMvc()
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

      #region SERVICE METHOD VERSIONING - Part 1 of 3

      services.AddMvcCore().AddVersionedApiExplorer(options =>
      {
        options.GroupNameFormat = $" {Assembly.GetExecutingAssembly().GetName().Name}-'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
      });

      services.AddApiVersioning(o =>
      {
        o.ReportApiVersions = true;
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.DefaultApiVersion = new ApiVersion(1, 0);
      });

      services.AddSwaggerGen(options =>
      {
        // resolve the IApiVersionDescriptionProvider service
        // note: that we have to build a temporary service provider here because one has not been created yet
        var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
          options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        // add a custom operation filter which sets default values
        options.OperationFilter<SwaggerDefaultValues>();

        // integrate xml comments
        options.IncludeXmlComments(XmlCommentsFilePath);

        var security = new Dictionary<string, IEnumerable<string>>
        {
          {"Bearer", new string[] { }},
        };

        options.AddSecurityDefinition("Bearer", new ApiKeyScheme
        {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name = "Authorization",
          In = "header",
          Type = "apiKey"
        });

        options.AddSecurityRequirement(security);

      });

      #endregion

      // Add functionality to inject IOptions<T>
      services.AddOptions();

      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
      services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
      services.Configure<ServicesEndpoints>(Configuration.GetSection("ServicesEndpoints"));

      services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<AppSettings>>().Value);
      services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ConnectionStrings>>().Value);
      services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ServicesEndpoints>>().Value);

      services.AddSingleton(Configuration);
      services.AddSingleton<IConfiguration>(Configuration);

      // Dependency injection - pass on X-Tracking-Id so subsequent http calls can be grouped together in logging
      services.AddScoped<IHttpClientFactory, HttpClientFactory>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddSingleton(Configuration);
      services.AddScoped<IHttpClientFactory, HttpClientFactory>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddTransient<IHttpClientRepo, HttpClientRepo>();
      services.AddTransient<IConfigService, ConfigService>();
      services.AddTransient<ILoggerService, LoggerService>();
      services.AddTransient<ITrackingService, TrackingService>();
      services.AddTransient<IConnectionFactory, ConnectionFactory>();
      services.AddTransient<IOriginSQLUserService, OriginSQLUserService>();// Origin user
      services.AddTransient<IOriginSQLPasswordService, OriginSQLPasswordService>();// Origin password

      services.AddTransient<ITestService, TestService>();

      services.AddTransient<ISMSService, SMSService>();
      services.AddTransient<ISMSRepo, SmsRepo>();
      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app,
      IHostingEnvironment env,
      ILoggerFactory loggerFactory,
      IConfigService configService,
      IApiVersionDescriptionProvider provider)
    {
      // Add Logging
      loggerFactory.AddNLog();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error"); // specify a specific error page.
      }

      app.UseStaticFiles();
      app.UseExceptionHandleMiddleware();
      app.UseResponseTimeMiddleware();

      app.UseMvc();

      app.UseMvc(routes =>
      {   // Creates a default route where route is not added to controller method.
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseSwagger();

      #region SERVICE METHOD VERSIONING - Part 2 of 3

      app.UseSwaggerUI(options =>
      {
        // build a swagger endpoint for each discovered API version
        foreach (var description in provider.ApiVersionDescriptions)
        {
          options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
      });

      #endregion
    }

    #region SERVICE METHOD VERSIONING - Part 3 of 3

    /// <summary>
    /// Necessary for Service Methods Versioning
    /// </summary>
    static string XmlCommentsFilePath
    {
      get
      {
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
        return Path.Combine(basePath, fileName);
      }
    }

    /// <summary>
    /// Necessary for Service Methods Versioning
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    static Info CreateInfoForApiVersion(ApiVersionDescription description)
    {
      var info = new Info()
      {
        Title = $" {Assembly.GetExecutingAssembly().GetName().Name} - {description.ApiVersion}",
        Version = description.ApiVersion.ToString(),
        Description = " Strike Media SMS Service - Send SMS via ClickaTell and check SMS send History"
      };

      if (description.IsDeprecated)
      {
        info.Description += " This API version has been deprecated.";
      }

      return info;
    }

    #endregion

  }
}
