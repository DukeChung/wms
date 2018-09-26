using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using FortuneLab.WebClient.Security.AuthClient;

namespace FortuneLab.WebClient
{
    // 摘要: 
    //     Extension methods provided by the cookies authentication middleware
    public static class YMCCookieAuthenticationExtensions
    {
        public static string CookieDomain = WebConfigurationManager.AppSettings["CookieDomain"];
        public static string AuthCookieName = WebConfigurationManager.AppSettings["AuthCookieName"];

        public static IAppBuilder UseYMCCookieAuthentication(this IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login"),
                CookieName = AuthCookieName,
                CookieDomain = CookieDomain,
                CookieHttpOnly = false,
                CookieSecure = CookieSecureOption.Never,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        else
                        {
                            ctx.Response.StatusCode = 401;
                            ctx.Response.Headers.Add("loginUrl", new string[] { ctx.RedirectUri });
                        }
                    },
                    OnValidateIdentity = SessionKeyStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    validateInterval: TimeSpan.FromSeconds(120),
                    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            return app;
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
        //
        // 摘要: 
        //     Adds a cookie-based authentication middleware to your web application pipeline.
        //
        // 参数: 
        //   app:
        //     The IAppBuilder passed to your configuration method
        //
        //   options:
        //     An options class that controls the middleware behavior
        //
        //   stage:
        //
        // 返回结果: 
        //     The original app parameter
        //public static IAppBuilder UseCookieAuthentication(this IAppBuilder app, CookieAuthenticationOptions options, PipelineStage stage);
    }
}
