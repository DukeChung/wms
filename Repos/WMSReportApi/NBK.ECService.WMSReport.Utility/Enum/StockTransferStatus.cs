using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum StockTransferStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 已转移
        /// </summary>
        [Description("已转移")]
        Transfer = 20,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = -20
    }
}
