using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFAssemblyShelvesQuery : BaseQuery
    {
        public string AssemblyOrder { get; set; }

        public string UPC { get; set; }
    }
}
