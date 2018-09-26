using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO
{
    public class ExceedThreeDaysPurchase
    {
        public Guid SysId { get; set; }
        public string PurchaseOrder { get; set; }
        public DateTime? AuditingDate { get; set; }

        public string AuditingDateDisplay
        {
            get {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString(PublicConst.DateFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }
    }
}
