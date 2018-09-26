using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Kendo.Mvc.Extensions;
using System.Web.Mvc.Html;
using System.Configuration;
using System.Text;

namespace FortuneLab.WebClient.Navigation
{
    public class NavUrlGenerator : IUrlGenerator
    {
        public static NavUrlGenerator Instance = new NavUrlGenerator();

        public string Generate(RequestContext requestContext, string url)
        {
            return new UrlHelper(requestContext).Content(url);
        }
        public string Generate(RequestContext requestContext, INavigatable navigationItem, RouteValueDictionary routeValues)
        {
            //如果Controller没有设置，则认为此菜单无链接
            if (string.IsNullOrEmpty(navigationItem.ControllerName))
            {
                return null;
            }

            UrlHelper urlHelper = new UrlHelper(requestContext);
            string result = null;

            var subDomain = routeValues.ContainsKey("subdomain") ? routeValues["subdomain"].ToString() : null;

            if (!string.IsNullOrWhiteSpace(subDomain))
            {
                //Clear subdomain from route values
                routeValues.Remove("subdomain");
            }

            if (!routeValues.ContainsKey("area"))
            {
                routeValues.Add("area", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(subDomain) && !IsLocalSystem(subDomain))
            {
                result = GenerateWithSubdomain(requestContext, navigationItem, routeValues, urlHelper, subDomain);
            }
            else if (!string.IsNullOrEmpty(navigationItem.ControllerName) && !string.IsNullOrEmpty(navigationItem.ActionName))
            {
                result = urlHelper.Action(navigationItem.ActionName, navigationItem.ControllerName, routeValues, null, null);
            }
            else
            {
                if (!string.IsNullOrEmpty(navigationItem.Url))
                {
                    result = (navigationItem.Url.StartsWith("~/", StringComparison.Ordinal) ? urlHelper.Content(navigationItem.Url) : navigationItem.Url);
                }
                else
                {
                    if (routeValues.Any<KeyValuePair<string, object>>())
                    {
                        result = urlHelper.RouteUrl(routeValues);
                    }
                }
            }
            return result;
        }

        private static string GenerateWithSubdomain(RequestContext requestContext, INavigatable navigationItem, RouteValueDictionary routeValues, UrlHelper urlHelper, string subDomain)
        {
            if (!string.IsNullOrEmpty(navigationItem.ControllerName) && !string.IsNullOrEmpty(navigationItem.ActionName))
            {
                StringBuilder sbUrl = new StringBuilder();
                if (routeValues["area"] != null && !string.IsNullOrWhiteSpace(routeValues["area"].ToString()))
                {
                    sbUrl.AppendFormat("{0}/{1}/{2}", routeValues["area"], navigationItem.ControllerName, navigationItem.ActionName);
                }
                else
                {
                    sbUrl.AppendFormat("{0}/{1}", navigationItem.ControllerName, navigationItem.ActionName);
                }
                return string.Format("http://{0}.{1}/{2}", subDomain, ConfigurationManager.AppSettings["domain"], sbUrl.ToString());
            }
            throw new NotSupportedException("不支持非controller, action的URL生成形式");
        }

        private bool IsLocalSystem(string routeSubdomain)
        {
            return routeSubdomain.Equals(ConfigurationManager.AppSettings["subdomain"], StringComparison.OrdinalIgnoreCase);
        }

        public string Generate(RequestContext requestContext, INavigatable navigationItem)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (navigationItem.RouteValues.Any<KeyValuePair<string, object>>())
            {
                routeValueDictionary.Merge(navigationItem.RouteValues);
            }
            return this.Generate(requestContext, navigationItem, routeValueDictionary);
        }
    }

    //public static class UrlExtensions
    //{
    //    public static string RouteUrl(this UrlHelper urlHelper, string routeName, object routeValues, bool requireAbsoluteUrl)
    //    {
    //        var dicRouteValues = new RouteValueDictionary(routeValues);
    //        if (dicRouteValues.ContainsKey("subdomain"))
    //        {
    //            return urlHelper.RouteUrl(routeName, dicRouteValues, "http", string.Format("{0}.yaomaiche.com", dicRouteValues["subdomain"]));
    //        }
    //        return urlHelper.RouteUrl(routeName, routeName);
    //    }
    //}
}