using System;
using NBK.ECService.WMSReport.Utility;


namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class AdventSkuChartDto
    {
        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public int Qty { get; set; }

        public DateTime? ExpiryDate { get; set; } 

        public string ExpiryDateText {
            get
            {
                if (ExpiryDate.HasValue && ExpiryDate != DateTime.Now)
                {
                    return ExpiryDate.Value.ToString(PublicConst.DateFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}