using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum OutboundDetailStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 部分分配
        /// </summary>
        [Description("部分分配")]
        PartAllocation = 15,

        /// <summary>
        /// 分配完成
        /// </summary>
        [Description("分配完成")]
        Allocation = 20,

        /// <summary>
        /// 拣货完成
        /// </summary>
        [Description("拣货完成")]
        Picking = 30,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Delivery = 50
    }
}
