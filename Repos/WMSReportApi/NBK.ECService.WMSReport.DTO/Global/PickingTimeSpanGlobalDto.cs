using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class PickingTimeSpanGlobalDto : BaseDto
    {
        public string WarehouseName { get; set; }

        public string OutboundOrder { get; set; }

        public int SkuTypeNumber { get; set; }

        public int SkuTotalCount { get; set; }

        public string Operator { get; set; }

        public DateTime StartTime { get; set; }

        public string StartTimeDisplay
        {
            get
            {
                return StartTime.ToString(PublicConst.DateTimeFormat);
            }
        }

        public DateTime EndTime { get; set; }

        public string EndTimeDisplay
        {
            get
            {
                return EndTime.ToString(PublicConst.DateTimeFormat);
            }
        }

        public string StayTime
        {
            get
            {
                TimeSpan dtSpan = EndTime - StartTime;
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分" + dtSpan.Seconds + "秒";
            }
        }
    }
}
