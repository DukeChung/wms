using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IVanningAppService : IApplicationService
    {
        /// <summary>
        /// 获取装箱操作相关数据
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        VanningOperationDto GetVanningOperationDtoByOrder(string orderNumber, Guid wareHouseSysId);

        Pages<VanningDto> GetVanningList(VanningQueryDto vanningQueryDto);


        VanningDetailDto SaveVanningDetailOperationDto(List<VanningDetailOperationDto> vanningDetailOperationDto, string actionType, string currentUserName, int currentUserId, Guid wareHouseSysId);

        Pages<HandoverGroupDto> GetHandoverGroupByPage(HandoverGroupQuery request);

        HandoverGroupDto GetHandoverGroupByOrder(string HandoverGroupOrder, Guid wareHouseSysId);

        VanningViewDto GetVanningViewById(VanningViewQuery vanningViewQuery);

        /// <summary>
        /// 根据装箱明细SysId获取装箱SysId
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        Guid? GetVanningSysIdByVanningDetailSysId(Guid vanningDetailSysId, Guid wareHouseSysId);

        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="vanningCancelDto"></param>
        /// <returns></returns>
        CommonResponse CancelVanning(VanningCancelDto vanningCancelDto);
    }
}