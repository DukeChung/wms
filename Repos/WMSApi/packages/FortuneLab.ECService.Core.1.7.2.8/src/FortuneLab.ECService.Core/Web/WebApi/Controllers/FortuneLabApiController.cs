using Abp.Dependency;
using Abp.Securities;
using Abp.Web.WebApi.Controllers.Filters;
using Abp.WebApi.Controllers;
using FortuneLab.ECService.Securities.Filters;
using FortuneLab.Runtime.Session;
using FortuneLab.WebApiClient.Query;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Linq;
using System.Web.Http;

namespace FortuneLab.ECService.Web.WebApi.Controllers
{
    /// <summary>
    /// 所有项目API Controller基类
    /// </summary>
    public class FortuneLabApiController : AbpApiController
    {
        public FortuneLabApiController()
        {
            this.CurrentSession = IocManager.Instance.Resolve<IFortuneLabSession>();
        }

        /// <summary>
        /// 当前Session
        /// </summary>
        protected new IFortuneLabSession CurrentSession { get; private set; }


        private SessionQuery _sessionQuery = null;
        public SessionQuery SessionQuery
        {
            get
            {
                if (UserIdentity == null)
                    throw new Exception(string.Format("Current Thread.CurrentPrincipal.Identity is {0}, It is not FortuneLabUserIdentity<int>",
                        Thread.CurrentPrincipal.Identity == null ? "NULL" : Thread.CurrentPrincipal.Identity.GetType().FullName));

                if (_sessionQuery == null)
                    _sessionQuery = new SessionQuery() { SessionKey = UserIdentity.SessionKey };
                return _sessionQuery;
            }
        }

        private PlatformQuery _platformQuery = null;
        public PlatformQuery PlatformQuery
        {
            get
            {
                if (_platformQuery == null)
                    _platformQuery = new PlatformQuery() { OperationUserId = ConfigurationManager.AppSettings["OperationUserId"] ?? "111111" };
                return _platformQuery;
            }
        }

        protected FortuneLabUserIdentity<int> UserIdentity
        {
            get
            {
                return Thread.CurrentPrincipal.Identity as FortuneLabUserIdentity<int>;
            }
        }
    }
}
