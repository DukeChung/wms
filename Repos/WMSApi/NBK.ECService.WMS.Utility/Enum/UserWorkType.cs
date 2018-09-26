using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum UserWorkType
    {
        /// <summary>
        /// 收货
        /// </summary>
        [Description("收货")]
        Receipt = 20,

        /// <summary>
        /// 上架
        /// </summary>
        [Description("上架")]
        Shelve = 40,

        /// <summary>
        /// 拣货
        /// </summary>
        [Description("拣货")]
        Picking = 60
    }
}
