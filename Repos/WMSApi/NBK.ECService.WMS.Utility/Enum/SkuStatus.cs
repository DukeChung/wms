using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum SkuStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 10,

        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Frozen = 20
    }
}
