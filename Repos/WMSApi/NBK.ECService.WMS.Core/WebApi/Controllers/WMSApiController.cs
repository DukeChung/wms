using Abp.Dependency;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Core.Securities;
using NBK.ECService.WMS.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Core.WebApi.Controllers
{
    /// <summary>
    /// 所有项目API Controller基类
    /// </summary>
    public class WMSApiController : AbpApiController
    {
        public WMSApiController()
        {
            this.CurrentSession = IocManager.Instance.Resolve<IWMSSession>();
        }

        /// <summary>
        /// 当前Session
        /// </summary>
        protected new IWMSSession CurrentSession { get; private set; }

        protected WMSUserIdentity<int> UserIdentity
        {
            get
            {
                return Thread.CurrentPrincipal.Identity as WMSUserIdentity<int>;
            }
        }
    }
}
