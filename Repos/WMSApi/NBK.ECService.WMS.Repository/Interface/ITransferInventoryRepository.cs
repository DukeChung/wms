using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface ITransferInventoryRepository: ICrudRepository
    {
        /// <summary>
        /// 分页获取移仓单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<TransferinventoryListDto> GetTransferinventoryByPage(TransferinventoryQuery request);

        /// <summary>
        /// 根据Id获取移仓单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        TransferInventoryViewDto GetTransferinventoryBySysId(Guid sysId);

        /// <summary>
        /// 根据移仓单获取移仓单明细列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        List<TransferInventoryDetailDto> GetTransferInventoryDetail(Guid sysId);

        void BatchInsertTransferinventoryReceiptExtend(List<MQTransferinventoryReceiptExtendDto> list);
    }
}
