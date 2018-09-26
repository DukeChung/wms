using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum ReceiptSNStatus
    {
        /// <summary>
        /// 收货
        /// </summary>
        [Description("收货")]
        Receive = 1,

        /// <summary>
        /// 发货
        /// </summary>
        [Description("发货")]
        Outbound = 2
    }
}
