using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundReviewDetailDto  
    {
        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string StorageCase { get; set; }

        public int? OutboundQty { get; set; }

        public int PickQty { get; set; }

        public int ReviewQty { get; set; }

        /// <summary>
        /// 散货待复核数量
        /// </summary>
        public int SingleQty { get; set; }

        /// <summary>
        /// 整件待复核数量
        /// </summary>
        public int WholeQty { get; set; }
    }
}
