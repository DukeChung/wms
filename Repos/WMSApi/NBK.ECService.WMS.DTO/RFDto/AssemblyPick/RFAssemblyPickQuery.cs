using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFAssemblyPickQuery : BaseQuery
    {
        /// <summary>
        /// 加工单号
        /// </summary>
        public string AssemblyOrder { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }
    }
}
