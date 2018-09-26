using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuQuery : BaseQuery
    {
        public string SkuCodeSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public string SkuDescrSearch { get; set; }

        public string UPC { get; set; }

        public Guid? PackSysIdSearch { get; set; }

        public Guid? LotTemplateSysIdSearch { get; set; }

        public Guid? SkuClassSysIdSearch { get; set; }

        public bool? IsActiveSearch { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPCSearch { get; set; }

        public string OtherIdSearch { get; set; }
    }
}
