using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum OutboundTransferOrderType
    {
        /// <summary>
        /// 拼箱
        /// </summary>
        [Description("拼箱")]
        Scattered = 10,

        /// <summary>
        /// 整件
        /// </summary>
        [Description("整件")]
        Whole = 20
    }
}
