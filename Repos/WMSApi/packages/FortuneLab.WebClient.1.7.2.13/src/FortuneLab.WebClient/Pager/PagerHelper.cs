/*
 ASP.NET MvcPager control
 Copyright:2009-2010 Webdiyer (http://en.webdiyer.com)
 Source code released under Ms-PL license
 */
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Webdiyer.WebControls.Mvc
{
    public static class PagerHelper
    {

        #region Html Pager
        /// <summary>
        /// Page对象初始化
        /// Controller & Action 优先级依次为: 单独的构造参数 -> RouteValues中的设定项 -> 默认的设定项(当前设定项)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="totalPageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="actionName">优先级依次为: 单独的ActionName -> RouteValues中的Action -> 默认的Action</param>
        /// <param name="controllerName">优先级依次为: 单独的ControllerName -> RouteValues中的Controller -> 默认的Controller</param>
        /// <param name="pagerOptions"></param>
        /// <param name="routeName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static string Pager(this HtmlHelper helper, int totalPageCount, int pageIndex, string actionName, string controllerName, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    new RouteValueDictionary(routeValues),
                    new RouteValueDictionary(htmlAttributes)
                );
            return builder.RenderPager();
        }


        public static string Pager(this HtmlHelper helper, int totalPageCount, int pageIndex, string actionName, string controllerName, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    routeValues,
                    htmlAttributes
                );
            return builder.RenderPager();
        }

        private static string Pager(HtmlHelper helper,PagerOptions pagerOptions,IDictionary<string,object> htmlAttributes)
        {
            return new PagerBuilder(helper, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList)
        {
            if (pagedList == null)
                return Pager(helper,null, null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null,null, null,null);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions)
        {
            if(pagedList==null)
                return Pager(helper,pagerOptions,null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions,null, null,null);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions,object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null,htmlAttributes);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, IDictionary<string,object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        /*
        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string actionName,string controllerName,PagerOptions pagerOptions,object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, 0, 1,actionName, controllerName, pagerOptions, null, null,  htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, pagerOptions, null, null, htmlAttributes);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string actionName, string controllerName, PagerOptions pagerOptions, IDictionary<string,object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, 0, 1, actionName, controllerName, pagerOptions, null, null, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, pagerOptions, null, null, htmlAttributes);
        }
        */

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions,null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions,null);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,pagerOptions,htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,null,new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues, IDictionary<string,object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper,null, htmlAttributes);
            return Pager(helper, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        #endregion

        #region jQuery Ajax Pager
        
        private static string AjaxPager(HtmlHelper html,PagerOptions pagerOptions,IDictionary<string,object> htmlAttributes)
        {
            return new PagerBuilder(html,null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static string AjaxPager(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName, string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }

        public static string AjaxPager(this HtmlHelper html, int totalPageCount, int pageIndex, string actionName, string controllerName, string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html,null,null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions,
                             null);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList,string routeName, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html,null,null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, null, ajaxOptions,
                             null);
        }

        public static string AjaxPager<T>(this HtmlHelper html,PagedList<T> pagedList,PagerOptions pagerOptions,AjaxOptions ajaxOptions)
        {
            if(pagedList==null)
                return AjaxPager(html,pagerOptions,null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions,object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,htmlAttributes);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions,
                             null);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,htmlAttributes);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static string AjaxPager<T>(this HtmlHelper html, PagedList<T> pagedList, string actionName, string controllerName, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html,pagerOptions,null);
            return AjaxPager(html, pagedList.TotalPageCount, pagedList.CurrentPageIndex, actionName, controllerName, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        #endregion

        #region Microsoft Ajax Pager

        public static string Pager(this AjaxHelper ajax, int totalPageCount, int pageIndex, string actionName, string controllerName, string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }
        
        public static string Pager(this AjaxHelper ajax, int totalPageCount, int pageIndex, string actionName, string controllerName, string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string,object> htmlAttributes)
        {
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        private static string Pager(AjaxHelper ajax,PagerOptions pagerOptions,IDictionary<string,object> htmlAttributes)
        {
            return new PagerBuilder(null, ajax, pagerOptions, htmlAttributes).RenderPager();
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, AjaxOptions ajaxOptions)
        {
            return pagedList==null ? Pager(ajax,null, null) : Pager(ajax,pagedList.TotalPageCount,pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions, null);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList,PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax,pagerOptions,null) : Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null,pagerOptions, null, ajaxOptions, null);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if(pagedList==null)
                return Pager(ajax,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string,object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax,pagerOptions,htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if(pagedList==null)
                return Pager(ajax,null,new RouteValueDictionary(htmlAttributes));
            return Pager(ajax,pagedList.TotalPageCount,pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions,IDictionary<string,object> htmlAttributes)
        {
            if(pagedList==null)
                return Pager(ajax,null, htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName,object routeValues,PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if(pagedList==null)
                return Pager(ajax,pagerOptions,new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }

        public static string Pager<T>(this AjaxHelper ajax, PagedList<T> pagedList, string routeName, RouteValueDictionary routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string,object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax,pagerOptions,htmlAttributes);
            return Pager(ajax, pagedList.TotalPageCount, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }
        

        
        #endregion
    }
}