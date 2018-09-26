using BachLib.DependencyInjection;
using BachLib.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
//using Microsoft.Extensions.Logging;
using Schubert.Framework;
using Schubert.Framework.Web;

namespace WMSBussinessApi.Website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
            Configuration.AddConfiguration(env.ContentRootPath);
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvcFitlerService();               //这个在Bach中
            //services.AddBachService(option =>             //这个在Bach中
            //{
            //    string expire = Configuration.GetSection("authenticationConfig")["expire"];
            //    if (!string.IsNullOrEmpty(expire))
            //    {
            //        option.Authentication.Expire = Convert.ToInt32(expire);
            //    }
            //    option.Authentication.VerifyUrl = Configuration.GetSection("authenticationConfig")["verifyUrl"];
            //    option.Authentication.AllowAccessTokensSection = Configuration.GetSection("AllowAccessTokens");
            //});
            services.AddSchubertFramework(this.Configuration,
            builder =>
            {
                builder.AddWebFeature(
                        options =>
                        {
                            options.AddFluentValidationForMvc();
                            options.ConfigureFeature(settings =>
                                    settings.MvcFeatures = MvcFeatures.Api);
                        }
                    );
                builder.AddSnowflakeIdGenerationService();
                builder.AddEyes();
                builder.AddDapperDataFeature(options =>
                {
                    options.Dapper = new Schubert.Framework.Data.DapperOptions
                    {
                        CapitalizationRule = Schubert.Framework.Data.CapitalizationRule.Original,
                        IdentifierMappingStrategy = Schubert.Framework.Data.IdentifierMappingStrategy.PascalCase
                    }; 
                });
            },
            scope =>
            {
                scope.LoggerFactory.AddDebug(LogLevel.Debug);
                scope.LoggerFactory.AddConsole(LogLevel.Debug);
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCrossOrigin();
            //app.UseAuthentication();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseStaticFiles();

            app.StartSchubertWebApplication();
            loggerFactory.AddEyes(LogLevel.Information);

            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
        }
    }
}
