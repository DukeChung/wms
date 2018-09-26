using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFAssemblyPickDetailDto : BaseDto
    {
        /// <summary>
        /// 加工单号
        /// </summary>
        public string AssemblyOrder { get; set; }

        public Guid? SkuSysId { get; set; }

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

        public decimal DisplayQty { get; set; }
    }
}
