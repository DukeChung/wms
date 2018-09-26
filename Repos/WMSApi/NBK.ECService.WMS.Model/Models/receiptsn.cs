using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class receiptsn : SysIdEntity
    { 
        public Nullable<System.Guid> ReceiptSysId { get; set; }
        public Nullable<System.Guid> SkuSysId { get; set; }
        public string SN { get; set; }

        public Nullable<System.Guid> OutboundSysId { get; set; }

        public Nullable<System.Guid> PurchaseSysId { get; set; }

        public Guid WarehouseSysId { get; set; }

        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }

        public int Status { get; set; }

        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
