using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundExceptionQueryDto : BaseQuery
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OutboundSysId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SkuName { get; set; }

    }
}
