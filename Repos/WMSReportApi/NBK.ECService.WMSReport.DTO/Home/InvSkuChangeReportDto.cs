using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuChangeReportDto
    {
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品UPC
        /// </summary>
        public string SkuUPC { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 收货量
        /// </summary>
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int InvQty { get; set; }

        public decimal DisplayInvQty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public decimal Gains { get; set; }

        /// <summary>
        /// 涨跌
        /// </summary>
        public decimal CPrice { get; set; }

        /// <summary>
        /// 历史最低价
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 历史最高价
        /// </summary>
        public decimal MaxPrice { get; set; }
    }
}
