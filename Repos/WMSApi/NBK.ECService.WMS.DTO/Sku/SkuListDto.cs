using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuListDto
    {
        public Guid SysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public DateTime CreateDate { get; set; }

        public string UOMCode { get; set; }

        public string RecommendLoc { get; set; }
        /// <summary>
        /// 外部Id
        /// </summary>
        public string OtherId { get; set; }
    }
}
