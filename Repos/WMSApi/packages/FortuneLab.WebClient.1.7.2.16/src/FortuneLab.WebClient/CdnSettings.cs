using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient
{
    public static class CdnContentUrlExtensions
    {
        public static Lazy<CdnSettings> Configuration { get; set; }
        public static Dictionary<string, CdnResourceItem> ResUrls { get; set; }

        static CdnContentUrlExtensions()
        {
            Configuration = new Lazy<CdnSettings>(CdnSettings.Load);

            InitCdnUrl();
        }

        public static string CdnContent(this UrlHelper helper, string path, bool forceLocal = false, bool debugFromLocal = false, bool excludeVersion = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            path = path.ToLowerInvariant();
            var underlying = helper.Content(path);

            if (!Configuration.Value.Enabled || forceLocal || debugFromLocal)
            {
                //使用本地地址
                return underlying;
            }

            if (ResUrls.ContainsKey(path))
                underlying = ResUrls[path].url;

            //var cloudRoot = HttpContext.Current.Request.Url.Scheme + "://" + Configuration.Value.CloudFrontDomainName;
            var cloudRoot = "//" + Configuration.Value.CloudFrontDomainName;

            var cdnUrl = underlying.StartsWith("/") ? string.Format("{0}{1}", cloudRoot, underlying) : string.Format("{0}/{1}", cloudRoot, underlying); //underlying.Replace(HttpContext.Current.Request.Url.Host, Configuration.Value.CloudFrontDomainName);

            if (excludeVersion)
            {
                return cdnUrl;
            }
            else
            {
                return string.Format("{0}{1}v={2}", cdnUrl, cdnUrl.IndexOf("?") < 0 ? "?" : "&", DateTime.Now.Date.Ticks.ToString().TrimEnd('0'));
            }
        }

        public static string CdnUrl(this UrlHelper helper)
        {
            return "//" + Configuration.Value.CloudFrontDomainName;
        }

        private static void InitCdnUrl()
        {
            //http://localhost:46110/resource/getresources
            var cloudRoot = string.Format("{0}://{1}", Configuration.Value.HttpSchema, Configuration.Value.CloudFrontDomainName);
            var rsp = WebApiClient.ApiClient.NExecute<List<CdnResourceItem>>(cloudRoot, "resource/getresources", null, useEndpointPreffix: false);
            if (rsp.Success)
            {
                ResUrls = rsp.ResponseResult.ToDictionary(x => x.key.ToLowerInvariant(), y => y);
            }
        }

        public class CdnResourceItem
        {
            public string key { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }
    }

    public class CdnSettings : ConfigurationSection
    {
        [ConfigurationProperty("enabled", DefaultValue = "false", IsRequired = true)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("cloudFrontDomainName", DefaultValue = "", IsRequired = true)]
        public string CloudFrontDomainName
        {
            get { return (string)this["cloudFrontDomainName"]; }
            set { this["cloudFrontDomainName"] = value; }
        }

        [ConfigurationProperty("httpSchema", DefaultValue = "http", IsRequired = false)]
        public string HttpSchema
        {
            get { return (string)this["httpSchema"]; }
            set { this["httpSchema"] = value; }
        }

        public static string SectionName { get { return "frontin"; } }

        public static CdnSettings Load()
        {
            var sectionConfig = ConfigurationManager.GetSection(SectionName);
            if (sectionConfig == null)
                throw new Exception("CDN未配置, 请到Web.Config中配置CDN节");
            return sectionConfig as CdnSettings;
        }
    }
}
