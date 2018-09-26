using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;
namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IBaseRepository : ICrudRepository
    {
        List<MenuDto> GetSystemMenuList();
    }
}
