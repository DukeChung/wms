using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class TransferinventoryQuery : BaseQuery
    {
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }
        /// <summary>
        /// 来源仓
        /// </summary>
        public Guid FromWareHouseSysId { get; set; }
        /// <summary>
        /// 目标仓
        /// </summary>
        public Guid ToWareHouseSysId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 移仓出库从
        /// </summary>
        public DateTime? TransferOutboundDateFrom { get; set; }
        /// <summary>
        /// 移仓出库到
        /// </summary>
        public DateTime? TransferOutboundDateTo { get; set; }
        /// <summary>
        /// 移仓入库从
        /// </summary>
        public DateTime? TransferInboundDateFrom { get; set; }
        /// <summary>
        /// 移仓入库到
        /// </summary>
        public DateTime? TransferInboundDateTo { get; set; }

        /// <summary>
        /// 移仓出库单
        /// </summary>
        public string TransferOutboundOrder { get; set; }

        /// <summary>
        /// 移仓入库单
        /// </summary>
        public string TransferPurchaseOrder { get; set; }

        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternOrderId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
