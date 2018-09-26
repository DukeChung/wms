using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMS.Model.Models
{
    public class picking : SysIdEntity
    {
        public string PickingOrder { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public System.Guid ReceiptSysId { get; set; }
        public string ReceiptOrder { get; set; }
        public int PickingNumber { get; set; }
        public Nullable<int> PickingUserId { get; set; }
        public string PickingUserName { get; set; }
        public System.DateTime PickingDate { get; set; }
        public string Remark { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
