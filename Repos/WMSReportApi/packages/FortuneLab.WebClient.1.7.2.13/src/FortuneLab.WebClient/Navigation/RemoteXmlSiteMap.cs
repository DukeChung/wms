using Kendo.Mvc;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.Infrastructure.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Web.Routing;
using System.Xml;
using Kendo.Mvc.Extensions;
using System.Net.Http;
using System.Configuration;

namespace FortuneLab.WebClient.Navigation
{
    public class RemoteXmlSiteMap : SiteMapBase
    {
        private const string SiteMapNodeName = "siteMapNode";
        private const string RouteValuesNodeName = "routeValues";
        private const string TitleAttributeName = "title";
        private const string VisibleAttributeName = "visible";
        private const string RouteAttributeName = "route";
        private const string ControllerAttributeName = "controller";
        private const string ActionAttributeName = "action";
        private const string UrlAttributeName = "url";
        private const string LastModifiedAttributeName = "lastModifiedAt";
        private const string ChangeFrequencyAttributeName = "changeFrequency";
        private const string UpdatePriorityAttributeName = "updatePriority";
        private const string IncludeInSearchEngineIndexAttributeName = "includeInSearchEngineIndex";
        private const string AreaAttributeName = "area";
        private static readonly string[] knownAttributes = RemoteXmlSiteMap.CreateKnownAttributes();
        private readonly ICacheProvider cacheProvider;
        private readonly IPathResolver pathResolver;
        private readonly IVirtualPathProvider provider;
        //private static string defaultPath = "~/Web.sitemap";

        private static string sitemapUrlKey = "navMenuSitemapUrl";
        /// <summary>
        /// Gets or sets the default path.
        /// </summary>
        /// <value>The default path.</value>
        public static string DefaultPath
        {
            get
            {
                return ConfigurationManager.AppSettings[RemoteXmlSiteMap.sitemapUrlKey];
            }
            //set
            //{
            //    RemoteXmlSiteMap.sitemapUrlKey = value;
            //}
        }
        public RemoteXmlSiteMap(IPathResolver pathResolver, IVirtualPathProvider provider, ICacheProvider cacheProvider)
        {
            this.pathResolver = pathResolver;
            this.provider = provider;
            this.cacheProvider = cacheProvider;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kendo.Mvc.XmlSiteMap" /> class.
        /// </summary>
        public RemoteXmlSiteMap()
            : this(new PathResolver(), DI.Current.Resolve<IVirtualPathProvider>(), DI.Current.Resolve<ICacheProvider>())
        {
        }
        /// <summary>
        /// Loads from the default path.
        /// </summary>
        public void Load()
        {
            this.LoadFrom(RemoteXmlSiteMap.DefaultPath);
        }
        /// <summary>
        /// Loads from the specified path.
        /// </summary>
        /// <param name="remotePath">The relative virtual path.</param>
        public virtual void LoadFrom(string remotePath)
        {
            if (!string.IsNullOrEmpty(remotePath))
            {
                this.RemoteLoad(remotePath);
            }
        }
        internal void InsertInCache(string filePath)
        {
            string key = base.GetType().AssemblyQualifiedName + ":" + filePath;
            if (this.cacheProvider.Get(key) == null)
            {
                this.cacheProvider.Insert(key, filePath, new CacheItemRemovedCallback(this.OnCacheItemRemoved));
            }
        }
        //internal virtual void InternalLoad(string physicalPath)
        //{
        //    string text = this.provider.ReadAllText(physicalPath);
        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        using (StringReader stringReader = new StringReader(text))
        //        {
        //            using (XmlReader xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
        //            {
        //                CloseInput = true,
        //                IgnoreWhitespace = true,
        //                IgnoreComments = true,
        //                IgnoreProcessingInstructions = true
        //            }))
        //            {
        //                XmlDocument xmlDocument = new XmlDocument();
        //                xmlDocument.Load(xmlReader);
        //                this.Reset();
        //                if (xmlDocument.DocumentElement != null && xmlDocument.HasChildNodes)
        //                {
        //                    base.CacheDurationInMinutes = RemoteXmlSiteMap.GetFloatValueFromAttribute(xmlDocument.DocumentElement, "cacheDurationInMinutes", SiteMapBase.DefaultCacheDurationInMinutes);
        //                    base.Compress = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "compress", true);
        //                    base.GenerateSearchEngineMap = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "generateSearchEngineMap", true);
        //                    XmlNode firstChild = xmlDocument.DocumentElement.FirstChild;
        //                    RemoteXmlSiteMap.Iterate(base.RootNode, firstChild);
        //                    this.InsertInCache(physicalPath);
        //                }
        //            }
        //        }
        //    }
        //}

        internal void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.DependencyChanged)
            {
                this.RemoteLoad((string)value);
            }
        }

        internal virtual void RemoteLoad(string remotePath)
        {
            HttpClient client = new HttpClient();
            var respose = client.GetAsync(remotePath).Result;
            if (!respose.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Can't get menu config file from remote path: {0}", remotePath));
            }

            var text = respose.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(text))
            {
                using (StringReader stringReader = new StringReader(text))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
                    {
                        CloseInput = true,
                        IgnoreWhitespace = true,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true
                    }))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlReader);
                        this.Reset();
                        if (xmlDocument.DocumentElement != null && xmlDocument.HasChildNodes)
                        {
                            base.CacheDurationInMinutes = RemoteXmlSiteMap.GetFloatValueFromAttribute(xmlDocument.DocumentElement, "cacheDurationInMinutes", SiteMapBase.DefaultCacheDurationInMinutes);
                            base.Compress = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "compress", true);
                            base.GenerateSearchEngineMap = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlDocument.DocumentElement, "generateSearchEngineMap", true);
                            XmlNode firstChild = xmlDocument.DocumentElement.FirstChild;
                            RemoteXmlSiteMap.Iterate(base.RootNode, firstChild);
                            this.InsertInCache(remotePath);
                        }
                    }
                }
            }
        }

        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            RemoteXmlSiteMap.PopulateNode(siteMapNode, xmlNode);
            foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
            {
                if (xmlNode2.LocalName.IsCaseSensitiveEqual("siteMapNode"))
                {
                    SiteMapNode siteMapNode2 = new SiteMapNode();
                    siteMapNode.ChildNodes.Add(siteMapNode2);
                    RemoteXmlSiteMap.Iterate(siteMapNode2, xmlNode2);
                }
            }
        }
        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            XmlNode firstChild = xmlNode.FirstChild;
            if (firstChild != null && firstChild.LocalName.IsCaseSensitiveEqual("routeValues"))
            {
                foreach (XmlNode xmlNode2 in firstChild.ChildNodes)
                {
                    routeValueDictionary[xmlNode2.LocalName] = xmlNode2.InnerText;
                }
            }
            siteMapNode.Title = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "title");
            siteMapNode.Visible = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlNode, "visible", true);
            string stringValueFromAttribute = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "route");
            string stringValueFromAttribute2 = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "controller");
            string stringValueFromAttribute3 = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "action");
            string stringValueFromAttribute4 = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "url");
            string stringValueFromAttribute5 = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "area");
            string stringValueFromAttribute6 = RemoteXmlSiteMap.GetStringValueFromAttribute(xmlNode, "subdomain");
            if (stringValueFromAttribute5 != null)
            {
                routeValueDictionary["area"] = stringValueFromAttribute5;
            }
            if (stringValueFromAttribute6 != null)
            {
                routeValueDictionary["subdomain"] = stringValueFromAttribute6;
            }
            if (!string.IsNullOrEmpty(stringValueFromAttribute))
            {
                siteMapNode.RouteName = stringValueFromAttribute;
                siteMapNode.RouteValues.Clear();
                siteMapNode.RouteValues.Merge(routeValueDictionary);
            }
            else
            {
                if (!string.IsNullOrEmpty(stringValueFromAttribute2) && !string.IsNullOrEmpty(stringValueFromAttribute3))
                {
                    siteMapNode.ControllerName = stringValueFromAttribute2;
                    siteMapNode.ActionName = stringValueFromAttribute3;
                    siteMapNode.RouteValues.Clear();
                    siteMapNode.RouteValues.Merge(routeValueDictionary);
                }
                else
                {
                    if (!string.IsNullOrEmpty(stringValueFromAttribute4))
                    {
                        siteMapNode.Url = stringValueFromAttribute4;
                    }
                }
            }
            DateTime? dateValueFromAttribute = RemoteXmlSiteMap.GetDateValueFromAttribute(xmlNode, "lastModifiedAt");
            if (dateValueFromAttribute.HasValue)
            {
                siteMapNode.LastModifiedAt = new DateTime?(dateValueFromAttribute.Value);
            }
            siteMapNode.IncludeInSearchEngineIndex = RemoteXmlSiteMap.GetBooleanValueFromAttribute(xmlNode, "includeInSearchEngineIndex", true);
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                if (!string.IsNullOrEmpty(xmlAttribute.LocalName) && Array.BinarySearch<string>(RemoteXmlSiteMap.knownAttributes, xmlAttribute.LocalName, StringComparer.OrdinalIgnoreCase) < 0)
                {
                    siteMapNode.Attributes.Add(xmlAttribute.LocalName, xmlAttribute.Value);
                }
            }
        }
        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            string result = null;
            if (node.Attributes.Count > 0)
            {
                XmlAttribute xmlAttribute = node.Attributes[attributeName];
                if (xmlAttribute != null)
                {
                    result = xmlAttribute.Value;
                }
            }
            return result;
        }
        private static bool GetBooleanValueFromAttribute(XmlNode node, string attributeName, bool defaultValue)
        {
            bool result = defaultValue;
            string stringValueFromAttribute = RemoteXmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            bool flag;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && bool.TryParse(stringValueFromAttribute, out flag))
            {
                result = flag;
            }
            return result;
        }
        private static float GetFloatValueFromAttribute(XmlNode node, string attributeName, float defaultValue)
        {
            float result = defaultValue;
            string stringValueFromAttribute = RemoteXmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            float num;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && float.TryParse(stringValueFromAttribute, out num))
            {
                result = num;
            }
            return result;
        }
        private static DateTime? GetDateValueFromAttribute(XmlNode node, string attributeName)
        {
            string stringValueFromAttribute = RemoteXmlSiteMap.GetStringValueFromAttribute(node, attributeName);
            DateTime? result = null;
            DateTime value;
            if (!string.IsNullOrEmpty(stringValueFromAttribute) && DateTime.TryParse(stringValueFromAttribute, out value))
            {
                result = new DateTime?(value);
            }
            return result;
        }
        private static string[] CreateKnownAttributes()
        {
            List<string> list = new List<string>(new string[]
			{
				"title",
				"visible",
				"route",
				"controller",
				"action",
				"url",
				"lastModifiedAt",
				"changeFrequency",
				"updatePriority",
				"includeInSearchEngineIndex"
			});
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list.ToArray();
        }
    }
}