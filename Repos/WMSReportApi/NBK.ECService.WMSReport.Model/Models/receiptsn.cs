using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class receiptsn : SysIdEntity
    { 
        public Nullable<System.Guid> ReceiptSysId { get; set; }
        public Nullable<System.Guid> SkuSysId { get; set; }
        public string SN { get; set; }
        [JsonIgnore]
        public virtual receipt receipt { get; set; }
    }
}
