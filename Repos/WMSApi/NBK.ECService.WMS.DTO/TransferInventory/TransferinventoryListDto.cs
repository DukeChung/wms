using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class TransferinventoryListDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid SysId { get; set; }
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string StatusName
        {
            get { return ((TransferInventoryStatus)Status).ToDescription(); }
        }
        /// <summary>
        /// 来源仓
        /// </summary>
        public string FromWareHouseName { get; set; }
        /// <summary>
        /// 目标仓
        /// </summary>
        public string ToWareHouseName { get; set; }
        /// <summary>
        /// 外部单据号
        /// </summary>
        public string ExternOrderId { get; set; }
        /// <summary>
        /// 移仓出库时间
        /// </summary>
        public DateTime? TransferOutboundDate { get; set; }
        public string TransferOutboundDateDisplay
        {
            get
            {
                if (TransferOutboundDate.HasValue)
                {
                    return TransferOutboundDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 移仓入库时间
        /// </summary>
        public DateTime? TransferInboundDate { get; set; }
        public string TransferInboundDateDisplay
        {
            get
            {
                if (TransferInboundDate.HasValue)
                {
                    return TransferInboundDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 移仓出库单
        /// </summary>
        public string TransferOutboundOrder { get; set; }
        /// <summary>
        /// 移仓入库单
        /// </summary>
        public string TransferPurchaseOrder { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
