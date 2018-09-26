using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IAccountAppService
    {
        CommonResponse UserLoginCheck(LoginUserInfo user);

        List<MenuDto> GetSystemMenuList();

        void SynchroMenu();
        //void UserLoginSuccess(LoginUserInfo user);

        //void UserLoginFail(LoginUserInfo user);
    }
}
