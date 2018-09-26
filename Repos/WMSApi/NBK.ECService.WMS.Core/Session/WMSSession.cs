using Abp.Dependency;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.MultiTenancy;
using Abp.Configuration.Startup;
using System.Threading;
using NBK.ECService.WMS.Core.Securities;

namespace NBK.ECService.WMS.Core.Session
{
    /// <summary>
    /// Implements IAbpSession to get session informations from ASP.NET Identity framework.
    /// </summary>
    public class WMSSession : IWMSSession, ISingletonDependency
    {
        private WMSUserPrincipal<int> Principal
        {
            get { return Thread.CurrentPrincipal as WMSUserPrincipal<int>; }
        }

        public int? ImpersonatorTenantId
        {
            get { return null; }
        }

        public long? ImpersonatorUserId
        {
            get { return null; }
        }
        private readonly IMultiTenancyConfig _multiTenancy;
        public MultiTenancySides MultiTenancySide
        {
            get
            {
                return _multiTenancy.IsEnabled && !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        public int? TenantId
        {
            get { return null; }
        }

        public long? UserId
        {
            get
            {
                if (Principal != null)
                    return Principal.Identity.UserId;
                return null;
            }
        }
    }
}
