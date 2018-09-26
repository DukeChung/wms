using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NBK.ECService.WMS.Utility.Enum
{
    public enum OutboundTransferOrderStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        PrePack = 20,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish = 30,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = -999

    }
}
