using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum TransferInventoryStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 出库完成
        /// </summary>
        [Description("出库完成")]
        Delivery = 20,

        /// <summary>
        /// 部分收货
        /// </summary>
        [Description("部分收货")]
        PartReceipt = 25,

        /// <summary>
        /// 收货完成
        /// </summary>
        [Description("收货完成")]
        ReceiptFinish = 30,


        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = -999,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Close = -10
    }

    public enum TransferInventoryDetailStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,
    }
}
