using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenListQuery : BaseQuery
    {
        public int? Type { get; set; }

        public int? Status { get; set; }

        public Guid? ZoneSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }
    }
}
