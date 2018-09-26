using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Security.AuthClient;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace FortuneLab.WebClient.Mvc
{
    public class CurrentSessionInfo
    {
        const string CurrentSessionInfoCacheKey = "CurrentSessionInfoCacheKey";
        public static CurrentSessionInfo Instance
        {
            get
            {
                if (HttpContext.Current.Items[CurrentSessionInfoCacheKey] == null)
                {
                    HttpContext.Current.Items.Add(CurrentSessionInfoCacheKey, new CurrentSessionInfo());
                }
                return HttpContext.Current.Items[CurrentSessionInfoCacheKey] as CurrentSessionInfo;
            }
        }

        private CurrentSessionInfo()
        {

            var identity = HttpContext.Current.User.Identity;
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) return;

            var claims = claimsIdentity.Claims.ToDictionary(x => x.Type, x => x.Value);
            SessionKey = claims.ContainsKey(AuthConstants.SessionKey) ? claims[AuthConstants.SessionKey] : null;
            ApplicationUser = claims.ContainsKey(AuthConstants.LogonUser) ? JsonConvert.DeserializeObject<ApplicationUser>(claims[AuthConstants.LogonUser]) : null;
        }

        //private Dictionary<string, string> _claims = null;
        //private readonly object _claimsLocker = new object();
        //private Dictionary<string, string> Claims
        //{
        //    get
        //    {
        //        if (_claims == null)
        //        {
        //            lock (_claimsLocker)
        //            {
        //                var identity = HttpContext.Current.User.Identity;
        //                if (_claims == null && identity is ClaimsIdentity)
        //                {
        //                    _claims = ((ClaimsIdentity)identity).Claims.ToDictionary(x => x.Type, x => x.Value);
        //                    Debug.Assert(_claims != null, "claims != null");
        //                }
        //            }
        //        }
        //        return _claims;
        //    }
        //}
        public string SessionKey { get; private set; }
        public ApplicationUser ApplicationUser { get; private set; }
    }
}
