using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkUserQuery : BaseQuery
    {
        public string WorkUserCodeSearch { get; set; }
        public string WorkUserNameSearch { get; set; }
        public bool? IsActiveSearch { get; set; }
        public int? WorkTypeSearch { get; set; }
        public int? WorkStatusSearch { get; set; }
    }
}
