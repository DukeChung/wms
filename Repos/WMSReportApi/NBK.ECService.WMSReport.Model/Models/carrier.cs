using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class carrier : SysIdEntity
    {
        public carrier()
        {
          
        }
        public string CarrierName { get; set; }
        public string CarrierPhone { get; set; }
        public string OtherCarrierId { get; set; }
        public string CarrierContacts { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> DeliveryCount { get; set; }
        public bool IsActive { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
    }
}
