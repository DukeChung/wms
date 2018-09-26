using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface IMessageRepository : ILogCrudRepository
    {
        Pages<MessageDto> GetMessageListByUser(MessageQuery request);
    }
}
