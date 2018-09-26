using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum AssemblyStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 加工中
        /// </summary>
        [Description("加工中")]
        Assembling = 20,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finished = 30,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Voided = -999
    }
}
