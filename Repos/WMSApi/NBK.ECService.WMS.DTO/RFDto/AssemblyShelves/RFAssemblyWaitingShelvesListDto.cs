using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFAssemblyWaitingShelvesListDto
    {
        /// <summary>
        /// 加工单主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 加工单号
        /// </summary>
        public string AssemblyOrder { get; set; }

        /// <summary>
        /// Sku数
        /// </summary>
        public int SkuNumber { get; set; }

        /// <summary>
        /// 待上架商品数量
        /// </summary>
        public int? SkuQty { get; set; }
    }
}
