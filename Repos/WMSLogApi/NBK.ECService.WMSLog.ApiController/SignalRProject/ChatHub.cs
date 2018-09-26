using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.ApiController
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 紀錄目前已連結的 Client 識別資料
        /// </summary>
        public static Dictionary<string, ClientInfo> CurrClients = new Dictionary<string, ClientInfo>();

        /// <summary>
        /// 给指定登陆用户 发消息（根据userid）
        /// </summary>
        /// <param name="request"></param>
        public void SendMessageToUser(ClientInfo request)
        {
            //Clients.All.NotifyMessage(message);

            var connectionId = Context.ConnectionId;
            var notifyUserList = CurrClients.Where(p => p.Value.UserId == request.UserId);
            if (notifyUserList != null && notifyUserList.Count() > 0)
            {
                foreach (var notifyUser in notifyUserList.ToList())
                {
                    Clients.Client(notifyUser.Value.ConnId).NotifyMessage(request);
                }
            }
        }

        /// <summary>
        /// 全员推送 消息
        /// </summary>
        /// <param name="request"></param>
        public void SendMessageToAll(ClientInfo request)
        {
            Clients.All.NotifyMessage(request);
        }

        /// <summary>
        /// 提供 Client 端呼叫
        /// 功能:對 Server 進行身分註冊
        /// </summary>
        /// <param name="clientName">使用者稱謂</param>
        public void Register(int userId)
        {
            string connId = Context.ConnectionId;
            lock (CurrClients)
            {
                if (!CurrClients.ContainsKey(connId))
                {
                    CurrClients.Add(connId, new ClientInfo { ConnId = connId, UserId = userId });
                }
            }
            Clients.All.NowUser(CurrClients);
        }

        /// <summary>
        /// Client 端離線時的動作
        /// </summary>
        /// <param name="stopCalled">true:為使用者正常關閉(離線); false: 使用者不正常關閉(離線)，如連線狀態逾時</param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string connId = Context.ConnectionId;
            lock (CurrClients)
            {
                if (CurrClients.ContainsKey(connId))
                {
                    CurrClients.Remove(connId);
                }
            }
            Clients.All.NowUser(CurrClients);//呼叫 Client 所提供 NowUser 方法(ReceiveMsg 方法由Client 端實作)

            stopCalled = true;
            return base.OnDisconnected(stopCalled);
        }
    }

    /// <summary>
    /// 保存Client識別資料的物件
    /// </summary>
    public class ClientInfo
    {
        public string ConnId { get; set; }
        
        public int UserId { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public string WarehouseSysId { get; set; }
    }
}
