using Abp.Securities;
using FortuneLab.ECService.Securities;
using FortuneLab.Runtime.Session;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using System;
using System.Configuration;
using System.Threading;

namespace FortuneLab.ECService.Application.Services
{
    /// <summary>
    /// 所有Application Service基类
    /// </summary>
    public class ApplicationService : Abp.Application.Services.ApplicationService
    {
        public ApplicationService()
        {

        }

        private SessionQuery _sessionQuery = null;
        protected SessionQuery SessionQuery
        {
            get
            {
                if (_sessionQuery == null)
                    _sessionQuery = new SessionQuery() { SessionKey = UserIdentity.SessionKey };
                return _sessionQuery;
            }
        }

        private PlatformQuery _platformQuery = null;
        protected PlatformQuery PlatformQuery
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

        public IFortuneLabSession FortuneLabSession { get; set; }

        //[Obsolete("请使用FunctionAuthorize替换,强制过期时间:2016-04-01")]
        //protected bool Authorize(string permissionKey)
        //{
        //    return AuthorizationService.Instance.FunctionAuthorize(permissionKey);
        //}

        /// <summary>
        /// 根据Role授权
        /// </summary>
        /// <param name="roleName">SystemRole中的RoleName</param>
        /// <returns></returns>
        protected bool RoleAuthorize(string roleName)
        {
            return Securities.AuthorizationService.Instance.RoleAuthorize(roleName);
        }

        /// <summary>
        /// 根据Function授权
        /// </summary>
        /// <param name="authKey">SystemFunction中的AuthKey</param>
        /// <returns></returns>
        protected bool FunctionAuthorize(string authKey)
        {
            return Securities.AuthorizationService.Instance.FunctionAuthorize(authKey);
        }

        #region 用户实时消息
        /// <summary>
        /// 发送普通用户消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="contentType"></param>
        public void SendUserSiteMessage(int userId, string content, string title = null, UserMessageContentType contentType = UserMessageContentType.PlainText, Action<ApiResponse<Guid>> callBack = null)
        {
            SessionQuery.ParmsObj = new { userMessageType = (int)UserMessageTypeEnum.Normal, handlerName = "normalMessageHandler", userId };
            var rsp = WebApiClient.ApiClient.Post<Guid>(ConfigurationManager.AppSettings[FxConstConfiguration.FxApiUrlConfigName], "users/userMessage/add", SessionQuery, new { content, title, contentType });
            if (callBack != null)
            {
                callBack(rsp);
            }
        }

        /// <summary>
        /// 发送页面级别Block消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        public void SendPageBlockNotifyMessage(int userId, object data, Action<ApiResponse<Guid>> callBack = null)
        {
            SessionQuery.ParmsObj = new { userMessageType = (int)UserMessageTypeEnum.PageBlock, handlerName = "pageBlockMessage", userId };
            var rsp = WebApiClient.ApiClient.Post<Guid>(ConfigurationManager.AppSettings[FxConstConfiguration.FxApiUrlConfigName], "users/userMessage/blockMessage/add", SessionQuery, data);
            if (callBack != null)
            {
                callBack(rsp);
            }
        }

        /// <summary>
        /// 发送全局级别Block消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        public void SendGlobalBlockNotifyMessage(int userId, object data, Action<ApiResponse<Guid>> callBack = null)
        {
            SessionQuery.ParmsObj = new { userMessageType = (int)UserMessageTypeEnum.GlobalBlock, handlerName = "globalBlockMessage", userId };
            var rsp = WebApiClient.ApiClient.Post<Guid>(ConfigurationManager.AppSettings[FxConstConfiguration.FxApiUrlConfigName], "users/userMessage/blockMessage/add", SessionQuery, data);
            if (callBack != null)
            {
                callBack(rsp);
            }
        }
        #endregion
    }

    /// <summary>
    /// 用户消息类型(不同类型具有不同的通知方式)
    /// </summary>
    public enum UserMessageTypeEnum : int
    {
        /// <summary>
        /// 常规站内消息, 会发送到用户的消息列表
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 全局阻断式消息，会持久化到用户消息列表，如用户在线，会及时通知，不在线的时候，会在下次登录系统时通知
        /// </summary>
        GlobalBlock = 2,

        /// <summary>
        /// 业务关联式阻断消息, 用户在打开某个业务页面时会通知用户，做某项消息处理，但是不会持久化，触发是在每次触发业务条件后实时发送
        /// </summary>
        PageBlock = 4

        /*
消息类型说明:
* 1. 全局页面block, 进入任何系统都会检测处理, 检测到之后做处理
* 2. 某个业务页面block, 由对应的业务页面处理，进入页面时做检测，并提供判断业务逻辑，发送消息给当前的客户端.
* 3. 非block消息, 记入Message表，在个人的消息列表展示
*/
    }

    public enum UserMessageContentType : int
    {
        PlainText = 1,
        Html = 2
    }
}
