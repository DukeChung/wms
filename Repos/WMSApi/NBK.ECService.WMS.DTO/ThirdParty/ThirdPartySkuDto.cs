using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartySkuDto
    {
        /// <summary>
        /// SysNo, SkuCode
        /// </summary>
        public string OtherId { get; set; }

        /// <summary>
        /// ProductName
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// CategorySysNo 根据本字段去对应的 SkuClass 分类表关联 OtherId 获取 SysClassSysId  
        /// </summary>
        public string SkuClassSysId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// UOMSysId
        /// </summary>
        public string UOMName { get; set; }

        /// <summary>
        /// GuaranteePeriod
        /// </summary>
        public int? DaysToExpire { get; set; }

        /// <summary>
        /// Length
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// CurrentPrice
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// ImageUrl
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 是否开具发票
        /// </summary>
        public bool IsInvoices { get; set; }

        /// <summary>
        /// 是否允许退换
        /// </summary>
        public bool IsRefunds { get; set; }

        /// <summary>
        /// 是否是原材料
        /// </summary>
        public bool? IsMaterial { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 是否新增
        /// </summary>
        public bool InsertFlag { get; set; }

        /// <summary>
        /// SN 管控商品
        /// </summary>
        public bool HasSN { get; set; }
    }
}