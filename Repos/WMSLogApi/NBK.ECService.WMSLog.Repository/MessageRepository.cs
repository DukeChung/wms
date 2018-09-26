using Abp.EntityFramework;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository
{
    public class MessageRepository : LogCrudRepository, IMessageRepository
    {
        public MessageRepository(IDbContextProvider<NBK_WMS_LogContext> dbContextProvider) : base(dbContextProvider) { }

        public Pages<MessageDto> GetMessageListByUser(MessageQuery request)
        {
            var query = from message in Context.message
                        select new { message };

            if (!request.ReceiveUserID.HasValue)
            {
                query = query.Where(p => p.message.receive_user_id == request.ReceiveUserID.Value);
            }

            var response = query.Select(p => new MessageDto()
            {
                SysId = p.message.SysId,
                receive_date = p.message.receive_date,
                message_type = p.message.message_type,
                content = p.message.content,
                groups = p.message.groups,
                create_date = p.message.create_date,
                receive_user_id = p.message.receive_user_id,
                receive_user_name = p.message.receive_user_name,
                create_user_id = p.message.create_user_id,
                create_user_name = p.message.create_user_name
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderBy(p => p.create_date).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }
    }
}
