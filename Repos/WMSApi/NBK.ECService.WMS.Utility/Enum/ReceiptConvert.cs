using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum ReceiptConvert
    {

        /// <summary>
        /// 成品转原材料
        /// </summary>
        [Description("成品转原材料")]
        ToMaterial = 1,

        /// <summary>
        /// 原材料转商品
        /// </summary>
        [Description("原材料转商品")]
        ToProduct = 2
    }
}
