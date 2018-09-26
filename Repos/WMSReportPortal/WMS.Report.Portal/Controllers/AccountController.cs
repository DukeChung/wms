#region Using

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WMS.Report.Portal.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using NBK.AuthServiceUtil;
using Microsoft.Owin.Security;
using NBK.WMS.Portal.App_Start;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using WMS.Report.Portal.Services;

#endregion

namespace WMS.Report.Portal.Controllers
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
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
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.LoginName));
                identity.AddClaim(new Claim(SystemDataConst.SessionLoginDisplayUserName, loginUser.DisplayName));
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
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //AuthenticationManager.SignIn(new AuthenticationProperties()
            //{
            //    IsPersistent = remeberMe,
            //    IssuedUtc = DateTime.UtcNow,
            //    ExpiresUtc = DateTime.UtcNow.AddDays(7)
            //}, await user.GenerateUserIdentityAsync(HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>()));

            // var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            //var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            //identity.AddClaim(new Claim("LogonUser", JsonConvert.SerializeObject(user)));

            //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);


            // 2. 利用ASP.NET Identity获取identity 对象
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
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



    ////[Authorize]
    //public class AccountController : Controller
    //{
    //    // TODO: This should be moved to the constructor of the controller in combination with a DependencyResolver setup
    //    // NOTE: You can use NuGet to find a strategy for the various IoC packages out there (i.e. StructureMap.MVC5)
    //    private readonly UserManager _manager = UserManager.Create();

    //    // GET: /account/forgotpassword
    //    [AllowAnonymous]
    //    public ActionResult ForgotPassword()
    //    {
    //        // We do not want to use any existing identity information
    //        EnsureLoggedOut();

    //        return View();
    //    }

    //    // GET: /account/login
    //    [AllowAnonymous]
    //    public ActionResult Login(string returnUrl)
    //    {
    //        // We do not want to use any existing identity information
    //        EnsureLoggedOut();

    //        // Store the originating URL so we can attach it to a form field
    //        var viewModel = new AccountLoginModel { ReturnUrl = returnUrl };

    //        return View(viewModel);
    //    }

    //    // POST: /account/login
    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Login(AccountLoginModel viewModel)
    //    {
    //        // Ensure we have a valid viewModel to work with
    //        if (!ModelState.IsValid)
    //            return View(viewModel);

    //        // Verify if a user exists with the provided identity information
    //        //var user = await _manager.FindByEmailAsync(viewModel.Email);
    //        var user = new IdentityUser
    //        {
    //            UserName = viewModel.UserName ?? viewModel.Email,
    //            Email = viewModel.Email
    //        };



    //        // If a user was found
    //        if (user != null)
    //        {
    //            ApplicationUser loginUser = new ApplicationUser();
    //            loginUser.UserName = user.UserName;
    //            loginUser.DisplayName = "aaaaaaa";

    //            // Then create an identity for it and sign it in
    //            await SignInAsync(loginUser, viewModel.RememberMe);

    //            // If the user came from a specific page, redirect back to it
    //            return RedirectToLocal(viewModel.ReturnUrl);
    //        }

    //        // No existing user was found that matched the given criteria
    //        ModelState.AddModelError("", "Invalid username or password.");

    //        // If we got this far, something failed, redisplay form
    //        return View(viewModel);
    //    }

    //    // GET: /account/error
    //    [AllowAnonymous]
    //    public ActionResult Error()
    //    {
    //        // We do not want to use any existing identity information
    //        EnsureLoggedOut();

    //        return View();
    //    }

    //    // GET: /account/register
    //    [AllowAnonymous]
    //    public ActionResult Register()
    //    {
    //        // We do not want to use any existing identity information
    //        EnsureLoggedOut();

    //        return View(new AccountRegistrationModel());
    //    }

    //    // POST: /account/register
    //    //[HttpPost]
    //    //[AllowAnonymous]
    //    //[ValidateAntiForgeryToken]
    //    //public async Task<ActionResult> Register(AccountRegistrationModel viewModel)
    //    //{
    //    //    // Ensure we have a valid viewModel to work with
    //    //    if (!ModelState.IsValid)
    //    //        return View(viewModel);

    //    //    // Prepare the identity with the provided information
    //    //    var user = new IdentityUser
    //    //    {
    //    //        UserName = viewModel.Username ?? viewModel.Email,
    //    //        Email = viewModel.Email
    //    //    };

    //    //    // Try to create a user with the given identity
    //    //    try
    //    //    {
    //    //        var result = await _manager.CreateAsync(user, viewModel.Password);

    //    //        // If the user could not be created
    //    //        if (!result.Succeeded) {
    //    //            // Add all errors to the page so they can be used to display what went wrong
    //    //            AddErrors(result);

    //    //            return View(viewModel);
    //    //        }

    //    //        // If the user was able to be created we can sign it in immediately
    //    //        // Note: Consider using the email verification proces
    //    //        await SignInAsync(user, false);

    //    //        return RedirectToLocal();
    //    //    }
    //    //    catch (DbEntityValidationException ex)
    //    //    {
    //    //        // Add all errors to the page so they can be used to display what went wrong
    //    //        AddErrors(ex);

    //    //        return View(viewModel);
    //    //    }
    //    //}

    //    // POST: /account/Logout
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Logout()
    //    {
    //        // First we clean the authentication ticket like always
    //        FormsAuthentication.SignOut();

    //        // Second we clear the principal to ensure the user does not retain any authentication
    //        HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

    //        // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
    //        // this clears the Request.IsAuthenticated flag since this triggers a new request
    //        return RedirectToLocal();
    //    }

    //    private ActionResult RedirectToLocal(string returnUrl = "")
    //    {
    //        // If the return url starts with a slash "/" we assume it belongs to our site
    //        // so we will redirect to this "action"
    //        if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
    //            return Redirect(returnUrl);

    //        // If we cannot verify if the url is local to our host we redirect to a default location
    //        return RedirectToAction("index", "home");
    //    }

    //    private void AddErrors(DbEntityValidationException exc)
    //    {
    //        foreach (var error in exc.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors.Select(validationError => validationError.ErrorMessage)))
    //        {
    //            ModelState.AddModelError("", error);
    //        }
    //    }

    //    private void AddErrors(IdentityResult result)
    //    {
    //        // Add all errors that were returned to the page error collection
    //        foreach (var error in result.Errors)
    //        {
    //            ModelState.AddModelError("", error);
    //        }
    //    }

    //    private void EnsureLoggedOut()
    //    {
    //        // If the request is (still) marked as authenticated we send the user to the logout action
    //        if (Request.IsAuthenticated)
    //            Logout();
    //    }

    //    private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    //    {
    //        // Clear any lingering authencation data
    //        FormsAuthentication.SignOut();

    //        // Create a claims based identity for the current user
    //        //var identity = await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
    //        var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
    //        identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
    //        identity.AddClaim(new Claim(SystemDataConst.SessionLoginDisplayUserName, user.DisplayName));
    //        identity.AddClaim(new Claim(SystemDataConst.LoginUser, JsonConvert.SerializeObject(user)));

    //        // Write the authentication cookie
    //        FormsAuthentication.SetAuthCookie(identity.Name, isPersistent);
    //    }

    //    // GET: /account/lock
    //    [AllowAnonymous]
    //    public ActionResult Lock()
    //    {
    //        return View();
    //    }
    //}
}