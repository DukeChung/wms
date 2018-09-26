using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IZoneAppService : IApplicationService
    {
        /// <summary>
        /// 获取储区列表
        /// </summary>
        /// <param name="zoneQuery"></param>
        /// <returns></returns>
        Pages<ZoneDto> GetZoneList(ZoneQuery zoneQuery);

        /// <summary>
        /// 新增储区
        /// </summary>
        /// <param name="zoneDto"></param>
        /// <returns></returns>
        Guid AddZone(ZoneDto zoneDto);

        /// <summary>
        /// 根据Id获取储区
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        ZoneDto GetZoneById(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 编辑储区
        /// </summary>
        /// <param name="zoneDto"></param>
        void EditZone(ZoneDto zoneDto);

        /// <summary>
        /// 删除储区
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteZone(List<Guid> sysIds, Guid warehouseSysId);

        List<SelectItem> GetSelectZone(Guid? warehouseSysId , string zoneCode);
    }
}
