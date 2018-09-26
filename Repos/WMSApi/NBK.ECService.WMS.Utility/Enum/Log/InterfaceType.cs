using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum.Log
{
    public enum InterfaceType
    {
        /// <summary>
        /// 调用
        /// </summary>
        [Description("调用")]
        Invoke = 10,

        /// <summary>
        /// 被调用
        /// </summary>
        [Description("被调用")]
        Invoked = 20
    }
}
