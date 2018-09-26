using NBK.ECService.WMS.Core.Securities;
using NBK.ECService.WMS.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Core.WebApi.ApplicationService
{
    public class WMSApplicationService : Abp.Application.Services.ApplicationService
    {
        public WMSApplicationService()
        {

        }

        protected WMSUserIdentity<int> UserIdentity
        {
            get
            {
                return Thread.CurrentPrincipal.Identity as WMSUserIdentity<int>;
            }
        }

        protected WMSUserPrincipal<int> WMSUserPrincipal
        {
            get { return Thread.CurrentPrincipal as WMSUserPrincipal<int>; }
        }

        protected Guid? AccessLogSysId
        {
            get
            {
                Guid sysId = Guid.Empty;
                if (Guid.TryParse(WMSUserPrincipal.GetItem<string>(WMSHttpHeaders.AccessLogSysId), out sysId))
                {
                    return sysId;
                }
                return null;
            }
        }

        public IWMSSession WMSSession { get; set; }
    }
}
