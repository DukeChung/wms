using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFAssemblyScanShelvesDto : BaseDto
    {
        /// <summary>
        /// 加工单号
        /// </summary>
        public string AssemblyOrder { get; set; }

        public Guid? SkuSysId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// UI输入上架数量
        /// </summary>
        public decimal InputQty { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int UserId { get; set; }
    }
}
