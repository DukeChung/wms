using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class QualityControlDto : BaseDto
    {
        public Guid SysId { get; set; }
        public Guid WareHouseSysId { get; set; }
        public int Status { get; set; }
        public string StatusText { get { return ((QCStatus)Status).ToDescription(); } }
        public string QCOrder { get; set; }
        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public DateTime UpdateDate { get; set; }
        public int QCType { get; set; }
        public string QCTypeText { get { return ((QCType)QCType).ToDescription(); } }
        public string ExternOrderId { get; set; }
        public string DocOrder { get; set; }
        public string QCUserName { get; set; }
        public DateTime? QCDate { get; set; }
        public string QCDateText { get { return QCDate.HasValue ? QCDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }
        public string Descr { get; set; }
    }
}
