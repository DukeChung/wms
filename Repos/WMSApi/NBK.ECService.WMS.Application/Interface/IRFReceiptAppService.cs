using Abp.Application.Services;
using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    /// <summary>
    /// RF收货接口
    /// </summary>
    public interface IRFReceiptAppService : IApplicationService
    {
        /// <summary>
        /// 待收货列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        Page<RFReceiptListDto> GetWaitingReceiptListByPaging(RFReceiptQuery receiptQuery);

        /// <summary>
        /// 检查收货单是否能收货
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        RFCommResult CheckReceipt(RFReceiptQuery receiptQuery);

        /// <summary>
        /// 获取入库单收货明细
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        List<RFReceiptOperationDetailListDto> GetReceiptOperationDetailList(RFReceiptQuery receiptQuery);

        /// <summary>
        /// 收货完成
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        RFCommResult SaveReceiptOperation(ReceiptOperationDto receiptOperationDto);
    }
}
