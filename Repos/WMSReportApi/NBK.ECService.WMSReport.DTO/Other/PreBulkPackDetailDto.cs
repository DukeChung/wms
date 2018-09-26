using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PreBulkPackDetailDto
    {
        public Guid SysId { get; set; }

        public Guid PreBulkPackSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int Qty { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string UomCode { get; set; }

        public string PackCode { get; set; }

        /// <summary>
        /// 导入计划数量
        /// </summary>
        public int? PreQty { get; set; }
        /// <summary>
        /// 导入商品外部Id
        /// </summary>
        public string OtherId { get; set; }

        public string SkuDescr { get; set; }
    }
}
