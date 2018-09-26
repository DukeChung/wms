using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class TransferInventoryDetailDto
    {
        public Guid? SysId { get; set; }
        public Guid? TransferInventorySysId { get; set; }
        public Guid? SkuSysId { get; set; }
        public Guid? UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string SkuUPC { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string StatusName
        {
            get
            {
                return ConverStatus.GetOutboundStatus(Status);
            }
        }
        public string UomCode { get; set; }
        public string PackCode { get; set; }
        public int? Qty { get; set; }

        public decimal? DisplayQty { get; set; }

        public int? ShippedQty { get; set; }

        public decimal? DisplayShippedQty { get; set; }

        public int? ReceivedQty { get; set; }

        public decimal? DisplayReceivedQty { get; set; }

        public int? RejectedQty { get; set; }

        public decimal? DisplayRejectedQty { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }
    }
}
