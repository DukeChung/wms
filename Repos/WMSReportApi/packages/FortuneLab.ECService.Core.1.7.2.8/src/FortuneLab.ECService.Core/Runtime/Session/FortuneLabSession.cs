using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Securities;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.Runtime;
using FortuneLab.Runtime.Session;
using System.Threading;

namespace FortuneLab.ECService.Runtime.Session
{
    public class FortuneLabSession : AbpSession, IFortuneLabSession, ISingletonDependency
    {
        public FortuneLabSession()
            : base(IocManager.Instance.Resolve<IMultiTenancyConfig>())
        {

        }

        public FortuneLabUserPrincipal<int> Principal
        {
            get { return Thread.CurrentPrincipal as FortuneLabUserPrincipal<int>; }
        }

        public FortuneLabUserIdentity<int> Identity
        {
            get
            {
                return Principal == null ? null : Principal.Identity;
            }
        }

        /// <summary>
        /// 获取客户端访问的UserAgent
        /// </summary>
        public string UserAgent
        {
            get
            {
                return Principal.GetItem<string>(FortuneLabHttpHeaders.UserAgent);
            }
        }

        /// <summary>
        /// 获取客户端访问的IP
        /// </summary>
        public string UserIpAddress
        {
            get
            {
                return Principal.GetItem<string>(FortuneLabHttpHeaders.UserIpAddressV4);
            }
        }

        public ApiToken ApiToken
        {
            get
            {
                return Principal.GetItem<ApiToken>(FortuneLabHttpHeaders.ApiToken);
            }
        }
    }
}
