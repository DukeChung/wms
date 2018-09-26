using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Core.Securities
{
    public class WMSUserPrincipal<TKey> : IPrincipal
    {
        private ConcurrentDictionary<string, object> CacheItems { get; set; }

        public WMSUserPrincipal(WMSUserIdentity<TKey> identity)
        {
            Identity = identity;
            CacheItems = new ConcurrentDictionary<string, object>();
        }

        public WMSUserPrincipal(IWMSUser<TKey> user, string sessionKey)
            : this(new WMSUserIdentity<TKey>(user, sessionKey))
        {

        }

        public WMSUserPrincipal()
            : this(new WMSUserIdentity<TKey>())
        {

        }

        public void SetIdentity(WMSUserIdentity<TKey> identity)
        {
            this.Identity = identity;
        }

        /// <summary>
        /// 
        /// </summary>
        public WMSUserIdentity<TKey> Identity { get; private set; }

        IIdentity IPrincipal.Identity => Identity;


        bool IPrincipal.IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 缓存某个值到Identity对象里面
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>成功返回true; 不成功返回false</returns>
        public bool AddItem(string key, object value)
        {
            return this.CacheItems.TryAdd(key, value);
        }

        /// <summary>
        /// 从Identity对象里面获取某个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>存在则返回对应的值，不存在返回null</returns>
        public T GetItem<T>(string key)
            where T : class
        {
            if (CacheItems.ContainsKey(key))
                return CacheItems[key] as T;
            else
                return null;
        }
    }

    public class WMSHttpHeaders
    {
        public const string UserAgent = "UserAgent";
        public const string UserIpAddressV4 = "UserIpAddressV4";
        public const string IgnoreEnvelope = "IgnoreEnvelope";
        public const string ApiToken = "ApiToken";
        public const string AccessLogSysId = "AccessLogSysId";
    }
}
