using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum PickDetailStatus
    {
        /// <summary>
        /// 缺货
        /// </summary>
        [Description("缺货")]
        OutOfStock = -1,

        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 部分拣货
        /// </summary>
        [Description("部分拣货")]
        PartPick = 20,

        /// <summary>
        /// 拣货完成
        /// </summary>
        [Description("拣货完成")]
        Finish = 50,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = -999
    }
}
