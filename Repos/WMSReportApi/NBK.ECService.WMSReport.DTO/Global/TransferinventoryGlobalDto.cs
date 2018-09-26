using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class TransferinventoryGlobalDto : BaseDto
    {
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }

        /// <summary>
        /// 来源仓库
        /// </summary>
        public string FromWareHouseName { get; set; }

        /// <summary>
        /// 目标仓库
        /// </summary>
        public string ToWareHouseName { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string TransferPurchaseOrder { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string TransferOutboundOrder { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 入库操作人
        /// </summary>
        public string RUpdateUserName { get; set; }

        /// <summary>
        /// 出库操作人
        /// </summary>
        public string OUpdateUserName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        public string StatusName
        {
            get { return ((TransferInventoryStatus)Status).ToDescription(); }
        }

    }
}
