using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ITransferInventoryAppService : IApplicationService
    {
        /// <summary>
        /// 移仓单创建出库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        CommonResponse AddOutboundByTransferInventory(MQTransferInventoryDto transferInventoryDto);

        /// <summary>
        /// 移仓单创建入库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        CommonResponse CreateTransferInventoryReceipt(MQTransferInventoryDto transferInventoryDto);
        /// <summary>
        /// 分页获取移仓单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<TransferinventoryListDto> GetTransferinventoryByPage(TransferinventoryQuery request);
        /// <summary>
        /// 更新移仓单状态
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        CommonResponse UpdateTransferInventoryStatus(MQTransferInventoryDto transferInventoryDto);

        /// <summary>
        /// 获取移仓单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        TransferInventoryViewDto GetTransferinventoryBySysId(Guid sysId, Guid warehouseSysId);

        bool ObsoleteTransferinventory(TransferinventoryUpdateQuery dto);
    }
}
