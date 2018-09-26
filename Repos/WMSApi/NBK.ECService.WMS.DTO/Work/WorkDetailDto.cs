using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkDetailDto : BaseDto
    {
        public Guid SysId { get; set; }
        public string WorkOrder { get; set; }
        public int Status { get; set; }
        public string StatusName
        {
            get { return ((WorkStatus)Status).ToDescription(); }
        }
        public int WorkType { get; set; }
        public string WorkTypeName
        {
            get { return ((UserWorkType)WorkType).ToDescription(); }
        }

        public Nullable<int> Priority { get; set; }

        public Guid AppointUserId { get; set; }
        public string AppointUserName { get; set; }
        public DateTime? StartTime { get; set; }

        public string DisplayStartTime
        {
            get
            {
                if (StartTime.HasValue)
                {
                    return StartTime.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }

        public DateTime? EndTime { get; set; }
        public string DisplayEndTime
        {
            get
            {
                if (EndTime.HasValue)
                {
                    return EndTime.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }

        public DateTime? WorkTime { get; set; }
        public string DisplayWorkTime
        {
            get
            {
                if (WorkTime.HasValue)
                {
                    return WorkTime.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }
        public string Descr { get; set; }
        public string Source { get; set; }
        public Nullable<Guid> DocSysId { get; set; }
        public string DocOrder { get; set; }
        public Nullable<Guid> DocDetailSysId { get; set; }
        public Nullable<Guid> SkuSysId { get; set; }

        public string Lot { get; set; }
        public string Lpn { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public Nullable<int> FromQty { get; set; }
        public Nullable<int> ToQty { get; set; }


    }
}
