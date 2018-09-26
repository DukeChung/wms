using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
 
    public class ThirdPartyResponse
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string errorMessages { get; set; }
    }
}
