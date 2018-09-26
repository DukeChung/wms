using Abp.Application.Services;
using NBK.ECService.WMS.DTO.System;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ISystemAppService : IApplicationService
    {
        List<MenuDto> GetSystemMenuList();
    }
}
