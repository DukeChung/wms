using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFReceiptRepository : ICrudRepository
    {
        /// <summary>
        /// 待收货列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        Page<RFReceiptListDto> GetWaitingReceiptListByPaging(RFReceiptQuery receiptQuery);

        /// <summary>
        /// 获取入库单收货明细
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        List<RFReceiptOperationDetailListDto> GetReceiptOperationDetailList(RFReceiptQuery receiptQuery);

        /// <summary>
        /// 收货完成获取入库单明细
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        List<PurchaseDetailViewDto> GetPurchaseDetailViewDtoList(Guid purchaseSysId);
    }
}
