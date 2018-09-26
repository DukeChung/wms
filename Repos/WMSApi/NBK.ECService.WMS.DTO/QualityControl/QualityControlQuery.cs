using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class QualityControlQuery : BaseQuery
    {
        public string QCOrderSearch { get; set; }

        public int? StatusSearch { get; set; }

        public int? QCTypeSearch { get; set; }

        public string DocOrderSearch { get; set; }

        public string ExternOrderIdSearch { get; set; }
    }
}
