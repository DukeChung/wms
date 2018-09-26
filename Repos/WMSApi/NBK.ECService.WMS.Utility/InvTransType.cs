namespace NBK.ECService.WMS.Utility
{
    public static class InvTransType
    {
        /// <summary>
        /// 入库
        /// </summary>
        public const string Inbound = "IN";

        /// <summary>
        /// 出库
        /// </summary>
        public const string Outbound = "OUT";

        /// <summary>
        /// 调整
        /// </summary>
        public const string Adjustment = "AJ";

        /// <summary>
        /// 批次转移
        /// </summary>
        public const string StockTransfer = "ST";

        /// <summary>
        /// 加工
        /// </summary>
        public const string Assembly = "ASSY";

        /// <summary>
        /// 盘点
        /// </summary>
        public const string StockTake = "STAKE";
    }

    public static class InvSourceTransType
    {
        /// <summary>
        /// 收货
        /// </summary>
        public const string Receipt = "Receipt";

        /// <summary>
        /// 上架
        /// </summary>
        public const string Shelve = "Shelve";

        /// <summary>
        /// 成品上架
        /// </summary>
        public const string AssemblyShelve = "AssemblyShelve";

        /// <summary>
        /// 拣货
        /// </summary>
        public const string Picking = "Picking";

        /// <summary>
        /// 加工拣货
        /// </summary>
        public const string AssemblyPicking = "AssemblyPicking";

        /// <summary>
        /// 快速发货
        /// </summary>
        public const string QuickDelivery = "QuickDelivery";

        /// <summary>
        /// 分配发货
        /// </summary>
        public const string AllocationDelivery = "AD";

        /// <summary>
        /// 发货（正常流程）
        /// </summary>
        public const string Shipment = "Shipment";

        /// <summary>
        /// 损益
        /// </summary>
        public const string Losses = "Losses";

        /// <summary>
        /// 借出
        /// </summary>
        public const string LendOut = "LendOut";
        
        /// <summary>
        /// 移动
        /// </summary>
        public const string Movement = "Movement";
        /// <summary>
        /// 批次转移
        /// </summary>
        public const string StockTransfer = "StockTransfer";

        /// <summary>
        /// 复盘
        /// </summary>
        public const string ReplayStockTake = "ReplayStockTake";

    }

    public static class InvTransStatus
    {
        /// <summary>
        /// 确定
        /// </summary>
        public const string Ok = "OK";

        /// <summary>
        /// 取消
        /// </summary>
        public const string Cancel = "CANCEL";

 
    }
}