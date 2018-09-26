using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum FrozenSource
    {
        /// <summary>
        /// 冻结业务
        /// </summary>
        [Description("冻结业务")]
        FrozenBiz = 10,

        /// <summary>
        /// 盘点
        /// </summary>
        [Description("盘点")]
        StockTake = 20,
    }
}
