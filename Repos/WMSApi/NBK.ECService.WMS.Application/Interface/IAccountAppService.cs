using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IAccountAppService
    {
        CommonResponse UserLoginCheck(LoginUserInfo user);

        void UserLoginSuccess(LoginUserInfo user);

        void UserLoginFail(LoginUserInfo user);
    }
}
