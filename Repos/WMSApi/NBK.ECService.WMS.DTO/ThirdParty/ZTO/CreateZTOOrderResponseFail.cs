using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty.ZTO
{
    public class CreateZTOOrderResponseFail: ZTOOrderResponse
    {  
        public string code { get; set; }

        public string remark { get; set; }
    }
}
