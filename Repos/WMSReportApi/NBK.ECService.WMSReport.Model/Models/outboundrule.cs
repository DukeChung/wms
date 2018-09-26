using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class outboundrule : SysIdEntity
    {
        public outboundrule()
        {
        }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> MatchingLotAttr { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }
        public Nullable<int> DeliverySortRules { get; set; }
        public Nullable<bool> DeliveryIsAsyn { get; set; }
    }
}