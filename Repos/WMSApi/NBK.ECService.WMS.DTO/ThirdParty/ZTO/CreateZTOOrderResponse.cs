using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty.ZTO
{
    public class CreateZTOOrderResponse: ZTOOrderResponse
    {  
        public keys keys { get; set; }
    }

    public class keys
    {
        public string id { get; set; }

        public string orderid { get; set; }

        public string mailno { get; set; }

        public string mark { get; set; }

        public string isupdate { get; set; }

        public string sitecode { get; set; }

        public string sitename { get; set; }
    }
}
