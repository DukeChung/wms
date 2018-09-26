using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class FrozenSkuReportDto : BaseDto
    {
        public Guid SkuSysId { get; set; }
        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string LotAttr01 { get; set; }

        public int FrozenQty { get; set; }

        public decimal DisplayFrozenQty { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }
    }
}
