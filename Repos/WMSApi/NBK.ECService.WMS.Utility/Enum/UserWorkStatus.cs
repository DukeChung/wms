using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum UserWorkStatus
    {
        /// <summary>
        /// 工作中
        /// </summary>
        [Description("工作中")]
        Working = 10,

        /// <summary>
        /// 空闲
        /// </summary>
        [Description("空闲")]
        Free = 20
    }
}
