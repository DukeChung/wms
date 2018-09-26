using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Core.Securities
{
    public class WMSUserIdentity<TKey> : IIdentity
    {
        public WMSUserIdentity(IWMSUser<TKey> user, string sessionKey)
        {
            if (user != null)
            {
                IsAuthenticated = true;
                UserId = user.UserId;
                Name = user.LoginName;
                DisplayName = user.DisplayName;
                this.SessionKey = sessionKey;
            }
        }

        public WMSUserIdentity()
        {
            //IsAuthenticated = false;

        }

        public string AuthenticationType
        {
            get { return "WMSAuthentication"; }
        }

        public TKey UserId { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string SessionKey { get; set; }
    }
}
