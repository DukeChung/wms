using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class TransferinventoryGlobalQuery : BaseQuery
    {
        public string TransferInventoryOrder { get; set; }

        public Guid FromWareHouseSysId { get; set; }

        public Guid ToWareHouseSysId { get; set; }

        public string TransferPurchaseOrder { get; set; }

        public string TransferOutboundOrder { get; set; }

        public int? Status { get; set; }

        public string ExternOrderId { get; set; }
    }
}
