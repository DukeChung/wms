namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class PurchaseAndOutboundChartDto
    {
        public string ChartDate { get; set; }

        /// <summary>
        /// 采购新增数量
        /// </summary>
        public int? PurchaseNewCount { get; set; }
        /// <summary>
        /// 采购收货中数量
        /// </summary>
        public int? PurchaseInReceiptCount { get; set; }

        /// <summary>
        /// 采购收货完成数量
        /// </summary>
        public int? PurchaseFinishCount { get; set; }


        /// <summary>
        /// 采购新增数量
        /// </summary>
        public int? OutboundNewCount { get; set; }
        /// <summary>
        /// 采购收货中数量
        /// </summary>
        public int? OutboundPickCount { get; set; }

        /// <summary>
        /// 采购收货完成数量
        /// </summary>
        public int? OutboundDeliveryCount { get; set; }

    }

    public class PurchaseChartData
    {
        /// <summary>
        /// 采购新增数量
        /// </summary>
        public int? PurchaseNewCount { get; set; }
        /// <summary>
        /// 采购收货中数量
        /// </summary>
        public int? PurchaseInReceiptCount { get; set; }

        /// <summary>
        /// 采购收货完成数量
        /// </summary>
        public int? PurchaseFinishCount { get; set; }

    }
    public class OutboundChartData
    {
        /// <summary>
        /// 采购新增数量
        /// </summary>
        public int? OutboundNewCount { get; set; }
        /// <summary>
        /// 采购收货中数量
        /// </summary>
        public int? OutboundPickCount { get; set; }

        /// <summary>
        /// 采购收货完成数量
        /// </summary>
        public int? OutboundDeliveryCount { get; set; }
    }
}