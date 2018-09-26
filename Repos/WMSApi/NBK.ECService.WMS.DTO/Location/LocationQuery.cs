using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class LocationQuery : BaseQuery
    {
        public string LocationSearch { get; set; }

        public Guid? ZoneSysIdSearch { get; set; }

        public bool? IsActiveSearch { get; set; }
    }
}
