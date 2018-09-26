using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortuneLab.ECService.Securities.Entities;
using Abp.Auditing;
using Abp.Securities;
using FortuneLab.Runtime;

namespace FortuneLab.ECService.Securities
{

    public interface IUserAppService : IApplicationService
    {
        ApiToken GetToken(string token);

        SystemUser GetUserByLoginId(string loginId);

        UserDevice GetUserDevice(string sessionKey);

        SystemUser GetUser(int userSysId);

        void UpdateUserDevice(UserDevice userSession);

        UserDevice GetUserDevice(int userDeviceSysId, int deviceType);

        void AddUserDevice(UserDevice existsDevice);

        //Page<SystemUser> GetSystemUserList(string keyword, int pageIndex, int pageSize);

        bool CheckUserAuthorization(string token, string userID, string funAuthKey);

        SystemUser GetUserBySystemSession(string systemSessionKey);
    }
}
