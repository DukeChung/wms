using System.ComponentModel;    

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum SkuBorrowStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 已借出
        /// </summary>
        [Description("已借出")]
        Audit = 20,

        /// <summary>
        /// 已归还
        /// </summary>
        [Description("已归还")]
        ReturnAudit = 30,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void = -999
    }
}
