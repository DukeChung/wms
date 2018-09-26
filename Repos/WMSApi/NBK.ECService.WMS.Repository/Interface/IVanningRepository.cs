using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IVanningRepository : ICrudRepository
    {
        List<VanningDeliveryDto> GetVanningPickDetailDtoByVanningSysId(Guid sysId, Guid wareHouseSysId);

        Pages<VanningDto> GetVanningList(VanningQueryDto vanningQueryDto);

        Pages<HandoverGroupDto> GetHandoverGroupByPage(HandoverGroupQuery request);

        HandoverGroupDto GetHandoverGroupByOrder(string HandoverGroupOrder);

        List<HandoverGroupDetailDto> GetHandoverGroupDetailByOrder(string HandoverGroupOrder);

        /// <summary>
        /// 装箱历史记录
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        List<VanningRecordDto> GetVanningRecordByOrder(string orderNumber, Guid wareHouseSysId, pickdetail pickdetail);

        Pages<VanningDetailViewDto> GetVanningDetailViewListByPaging(VanningViewQuery vanningViewQuery);

        List<VanningPickDetailDto> GetVanningPickDetailByOrder(string orderNumber, pickdetail pickdetail);


        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="model"></param>
        /// <param name="CurrentUserId"></param>
        /// <param name="CurrentDisplayName"></param>
        /// <returns></returns>
        CommonResponse CancelVanning(vanning model, int CurrentUserId, string CurrentDisplayName);

        /// <summary>
        /// 取消装箱明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        CommonResponse CancelVanningDetail(vanning model, int CurrentUserId, string CurrentDisplayName);
    }
}