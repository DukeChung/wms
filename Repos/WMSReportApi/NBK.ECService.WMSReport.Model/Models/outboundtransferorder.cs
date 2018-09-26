using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class outboundtransferorder : SysIdEntity
    {
        public System.Guid OutboundSysId { get; set; }
        public string TransferOrder { get; set; }
        public string OutboundOrder { get; set; }
        public int BoxNumber { get; set; }
        public long CreateBy { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public string ConsigneeArea { get; set; }
        public string ServiceStationName { get; set; }
        public Nullable<System.Guid> PreBulkPackSysId { get; set; }
        public string PreBulkPackOrder { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> SkuQty { get; set; }

        public System.Guid WareHouseSysId { get; set; }
        public int TransferType { get; set; }

        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }

        public long? ReviewBy { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewUserName { get; set; }
    }
}
