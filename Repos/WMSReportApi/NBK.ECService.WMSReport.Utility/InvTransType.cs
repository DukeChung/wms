namespace NBK.ECService.WMSReport.Utility
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
    }
}