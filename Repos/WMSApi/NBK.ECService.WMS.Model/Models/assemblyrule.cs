using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class assemblyrule : SysIdEntity
    {
        public assemblyrule()
        {
        }
        public bool Status { get; set; }
        public bool MatchingLotAttr { get; set; }
        public int DeliverySortRules { get; set; } 
        public bool MatchingSkuBorrowChannel { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }
    }
}
