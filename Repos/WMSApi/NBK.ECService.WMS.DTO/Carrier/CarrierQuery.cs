using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CarrierQuery : BaseQuery
    {
        public string CarrierNameSearch { get; set; }

        public string CarrierPhoneSearch { get; set; }

        public string CarrierContactsSearch { get; set; }

        public bool? IsActiveSearch { get; set; }
    }
}
