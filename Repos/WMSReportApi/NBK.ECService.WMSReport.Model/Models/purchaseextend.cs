using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class purchaseextend : SysIdEntity
    {
        public System.Guid PurchaseSysId { get; set; }
        public string PlatformOrderId { get; set; }
        public string CustomerName { get; set; }
        public string ReturnContact { get; set; }
        public string ShippingAddress { get; set; }
        public string ExpressCompany { get; set; }
        public string ExpressNumber { get; set; }
        public Nullable<System.DateTime> ReturnTime { get; set; }
        public string ReturnReason { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }

        public string ServiceStationName { get; set; }

        public string ServiceStationCode { get; set; }
    }
}
