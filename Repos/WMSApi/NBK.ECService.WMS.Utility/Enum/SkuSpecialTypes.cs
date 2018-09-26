using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum SkuSpecialTypes
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,

        /// <summary>
        /// SN管控商品
        /// </summary>
        [Description("SN管控商品")]
        RedCard = 2,

        /// <summary>
        /// 农资
        /// </summary>
        [Description("农资")]
        NZ = 3
    }
}
