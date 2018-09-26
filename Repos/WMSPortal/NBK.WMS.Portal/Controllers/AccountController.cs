using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using NBK.WMS.Portal.App_Start;
using NBK.WMS.Portal.Models;
using NBK.WMS.Portal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;

namespace NBK.WMS.Portal.Controllers
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
            //var auth = "oauth2/authorize";
            //return Content(string.Format("<script type='text/javascript'>window.location.href='{0}'</script>",
            //                             $"{AuthCenterConfig.UserTokenURL}{auth}?client_id={AuthCenterConfig.SSOClientID}&response_type=code&redirect_uri={AuthCenterConfig.RedirectURL}"));

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

            ApplicationUser checkLoginUser = new ApplicationUser() { UserName = model.LoginName };
            var loginCheckResponse = BaseDataApiClient.GetInstance().UserLoginCheck(checkLoginUser);

            if (loginCheckResponse.Success && loginCheckResponse.ResponseResult.IsSuccess)
            {

                string DeLoginName = ar.Encrypt(model.LoginName);
                string DePassword = ar.Encrypt(model.Password);
                var systemUser = AuthorizeManager.Login(DeLoginName, DePassword);

                if (systemUser != null)
                {
                    ApplicationUser loginUser = new ApplicationUser(systemUser);

                    #region 根据UserId获取仓库信息
                    var rsp = BaseDataApiClient.GetInstance().GetWareHouseByUserId(LoginCoreQuery, loginUser.UserId);
                    if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.Count > 0)
                    {
                        loginUser.WarehouseSysId = rsp.ResponseResult[0].SysId;
                        loginUser.WarehouseName = rsp.ResponseResult[0].Name;
                        loginUser.WareHouseList = rsp.ResponseResult;
                    }
                    else
                    {
                        //ModelState.AddModelError("", "登录失败，此用户没有任何仓库权限!");
                        BaseDataApiClient.GetInstance().UserLoginFail(loginUser);
                        return Json(new { success = false, message = "登录失败，此用户没有任何仓库权限!" }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                    identity.AddClaim(new Claim(ClaimTypes.Name, model.LoginName));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginDisplayUserName, loginUser.DisplayName));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginUserID, loginUser.UserId.ToString()));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginWarehouseSysId, loginUser.WarehouseSysId.ToString()));
                    identity.AddClaim(new Claim(SystemDataConst.LoginUser, JsonConvert.SerializeObject(loginUser)));

                    //await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });

                    //SystemDataConst.UserName = model.LoginName;

                    //HttpContext.Session.Add(SystemDataConst.SessionLoginUserName, loginUser.UserName);
                    //HttpContext.Session.Add(SystemDataConst.SessionLoginDisplayUserName, loginUser.DisplayName);

                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                    BaseDataApiClient.GetInstance().UserLoginSuccess(loginUser);
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = "../Home/Index";
                    }
                    return Json(new { success = true, returnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ModelState.AddModelError("", "登录失败，用户名密码不正确!");
                    BaseDataApiClient.GetInstance().UserLoginFail(new ApplicationUser() { UserName = model.LoginName });
                    return Json(new { success = false, message = "登录失败，用户名密码不正确!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //ModelState.AddModelError("", loginCheckResponse.ResponseResult.ErrorMessage);
                BaseDataApiClient.GetInstance().UserLoginFail(checkLoginUser);

                return Json(new { success = false, message = loginCheckResponse.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SSOLogin(string code)
        {
            SSOUserToken token = AuthorizeSSOManager.GetSSOToken(code);
            if (token != null && !string.IsNullOrEmpty(token.Access_Token))
            {
                AuthUser authUser = AuthorizeSSOManager.GetSSOUser(token.Access_Token);
                if (authUser != null && authUser.Valid == true)
                {
                    ApplicationUser loginUser = new ApplicationUser();
                    loginUser.Id = authUser.Id.ToString();
                    loginUser.UserId = authUser.Id;
                    loginUser.UserName = authUser.UserName;
                    loginUser.DisplayName = authUser.Name;

                    #region 根据UserId获取仓库信息
                    var rsp = BaseDataApiClient.GetInstance().GetWareHouseByUserId(LoginCoreQuery, loginUser.UserId);
                    if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.Count > 0)
                    {
                        loginUser.WarehouseSysId = rsp.ResponseResult[0].SysId;
                        loginUser.WarehouseName = rsp.ResponseResult[0].Name;
                        loginUser.WareHouseList = rsp.ResponseResult;
                    }
                    else
                    {
                        BaseDataApiClient.GetInstance().UserLoginFail(loginUser);
                        return RedirectToAction("NullWarehouseDenied");
                    }
                    #endregion

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                    identity.AddClaim(new Claim(ClaimTypes.Name, authUser.UserName));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginDisplayUserName, loginUser.DisplayName));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginUserID, loginUser.UserId.ToString()));
                    identity.AddClaim(new Claim(SystemDataConst.SessionLoginWarehouseSysId, loginUser.WarehouseSysId.ToString()));
                    identity.AddClaim(new Claim(SystemDataConst.LoginUser, JsonConvert.SerializeObject(loginUser)));

                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                    BaseDataApiClient.GetInstance().UserLoginSuccess(loginUser);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("InternalServerError");
        }

        //
        // POST: RF登录
        [HttpPost]
        public ActionResult RFLogin(string userName, string password)
        {

            Asym_RSA ar = new Asym_RSA();
            try
            {
                userName = ar.JsDecrypt(userName);
                password = ar.JsDecrypt(password);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            userName = ar.Encrypt(userName);
            password = ar.Encrypt(password);


            var systemUser = AuthorizeManager.Login(userName, password);
            if (systemUser != null)
            {
                List<WareHouseDto> wareHouseList = new List<WareHouseDto>();
                #region 根据DepartmentID获取仓库信息
                var rsp = BaseDataApiClient.GetInstance().GetWareHouseByUserId(LoginCoreQuery, systemUser.SysNO);
                if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.Count > 0)
                {
                    wareHouseList = rsp.ResponseResult;
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "登录失败，此用户没有任何仓库权限!"
                    }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                return Json(new
                {
                    success = true,
                    UserName = systemUser.LoginName,
                    DisplayName = systemUser.DisplayName,
                    UserId = systemUser.SysNO,
                    WarehouseList = wareHouseList,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "登录失败，用户名密码不正确!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SSORFLogin(string code)
        {
            SSOUserToken token = AuthorizeSSOManager.GetSSOToken(code);
            if (token != null && !string.IsNullOrEmpty(token.Access_Token))
            {
                AuthUser authUser = AuthorizeSSOManager.GetSSOUser(token.Access_Token);
                if (authUser != null && authUser.Valid == true)
                {
                    List<WareHouseDto> wareHouseList = new List<WareHouseDto>();
                    #region 根据UserId获取仓库信息
                    var rsp = BaseDataApiClient.GetInstance().GetWareHouseByUserId(LoginCoreQuery, authUser.Id);
                    if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.Count > 0)
                    {
                        wareHouseList = rsp.ResponseResult;
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "登录失败，此用户没有任何仓库权限!"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    return Json(new
                    {
                        success = true,
                        UserName = authUser.UserName,
                        DisplayName = authUser.Name,
                        UserId = authUser.Id,
                        WarehouseList = wareHouseList,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                success = false,
                message = "服务器异常，请联系系统管理员!"
            }, JsonRequestBehavior.AllowGet);
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

        public ActionResult NullWarehouseDenied()
        {
            return View();
        }
    }
}