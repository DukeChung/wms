using Abp.WebApi.Controllers;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace NBK.ECService.WMSLog.ApiController
{
    [RoutePrefix("api/Message")]
    public class MessageController : AbpApiController
    {
        private IMessageAppService _messageAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageAppService"></param>
        public MessageController(IMessageAppService messageAppService)
        {
            _messageAppService = messageAppService;
        }

        /// <summary>
        /// 获取用户聊天记录
        /// </summary>
        [HttpGet]
        public void MessageApi() { }

        #region websocket

        /// <summary>
        /// 获取指定用户的系统消息,websocket
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSystemMessageListByUser")]
        public HttpResponseMessage GetSystemMessageListByUser()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessWSChat);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task ProcessWSChat(AspNetWebSocketContext arg)
        {
            WebSocket socket = arg.WebSocket;
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    var request = JsonConvert.DeserializeObject<MessageQuery>(message);
                    var response = _messageAppService.GetSystemMessageListByUser(request.ReceiveUserID.Value, request.Status.Value);

                    string returnMessage = JsonConvert.SerializeObject(response);//"You send :" + message + ". at" + DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(returnMessage));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }

        #endregion websocket

        /// <summary>
        /// 获取指定用户的系统消息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet, Route("TestGetSystemMessageListByUser")]
        public List<MessageDto> TestGetSystemMessageListByUser(int userID, int status)
        {
            return _messageAppService.GetSystemMessageListByUser(userID, status);
        }

        /// <summary>
        /// 分页查询条件消息列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMessageListByUser")]
        public Pages<MessageDto> GetMessageListByPage(MessageQuery request)
        {
            return _messageAppService.GetMessageListByUser(request);
        }

        /// <summary>
        /// 推送WMS系统消息
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("SendSystemMessage")]
        public void SendSystemMessage(MessageDto request)
        {
            _messageAppService.SendSystemMessage(request);
        }
    }
}
