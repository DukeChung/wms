using NBK.ECService.WMS.Utility;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.Application.OtherService;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.DTO.Authorize;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using NBK.ECService.WMSLog.Utility;
using NBK.ECService.WMSLog.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application
{
    public class MessageAppService : IMessageAppService
    {
        private IMessageRepository _messageRepository = null;

        public MessageAppService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public Pages<MessageDto> GetMessageListByUser(MessageQuery request)
        {
            return _messageRepository.GetMessageListByUser(request);
        }

        public List<MessageDto> GetSystemMessageListByUser(int userID, int status)
        {
            var response = _messageRepository.GetQuery<message>(p => p.receive_user_id == userID 
                && p.status == status
                && p.message_type == PublicConst.MessageType_SystemNotice
                && p.create_user_id == PublicConst.WMSUserID);

            if (response.Count() > 0)
            {
                foreach (var message in response.ToList())
                {
                    message.status = (int)MessageStatus.Read;
                    _messageRepository.Update(message);
                }                
            }

            return response.JTransformTo<MessageDto>();
        }

        public void SendSystemMessage(MessageDto request)
        {
            List<ApplicationUser> userList = AuthorizeManager.GetAllSystemUser();

            foreach (var user in userList)
            {
                message newMessage = new message() {
                    content = request.content,
                    system_id = (int)SystemEnum.WMS,
                    message_type = PublicConst.MessageType_SystemNotice,
                    create_date = DateTime.Now,
                    create_user_id = PublicConst.WMSUserID,
                    create_user_name = PublicConst.WMSUserName,
                    receive_user_id = user.UserId,
                    receive_date = DateTime.Now,
                    receive_user_name = user.DisplayName,
                    status = (int)MessageStatus.New
                };

                _messageRepository.Insert(newMessage);
            }
        }
    }
}
