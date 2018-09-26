using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FortuneLab.WebClient.Navigation
{
    public class SitemapHelper
    {
        private static object navMenuLocker = new object();
        private const string NavMenuKey = "navmenu";

        /// <summary>
        /// 检查并初始化NavMenu
        /// </summary>
        public static void CheckAndInitMenu()
        {
            if (!SiteMapManager.SiteMaps.ContainsKey(SitemapHelper.NavMenuKey))
            {
                lock (navMenuLocker)
                {
                    if (!SiteMapManager.SiteMaps.ContainsKey(SitemapHelper.NavMenuKey))
                    {
                        SiteMapManager.SiteMaps.Register<RemoteXmlSiteMap>(SitemapHelper.NavMenuKey, sitmap => sitmap.LoadFrom(ConfigurationManager.AppSettings["navMenuSitemapUrl"]));
                    }
                }
            }
        }

        /// <summary>
        /// 清除NavMenu缓存
        /// </summary>
        public static void ClearNavMenu()
        {
            if (SiteMapManager.SiteMaps.ContainsKey(SitemapHelper.NavMenuKey))
            {
                lock (navMenuLocker)
                {
                    if (SiteMapManager.SiteMaps.ContainsKey(SitemapHelper.NavMenuKey))
                    {
                        SiteMapManager.SiteMaps.Remove(SitemapHelper.NavMenuKey);
                    }
                }
            }
        }
    }
}