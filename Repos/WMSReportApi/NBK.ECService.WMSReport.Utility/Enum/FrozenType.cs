using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum FrozenType
    {
        /// <summary>
        /// 储区
        /// </summary>
        [Description("储区")]
        Zone = 10,

        /// <summary>
        /// 货位
        /// </summary>
        [Description("货位")]
        Location = 20,

        /// <summary>
        /// 商品
        /// </summary>
        [Description("商品")]
        Sku = 30,

        /// <summary>
        /// 货位商品
        /// </summary>
        [Description("货位商品")]
        LocSku = 40
    }
}
