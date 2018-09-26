namespace NBK.ECService.WMSReport.DTO
{
    public class ReceivedAndSendSkuGlobalDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 仓库所在区域
        /// </summary>
        public string WareHouseArea { get; set; }
        /// <summary>
        /// 仓库性质
        /// </summary>
        public string WareHouseProperty { get; set; }
        /// <summary>
        /// 收货数量
        /// </summary>
        public int ReceivedQty { get; set; }
        /// <summary>
        ///   收货件数
        /// </summary>
        public int ReceivedPieceQty { get; set; }
        /// <summary>
        /// 发货数量
        /// </summary>
        public int DeliveryQty { get; set; }
        /// <summary>
        /// 发货件数
        /// </summary>
        public int DeliveryPieceQty { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 入库单数量
        /// </summary>
        public int PurchaseCount { get; set; }
        /// <summary>
        /// 出库单数量
        /// </summary>
        public int OutboundCount { get; set; }
    }
}
