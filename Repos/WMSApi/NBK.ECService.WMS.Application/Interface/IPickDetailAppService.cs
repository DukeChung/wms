using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPickDetailAppService : IApplicationService
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        /// <summary>
        /// 获取待拣货出库
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        /// <summary>
        /// 根据贱货规则生成拣货单数据
        /// </summary>
        /// <param name="createPickDetailRuleDto"></param>
        /// <returns></returns>
         List<string> GeneratePickDetailByPickRule(CreatePickDetailRuleDto createPickDetailRuleDto);

        /// <summary>
        /// 取消拣货
        /// </summary>
        /// <param name="pickSysIdList"></param>
        /// <returns></returns>
        string CancelPickDetail(CancelPickDetailDto cancelPickDetailDto);

        /// <summary>
        /// 取消拣货数量
        /// </summary>
        /// <param name="cancelPickQtyDto"></param>
        void CancelPickQty(CancelPickQtyDto cancelPickQtyDto);

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        void SavePickingOperation(PickingOperationDto pickingOperationDto);

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        List<PickingOperationDetail> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery);
    }
}