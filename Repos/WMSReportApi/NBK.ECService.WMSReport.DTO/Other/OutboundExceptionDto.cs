using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class OutboundExceptionDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string UOMCode { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }
        public int SkuSpecialTypes { get; set; }
    }
}
