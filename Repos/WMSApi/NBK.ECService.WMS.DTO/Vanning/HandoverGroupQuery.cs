using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class HandoverGroupQuery : BaseQuery
    {
        public string HandoverGroupOrder { get; set; }

        public DateTime? HandoverCreateDateFrom { get; set; }

        public DateTime? HandoverCreateDateTo { get; set; }

        public string ExternOrderId { get; set; }

        public string VanningOrder { get; set; }

    }
}
