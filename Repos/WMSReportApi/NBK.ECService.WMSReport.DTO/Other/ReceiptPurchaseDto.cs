using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class ReceiptPurchaseDto
    {
        public Guid SysId { get; set; }
        public string ReceiptOrder { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public int Status { get; set; }
        public string Descr { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateUserName { get; set; }

        public string ReceiptDateText
        {
            get
            {
                if (ReceiptDate != null)
                {
                    return ReceiptDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return PublicConst.NotInbound;
                }
            }
        }

        public string StatusText
        {
            get { return ConverStatus.Receipt(Status); }
        }
    }
}
