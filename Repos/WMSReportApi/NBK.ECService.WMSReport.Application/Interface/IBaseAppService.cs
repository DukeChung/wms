using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IBaseAppService : IApplicationService
    {
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        CoordinateDto GetCoordinate(string city, string address);
    }
}
