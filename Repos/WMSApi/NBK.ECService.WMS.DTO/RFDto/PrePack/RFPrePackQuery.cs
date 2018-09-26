using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFPrePackQuery : BaseQuery
    {
        /// <summary>
        /// 预包装库位
        /// </summary>
        public string StorageLoc { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        public Guid? SkuSysId { get; set; }
    }
}
