using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class AddOutboundExceptionDto : BaseDto
    {
        public Guid OutboundSysId { get; set; }
        public List<OutboundExceptionDtoList> OutboundExceptionDtoList { get; set; }
    }


    public class OutboundExceptionDtoList
    {
        public Guid? SysId { get; set; }
        public Guid OutboundSysId { get; set; }
        public Guid OutboundDetailSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public int? MaxQty { get; set; }
        public string ExceptionReason { get; set; }
        public int ExceptionQty { get; set; }
        public string ExceptionDesc { get; set; }
        public string Result { get; set; }
        public string Department { get; set; }
        public string Responsibility { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> IsSettlement { get; set; }
    }
}
