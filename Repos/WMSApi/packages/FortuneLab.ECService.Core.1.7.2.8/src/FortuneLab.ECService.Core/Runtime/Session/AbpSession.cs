using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Runtime.Session
{
    /// <summary>
    /// Implements IAbpSession to get session informations from ASP.NET Identity framework.
    /// </summary>
    public class AbpSession : IAbpSession, ISingletonDependency
    {
        private FortuneLabUserPrincipal<int> Principal
        {
            get { return Thread.CurrentPrincipal as FortuneLabUserPrincipal<int>; }
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

        public int? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return 1; //TODO@hikalkan: This assumption may not be good?
                }

                var value = GetValueForClaimType(AbpClaimTypes.TenantId);
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return Convert.ToInt32(value);
            }
        }

        public string UserName
        {
            get
            {
                if (Principal != null)
                    return Principal.Identity.Name;
                return null;
            }
        }

        public string TenancyName
        {
            get
            {
                return GetValueForClaimType(AbpClaimTypes.TenantId);
            }
        }

        public MultiTenancySides MultiTenancySide
        {
            get
            {
                return _multiTenancy.IsEnabled && !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        public virtual string GetValueForClaimType(string claimType)
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return null;
            }

            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return null;
            }
            return claim.Value;
        }

        private readonly IMultiTenancyConfig _multiTenancy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;
        }

        public int? ImpersonatorTenantId
        {
            get { return null; }
        }

        public long? ImpersonatorUserId
        {
            get { return null; }
        }
    }
}
