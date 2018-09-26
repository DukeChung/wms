using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintVanningDetailStickSkuDto
    {
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UOMCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Qty { get; set; }
    }
}
