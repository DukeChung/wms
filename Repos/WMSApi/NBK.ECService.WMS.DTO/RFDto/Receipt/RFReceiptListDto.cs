using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFReceiptListDto
    {
        public Guid SysId { get; set; }

        public string ReceiptOrder { get; set; }

        public int? Status { get; set; }

        public string DisplayStatus
        {
            get
            {
                return Status.HasValue ? ((ReceiptStatus)Status.Value).ToDescription() : string.Empty;
            }
        }

        public DateTime CreateDate { get; set; }
    }
}
