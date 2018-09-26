using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class vendor : SysIdEntity
    { 
        public string VendorName { get; set; }
        public string VendorPhone { get; set; }
        public string OtherVendorId { get; set; }
        public string VendorContacts { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }

        public Nullable<int> DeliveryCount { get; set; }
    }
}
