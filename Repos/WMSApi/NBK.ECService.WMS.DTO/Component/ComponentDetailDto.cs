using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ComponentDetailDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 损耗数量
        /// </summary>
        public int LossQty { get; set; }

        /// <summary>
        /// 是否主要子件
        /// </summary>
        public bool? IsMain { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UOMCode { get; set; }
    }
}
