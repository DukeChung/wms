using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Mvc;
using FortuneLab.WebClient.Security.AuthClient;
using FortuneLab.WebClient.Service.API;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Security.AuthClient
{
    public class BaseAccountController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            return Redirect(string.Format("{0}/account/login?returnUrl={1}", ConfigurationManager.AppSettings["LoginUrl"], GetEncodeReturlUrl(returnUrl)));
        }

        [Authorize]
        public ActionResult LogOff(string returnUrl = "")
        {
            AuthorizationService.Instance.ClearAuthorizationCache(ApplicationUser.UserId);//移除User Permission Cache
            var authenticationManger = HttpContext.GetOwinContext().Authentication;
            authenticationManger.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            authenticationManger.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect(string.Format("{0}/account/logoff?returnUrl={1}", ConfigurationManager.AppSettings["LoginUrl"], GetEncodeReturlUrl(returnUrl)));
        }

        private string GetEncodeReturlUrl(string returnUrl)
        {
            return HttpUtility.UrlEncode(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, returnUrl));
        }

        public ActionResult ChangePassword()
        {
            return Redirect(string.Format("{0}/account/changePassword", ConfigurationManager.AppSettings["LoginUrl"]));
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            return Redirect(string.Format("{0}?returnUrl={1}", ConfigurationManager.AppSettings["LoginUrl"], GetEncodeReturlUrl(returnUrl)));
        }

        /// <summary>
        /// 通过sessionKey登录
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="isRemeber"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> LoginBySession(string sessionKey, bool isRemeber, string returnUrl)
        {
            var rsp = AccountApiClient.SessionValidate(new LoginQuery() { SessionKey = sessionKey });
            if (rsp.Success)
            {
                await SignInAsync(new ApplicationUser(rsp.ResponseResult), isRemeber);
            }
            return Redirect(returnUrl);

        }

        [NonAction]
        private async Task SignInAsync(ApplicationUser user, bool remeberMe = false)
        {
            var authenticationManger = HttpContext.GetOwinContext().Authentication;

            authenticationManger.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            authenticationManger.SignIn(new AuthenticationProperties()
            {
                IsPersistent = remeberMe,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, await user.GenerateUserIdentityAsync(HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>()));
        }
    }
}
