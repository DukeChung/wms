using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseDetailBatchDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int Qty { get; set; }


    }
}
