namespace NBK.ECService.WMS.Utility.Enum
{
    public enum ReceiptDetailStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        Init =0,
        /// <summary>
        /// 新增
        /// </summary>
        New = 10,
        /// <summary>
        /// 收货中
        /// </summary>
        Receiving=30,
        /// <summary>
        /// 收货完成
        /// </summary>
        Received=40,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel = -999
 
    }
}