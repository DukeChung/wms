using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum StockMovementStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Confirm = 20,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = -999
    }
}
