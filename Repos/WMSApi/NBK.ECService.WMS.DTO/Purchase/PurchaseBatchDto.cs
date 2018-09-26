using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseBatchDto : BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 供应商Id
        /// </summary>
        public Guid VendorSysId { get; set; }

        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime? PurchaseDate { get; set; }

        public string PurchaseDateText { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public string PoGroup { get; set; }

        /// <summary>
        /// 采购明细
        /// </summary>
        public List<PurchaseDetailBatchDto> PurchaseDetailBatchDto { get; set; }
    }
}
