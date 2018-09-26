using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ILocationAppService : IApplicationService
    {
        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <returns></returns>
        Pages<LocationListDto> GetLocationList(LocationQuery locationQuery);

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        Guid AddLocation(LocationDto locationDto);

        /// <summary>
        /// 根据Id获取库位
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        LocationDto GetLocationById(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 编辑库位
        /// </summary>
        /// <param name="locationDto"></param>
        void EditLocation(LocationDto locationDto);

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteLocation(List<Guid> sysIds, Guid warehouseSysId);

        /// <summary>
        /// 根据货位查询货位数据
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        LocationDto GetLocationByLoc(string loc, Guid? warehouseSysId);

        /// <summary>
        /// 获取库位下拉框
        /// </summary>
        /// <returns></returns>
        List<SelectItem> GetSelectLocation(Guid wareHouseSysId, Guid? zoneSysId = null);
    }
}
