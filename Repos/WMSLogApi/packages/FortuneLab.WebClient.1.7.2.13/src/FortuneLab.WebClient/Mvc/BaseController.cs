using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Common;
using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Mvc.ViewModels;
using FortuneLab.WebClient.Security.AuthClient;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Hosting;
using FortuneLab.WebClient.Navigation;
using FortuneLab.WebClient.Security;
using Pjax.Mvc5;

namespace FortuneLab.WebClient.Mvc
{
    [PjaxAttribute, Authorize]
    public class BaseController : Controller, IPjax
    {
        #region Pjax Property
        public bool IsPjaxRequest { get; set; }

        public string PjaxVersion { get; set; }
        #endregion

        public CurrentSessionInfo CurrentSessionInfo => CurrentSessionInfo.Instance;
        public Security.AuthorizationService PermissionService => Security.AuthorizationService.Instance;

        public BaseController()
        {

        }

        /// <summary>
        /// 当前Sessionkey
        /// </summary>
        protected string SessionKey => CurrentSessionInfo.SessionKey;

        /// <summary>
        /// 当前登录用户信息(简版User, Cookie中提取的)
        /// </summary>
        public ApplicationUser ApplicationUser => CurrentSessionInfo.ApplicationUser;

        /// <summary>
        /// 当前的LoginSession
        /// </summary>

        public LoginQuery LoginQuery => new ListQuery() { SessionKey = SessionKey };
        public SessionQuery SessionQuery => new SessionQuery() { SessionKey = SessionKey };

        [Obsolete("请使用ApplicationUser获取当前用户对象", true)]
        public User LogonUser
        {
            get
            {
                if (!Request.IsAuthenticated)
                    throw new UnauthorizedAccessException("登录之后才可以访问LogonUser对象");
                //return UsersCache.Instance.Get(User.Identity.Name);
                return null;
            }
        }

        protected string GetAbsolutePath(string returnUrl)
        {
            return string.Format("{0}{1}", Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, string.Empty), returnUrl);
        }

        [AllowAnonymous]
        public ContentResult KeepAlive()
        {
            return Content("alive");
        }

        ///// <summary>
        ///// Render partial view to string
        ///// </summary>
        ///// <returns>Result</returns>
        //public virtual string RenderPartialViewToString()
        //{
        //    return RenderPartialViewToString(null, null);
        //}
        ///// <summary>
        ///// Render partial view to string
        ///// </summary>
        ///// <param name="viewName">View name</param>
        ///// <returns>Result</returns>
        //public virtual string RenderPartialViewToString(string viewName)
        //{
        //    return RenderPartialViewToString(viewName, null);
        //}
        ///// <summary>
        ///// Render partial view to string
        ///// </summary>
        ///// <param name="model">Model</param>
        ///// <returns>Result</returns>
        //public virtual string RenderPartialViewToString(object model)
        //{
        //    return RenderPartialViewToString(null, model);
        //}
        ///// <summary>
        ///// Render partial view to string
        ///// </summary>
        ///// <param name="viewName">View name</param>
        ///// <param name="model">Model</param>
        ///// <returns>Result</returns>
        //public virtual string RenderPartialViewToString(string viewName, object model)
        //{
        //    //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = this.ControllerContext.RouteData.GetRequiredString("action");

        //    this.ViewData.Model = model;

        //    using (var sw = new StringWriter())
        //    {
        //        ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
        //        var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);

        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        [Obsolete("弃用状态", true)]
        protected void LogException(Exception exc)
        {
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //var logger = EngineContext.Current.Resolve<ILogger>();

            //var customer = workContext.CurrentCustomer;
            //logger.Error(exc.Message, exc, customer);
        }

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [Obsolete("弃用状态", true)]
        public virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [Obsolete("弃用状态,目前无效", true)]
        public virtual void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }

        [Obsolete("弃用状态,目前无效", true)]
        public virtual void ErrorNotification(IApiResponse response)
        {
            ErrorNotification(response.ApiMessage != null ? response.ApiMessage.ToString() : response.Message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        [Obsolete("弃用状态,目前无效", true)]
        public virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            if (logException) LogException(exception);
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [Obsolete("弃用状态,目前无效", true)]
        public virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("flab.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }

        //[Obsolete("弃用状态,目前无效", true)]
        //public ActionResult AjaxFromValidationPartial()
        //{
        //    return PartialView("_AjaxFormValidationView");
        //}

        #region 前端Form Ajax提交处理
        /// <summary>
        /// 前端Ajax From提交的处理结果， 统一处理之后交给前端 Global Ajax处理
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        protected JsonResult AjaxSubmitResult(bool isSuccess)
        {
            this.Response.AddHeader("formId", this.Request.Headers["fromId"] ?? "");

            if (isSuccess)
                return Json(new AjaxRequestResult<bool>("ajaxSubmit", true), JsonRequestBehavior.AllowGet);

            var errorInfo = new List<ErrorItem>();

            var errors = ModelState.Values.Where(x => x.Errors.Count > 0).ToList();

            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key].Errors.Count > 0)
                {
                    errorInfo.AddRange(ModelState[key].Errors.Select(x => new ErrorItem { errorCode = key, errorMessage = x.ErrorMessage }));
                }
            }
            return Json(new AjaxRequestResult<List<ErrorItem>>("ajaxSubmit", errorInfo), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 前端Form提交失败
        /// </summary>
        /// <returns></returns>
        protected JsonResult AjaxSubmitError()
        {
            return AjaxSubmitResult(false);
        }

        /// <summary>
        /// 前端Form提示成功
        /// </summary>
        /// <returns></returns>
        protected JsonResult AjaxSubmitSuccess()
        {
            return AjaxSubmitResult(true);
        }
        #endregion

        #region Kendo数据源Result
        [Obsolete("请直接返回KendoResult")]
        protected JsonResult KendoGridDataResult<TApiResponseResult>(ApiResponse<Page<TApiResponseResult>> response, ListQueryModel request)
        {
            if (response.Success)
            {
                return Json(response.ResponseResult.ToDataSourceResult<TApiResponseResult>(request), JsonRequestBehavior.AllowGet);
            }

            var result = new DataSourceResult() { };
            result.Errors = response.ApiMessage;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 为Kendo控件提供数据格式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected JsonResult KendoResult<TResult>(ApiResponse<Page<TResult>> response)
        {
            return response.ToKendoResult();
        }

        /// <summary>
        /// 为Kendo控件提供数据格式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dataList"></param>
        /// <returns></returns>
        protected JsonResult KendoResult<TResult>(ApiResponse<List<TResult>> response)
        {
            return response.ToKendoResult();
        }
        #endregion

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            if (Request.IsAjaxRequest())
            {
                Response.StatusCode = 403;
                return Json(new { code = "NoPermission", msg = "未授权的访问" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 403;
                return Content("未授权的访问");
            }
        }

        [Obsolete("请使用FunctionAuthorize替换", true)]
        public bool Authorize(string permissionName)
        {
            return FunctionAuthorize(permissionName);
        }

        /// <summary>
        /// 根据AuthKey授权
        /// </summary>
        /// <param name="authKey"></param>
        /// <returns></returns>
        public bool FunctionAuthorize(string authKey)
        {
            return AuthorizationService.Instance.FunctionAuthorize(ApplicationUser.UserId, authKey);
        }

        /// <summary>
        /// 根据角色名称授权
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool RoleAuthorize(string roleName)
        {
            return AuthorizationService.Instance.RoleAuthorize(ApplicationUser.UserId, roleName);
        }

        public ActionResult ClearCache(string returnUrl = "")
        {
            SitemapHelper.ClearNavMenu();

            if (Request.IsAuthenticated)
                AuthorizationService.Instance.ClearAuthorizationCache(ApplicationUser.UserId);

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home");
            return Redirect(returnUrl);
        }


        public ActionResult RestartApplication(string returnUrl = "")
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
            //    return AccessDeniedView();
            WebHelper webHelper = new WebHelper();
            //restart application
            webHelper.RestartAppDomain();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home");
            return Redirect(returnUrl);
        }
    }

    public enum NotifyType
    {
        Error,
        Success
    }
}
