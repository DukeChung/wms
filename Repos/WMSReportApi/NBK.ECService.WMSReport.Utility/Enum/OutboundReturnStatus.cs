using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum OutboundReturnStatus
    {
        /// <summary>
        /// 全部退货
        /// </summary>
        [Description("全部退货")]
        AllReturn = 10,

        /// <summary>
        /// 部分退货
        /// </summary>
        [Description("部分退货")]
        PartReturn = 20,

        /// <summary>
        /// B2C退货
        /// </summary>
        [Description("B2C退货")]
        B2CReturn = 30
    }
}
