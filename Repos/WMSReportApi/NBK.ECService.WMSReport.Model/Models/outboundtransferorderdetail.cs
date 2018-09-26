using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class outboundtransferorderdetail : SysIdEntity
    {
        public System.Guid OutboundTransferOrderSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public Guid? UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }

        public string ExternalLot { get; set; }
        public DateTime? ProduceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Qty { get; set; }

        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
