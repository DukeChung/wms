using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty.OutboundReturn
{
    public class CreatePurchaseOrderNumberResponse
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public ResponsePurchaseInfo ResultData { get; set; }
    }

    public class ResponsePurchaseInfo
    {
        public string PurchaseOrder { get; set; }

        public string ExternalOrder { get; set; }

        public string Descr { get; set; }
    }
}
