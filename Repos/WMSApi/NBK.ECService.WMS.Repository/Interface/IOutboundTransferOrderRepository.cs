using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using System;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IOutboundTransferOrderRepository : ICrudRepository
    {
        /// <summary>
        /// 分页获取交接数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<OutboundTransferOrderDto> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request);

        OutboundTransferOrderDto GetDataBySysId(Guid sysId);

        /// <summary>
        /// 根据出库单ID更新所有交接单状态
        /// </summary>
        /// <param name="dto"></param>
        void UpdateOutboundTransferOrder(OutboundTransferOrderQueryDto dto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        void UpdateOutboundTransferOrderFinish(OutboundTransferOrderQueryDto dto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>

        void DeleteOutboundTransferOrder(OutboundTransferOrderQueryDto dto);

        List<OutboundTransferPrintDto> GetOutboundTransferBox(List<Guid> request);

        List<OutboundTransferPrintDto> GetOutboundTransferOrder(OutboundTransferOrderQuery dto);
    }
}
