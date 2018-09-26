using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartTransferInventoryDto
    {
        /// <summary>
        /// 源仓库编号
        /// </summary>
        public string FromWarehouseId { get; set; }

        /// <summary>
        /// 目标仓库编号
        /// </summary>
        public string ToWarehouseId { get; set; }

        /// <summary>
        /// 外部下单时间 (ECC下单时间)
        /// </summary>
        public DateTime? ExternOrderDate { get; set; }

        /// <summary>
        /// 外部单据号（ECC移仓单号）
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// ECC 审核人ID
        /// </summary>
        public string AuditingBy { get; set; }
        /// <summary>
        /// ECC 审核人名称
        /// </summary>
        public string AuditingName { get; set; }

        /// <summary>
        /// ECC 审核时间
        /// </summary>
        public DateTime? AuditingDate { get; set; }

        /// <summary>
        /// 预留:运输方式
        /// </summary>
        public string ShippingMethod { get; set; }
        /// <summary>
        /// 预留:运费
        /// </summary>
        public decimal? Freight { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get; set; }


        public List<ThirdPartTransferInventoryDetailDto> ThirdPartTransferInventoryDetailList { get; set; }
    }
}
