using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IOutboundTransferOrderAppService : IApplicationService
    {
        /// <summary>
        /// 分页获取交接数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<OutboundTransferOrderDto> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request);

        /// <summary>
        /// 获取交接明细数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        OutboundTransferOrderDto GetDataBySysId(Guid sysId, Guid warehouseSysId);

        void UpdateOutboundTransferOrder(OutboundTransferOrderQueryDto dto);

        void DeleteOutboundTransferOrder(OutboundTransferOrderQueryDto dto);

        List<OutboundTransferPrintDto> GetOutboundTransferBox(List<Guid> request, Guid warehouseSysId);

        List<OutboundTransferPrintDto> GetOutboundTransferOrder(OutboundTransferOrderQuery dto);

        CommonResponse UpdateTransferOrderSku(OutboundTransferOrderMoveDto dto);
    }
}
