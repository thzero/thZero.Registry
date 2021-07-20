/* ------------------------------------------------------------------------- *
thZero.Registry
Copyright (C) 2021-2021 thZero.com

<development [at] thzero [dot] com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 * ------------------------------------------------------------------------- */

using System;

using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using thZero.AspNetCore;
using thZero.Configuration;
using thZero.DependencyInjection;
using thZero.Services;
using thZero.Registry.Services;

using thZero.Instrumentation;
using thZero.Registry.Configuration;

namespace thZero.Registry
{
    public sealed class Startup : ConfigurableMvcStartup<Configuration.Application, ApplicationDefaults, ApplicationEmail, Startup>
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger) : base("2007-2019", configuration, logger)
        {
        }

        #region Protected Methods
        protected override void ConfigureInitialize(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            base.ConfigureInitialize(app, env, loggerFactory, svp);

            //Services.ServiceLocalizationInitializer config = new Services.ServiceLocalizationInitializer();
            //config.Factory = svp.GetService<IStringLocalizerFactory>();
            //var instance = Factory.Instance.Retrieve<Services.IServiceLocalization>();
            //instance.Initialize(config, GetType());
        }

        protected override void ConfigureInitializeFinal(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            base.ConfigureInitializeFinal(app, env, svp);

            //app.UseAuthentication();

            IInstrumentationPacket packet = (IInstrumentationPacket)svp.GetRequiredService(typeof(IInstrumentationPacket));
            IStaticResourcesRegistryService staticResourcesService = (IStaticResourcesRegistryService)svp.GetRequiredService(typeof(IStaticResourcesRegistryService));
            thZero.Utilities.Background.Run(async () =>
            {
                await staticResourcesService.LoadAsync(packet);
            });
        }

        protected override void ConfigureInitializeProduction(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            app.UseExceptionHandler("/Home/Error");
        }

        protected override void ConfigureInitializeRoutingEndpointsRouteBuilder(IEndpointRouteBuilder endpointsRouteBuilder)
        {
            endpointsRouteBuilder.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            endpointsRouteBuilder.MapGrpcService<GrpcRegistryService>();

            base.ConfigureInitializeRoutingEndpointsRouteBuilder(endpointsRouteBuilder);
        }

        protected override void ConfigureInitializeStatic(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            app.UseStaticFiles();

            base.ConfigureInitializeStatic(app, env, svp);
        }

        protected override bool ConfigureServicesInitializeCompression(IServiceCollection services)
        {
            return base.ConfigureServicesInitializeCompression(services);

            //services.AddResponseCompression(options =>
            //{
            //	options.Providers.Add(new BrotliCompressionProvider());
            //});
        }

        protected override void ConfigureServicesInitializeMvcPost(IServiceCollection services)
        {
            base.ConfigureServicesInitializeMvcPost(services);

            services.AddSingleton<Services.IRegistryService, Services.RegistryService>();
            services.AddSingleton<Services.IStaticResourcesRegistryService, Services.StaticResourcesRegistryService>();

            services.Configure<MemRepositoryConfiguration>(this.Configuration.GetSection("RegistryRepository"));
            services.AddSingleton<Repository.IRegistryRepository, Repository.MemRegistryRepository>();

            IConfigurationSection config = Configuration.GetSection("Registry");
            if (config == null)
                throw new Exception("Invalid Registry config.");

            services.Configure<RegistryConfiguration>(config);

            services.AddSingleton<Services.HealthCheck.IHealthCheckRegistryService, Services.HealthCheck.HealthCheckRegistryService>();
            services.AddSingleton<Services.HealthCheck.GrpcPerformHealthCheckRegistryService, Services.HealthCheck.GrpcPerformHealthCheckRegistryService>();
            services.AddSingleton<Services.HealthCheck.HttpPerformHealthCheckRegistryService, Services.HealthCheck.HttpPerformHealthCheckRegistryService>();

            services.AddSingleton<IServiceJson, ServiceJsonNewtonsoft>();

            services.AddHostedService<Services.BackgroundDiscoveryServicer>();
            services.AddHostedService<Services.HealthCheck.BackgroundHealthCheckRegistryService>();

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = SharedKeyAuthenticationOptions.AuthenticationScheme;
            //})
            //.AddSharedSecret(services);

            //services.AddAuthorization(
            //    options =>
            //    {
            //        options.AddPolicy(AdminApiKeyAuthorizeAttribute.KeyPolicy,
            //            builder =>
            //            {
            //                builder.AuthenticationSchemes.Add(SharedKeyAuthenticationOptions.AuthenticationScheme);
            //                builder.RequireClaim(AdminApiKeyAuthorizeAttribute.KeyPolicy);
            //            });
            //        options.AddPolicy(ApiKeyAuthorizeAttribute.KeyPolicy,
            //            builder =>
            //            {
            //                builder.AuthenticationSchemes.Add(SharedKeyAuthenticationOptions.AuthenticationScheme);
            //                builder.RequireClaim(ApiKeyAuthorizeAttribute.KeyPolicy);
            //            });
            //    });

            // Configuration Options...
            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        }

        protected override void ConfigureServicesInitializeMvcPre(IServiceCollection services)
        {
            base.ConfigureServicesInitializeMvcPre(services);

            services.ScanForDependencies(typeof(Startup), GetType().Assembly.GetName().Name);
        }

        protected override void ConfigureServicesInitializeServerTypes(IServiceCollection services)
        {
            base.ConfigureServicesInitializeServerTypes(services);

            services.AddGrpc();
        }

        protected override void RegisterStartupExtensions()
        {
            base.RegisterStartupExtensions();

            RegisterStartupExtension(new AppFactoryStartupExtension());
            RegisterStartupExtension(new HttpSecurityNWebSecStartupExtension());
            RegisterStartupExtension(new SteeltoeInstrumentationStartupExtension());
            RegisterStartupExtension(new AppHealthChecksInstrumentationStartupExtension());
            RegisterStartupExtension(new AppMetricsInstrumentationStartupExtension());
            RegisterStartupExtension(new AppSwaggerStartupExtension()); 
        }
        #endregion

        #region Protected Properties
        protected override bool RequiresSsl => false; //true
        #endregion
    }

    [Factory(FactoryType = typeof(FactoryDryIoc))]
    public sealed class AppFactoryStartupExtension : FactoryStartupExtension
    {
        #region Protected Methods
        protected override void ConfigureInitializeFactory(IServiceProvider svp)
        {
            base.ConfigureInitializeFactory(svp);

            ConfigureInitializeFactoryDatastore();
        }

        protected override void ConfigureServicesInitializeFactory(IServiceCollection services)
        {
            base.ConfigureServicesInitializeFactory(services);

            ConfigureServicesInitializeFactoryUtility();
        }

        public override void ConfigureServicesInitializeMvcPost(IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            base.ConfigureServicesInitializeMvcPost(services, env, configuration);
        }

        #endregion

        #region Private Methods
        private void ConfigureInitializeFactoryDatastore()
        {
            //DataObjects.Context.ITransferContext context = Utilities.Services.Context.Transfer.Instance;
            //Configuration.Application config = Utilities.Configuration.Application;
            //Enforce.AgainstNull(() => config);
            //context.Initialize(new DataObjects.Context.MongoDbContextConnectionConfigration()
            //{
            //    ConnectionString = config.Datastore.ConnectionString,
            //    Database = config.Datastore.Instance
            //});
        }

        private void ConfigureServicesInitializeFactoryUtility()
        {
            Factory.Instance.AddSingleton<IServiceJson, ServiceJsonNewtonsoftFactory>();
        }
        #endregion
    }

    public sealed class AppHealthChecksInstrumentationStartupExtension : HealthChecksInstrumentationStartupExtension
    {
        public AppHealthChecksInstrumentationStartupExtension()
        {
            Route = "/diagnostics/healthz";
        }
    }

    public sealed class AppMetricsInstrumentationStartupExtension : MetricsInstrumentationStartupExtension
    {
    }

    public sealed class AppSwaggerStartupExtension : SwaggerStartupExtension
    {
        #region Protected Methods
        protected override void ConfigureInitializeSwaggerUI(SwaggerUIOptions options)
        {
            SwaggerEndpoint(options, "Registry", thZero.Registry.Constants.ApiGroups.RegistryV1);
        }

        public override void ConfigureServicesInitializeMvcBuilderOptionsPre(MvcOptions options)
        {
            //options.Conventions.Add(new ApiExplorerGroupNameConvention());
        }

        protected override void ConfigureServicesInitializeSwaggerGen(SwaggerGenOptions options)
        {
            String name = GetType().Assembly.GetName().Name;

            options.SwaggerDoc(thZero.Registry.Constants.ApiGroups.RegistryV1,
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = string.Format("{0}.Backend Admin Swagger API", name),
                    Description = string.Format("Registry API for {0}.Backend", name),
                    TermsOfService = null
                }
            );

            options.OperationFilter<SwaggerAuthorizationOperationFilter>();

            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert '[token]' into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                }
            );

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  Array.Empty<string>()
                }
              });
        }
        #endregion
    }
}
