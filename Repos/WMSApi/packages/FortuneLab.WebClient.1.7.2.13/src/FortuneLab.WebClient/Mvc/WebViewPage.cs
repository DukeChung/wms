using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Security;
using FortuneLab.WebClient.Security.AuthClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Mvc
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public Security.AuthorizationService PermissionService => Security.AuthorizationService.Instance;

        public CurrentSessionInfo CurrentSessionInfo => CurrentSessionInfo.Instance;

        public WebViewPage()
        {
        }

        /// <summary>
        /// 当前Sessionkey
        /// </summary>
        public string SessionKey
        {
            get
            {
                return CurrentSessionInfo.SessionKey;
            }
        }

        /// <summary>
        /// 当前登录用户信息(简版User, Cookie中提取的)
        /// </summary>
        public ApplicationUser ApplicationUser => CurrentSessionInfo.ApplicationUser;

        ///// <summary>
        ///// 当前的LoginSession
        ///// </summary>
        //[Obsolete("改用SessionQuery")]
        //public SessionQuery LoginQuery { get { return SessionQuery; } }

        //public SessionQuery SessionQuery { get { return CurrentSessionInfo; } }

        public string DisplayName => ApplicationUser.DisplayName ?? string.Empty;

        public override void InitHelpers()
        {
            base.InitHelpers();
        }

        protected override void InitializePage()
        {
            base.InitializePage();
        }

        /// <summary>
        /// GenericCanonicalUrl
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public string GCU(string relativePath)
        {
            return string.Format("{0}{1}", Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf(Request.Url.PathAndQuery)), relativePath);
        }

        protected IHtmlString GetPropertyLinkHTML(string propertyName)
        {
            return Html.Raw(String.Format(
                @"<a href=""http://51degrees.com/Resources/Property-Dictionary#{0}"">{0}</a>",
                propertyName));
        }

        protected IHtmlString GetPropertyHTML(string propertyName)
        {
            var value = Request.Browser[propertyName];
            if (String.IsNullOrEmpty(value))
                value = @"<a href=""http://51degrees.com/Products/Device-Detection"">Upgrade</a>";
            return Html.Raw(value);
        }
        public IHtmlString IsMenuActive(string fstNav = null, string sndNav = null, string thdNav = null, string className = " active ")
        {
            var firstNavName = ViewBag.FirstNavName ?? string.Empty;
            var secondNavName = ViewBag.SecondNavName ?? string.Empty;
            var thirdNavName = ViewBag.ThirdNavName ?? string.Empty;

            if (string.Equals(firstNavName, fstNav, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(sndNav) ? true : string.Equals(secondNavName, sndNav, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(thdNav) ? true : string.Equals(thirdNavName, thdNav, StringComparison.OrdinalIgnoreCase)))
            {
                return new MvcHtmlString(className);
            }
            return MvcHtmlString.Empty;
        }

        [Obsolete("请使用FunctionAuthorize替换")]
        public bool Authorize(string permissionName)
        {
            return FunctionAuthorize(permissionName);
        }

        public bool RoleAuthorize(string roleName)
        {
            return AuthorizationService.Instance.RoleAuthorize(CurrentSessionInfo.ApplicationUser.UserId, roleName);
        }

        public bool FunctionAuthorize(string authKey)
        {
            return AuthorizationService.Instance.FunctionAuthorize(CurrentSessionInfo.ApplicationUser.UserId, authKey);
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {

    }

    public static partial class EnumHelperExtension
    {
        public static List<SelectListItem> GetSelectList(this HtmlHelper obj, Type enumType, string selectedValue = null)
        {
            return Enum.GetNames(enumType).OrderBy(x => x).Select(x => new SelectListItem() { Text = x, Value = x, Selected = string.Equals(x, selectedValue, StringComparison.OrdinalIgnoreCase) }).ToList();

        }
    }
}
