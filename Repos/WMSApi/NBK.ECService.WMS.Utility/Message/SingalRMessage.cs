using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Message
{
    public class SingalRMessage
    {
        public static void SendMessageToUser(ClientInfo clientInfo)
        {
            var connection = new HubConnection(PublicConst.SingalRMessageWMSHostName);
            IHubProxy myHub = connection.CreateHubProxy("ChatHub");//其名稱必須與 Server Hub 類別名稱一樣

            //建立連線，連線建立完成後向 Server Hub 註冊使用者稱謂
            connection.Start().ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    //連線成功時呼叫 Server 端 Register 方法
                    myHub.Invoke("SendMessage", clientInfo);//必須與 MyHub 的 Register 方法名稱一樣
                }
                else
                {
                    throw new Exception("連線失敗!");
                }
            }).Wait();
            connection.Stop();
        }
    }
}
