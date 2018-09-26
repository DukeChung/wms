using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkListDto
    {
        public Guid SysId { get; set; }
        public string WorkOrder { get; set; }

        public int Status { get; set; }
        public string StatusName
        {
            get { return ((WorkStatus)Status).ToDescription(); }
        }
        public string DocOrder { get; set; }
        public string AppointUserName { get; set; }

        public int WorkType { get; set; }

        public string WorkTypeDisplay
        {
            get { return ((UserWorkType)WorkType).ToDescription(); }
        }

        public string Source { get; set; }
        public DateTime CreateDate { get; set; }

        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.ToString(PublicConst.DateTimeFormat);
            }
        }
    }
}
