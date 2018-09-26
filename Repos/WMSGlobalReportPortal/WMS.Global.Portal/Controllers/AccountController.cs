using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using WMS.Global.Portal.App_Start;
using WMS.Global.Portal.Models;
using WMS.Global.Portal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMSReport.DTO;

namespace WMS.Global.Portal.Controllers
{
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            Asym_RSA ar = new Asym_RSA();
            try
            {
                model.LoginName = ar.JsDecrypt(model.LoginName);
                model.Password = ar.JsDecrypt(model.Password);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            string DeLoginName = ar.Encrypt(model.LoginName);
            string DePassword = ar.Encrypt(model.Password);
            var systemUser = AuthorizeManager.Login(DeLoginName, DePassword);
            if (systemUser != null)
            {
                ApplicationUser loginUser = new ApplicationUser(systemUser);
                AuthenticationManager.SignOut(SystemDataConst.ApplicationID + DefaultAuthenticationTypes.ApplicationCookie);
                var identity = new ClaimsIdentity(SystemDataConst.ApplicationID + DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.LoginName));
                identity.AddClaim(new Claim(SystemDataConst.SessionLoginDisplayUserName, loginUser.DisplayName));
                identity.AddClaim(new Claim(SystemDataConst.SessionLoginUserID, loginUser.UserId.ToString()));
                identity.AddClaim(new Claim(SystemDataConst.LoginUser, JsonConvert.SerializeObject(loginUser)));

                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = "../Home/Index";
                }
                return Json(new { success = true, returnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "登录失败，用户名密码不正确!" }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool remeberMe = false)
        {
            // 2. 利用ASP.NET Identity获取identity 对象
            var identity = await UserManager.CreateIdentityAsync(user, SystemDataConst.ApplicationID + DefaultAuthenticationTypes.ApplicationCookie);
            // 3. 将上面拿到的identity对象登录
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(SystemDataConst.ApplicationID + DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        public ActionResult NotFoundError()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}