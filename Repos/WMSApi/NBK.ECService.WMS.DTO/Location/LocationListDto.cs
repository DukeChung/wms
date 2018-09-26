using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class LocationListDto
    {
        public Guid SysId { get; set; }

        public string Loc { get; set; }

        public string LocUsage { get; set; }

        public string LocUsageText { get; set; }

        public Guid? ZoneSysId { get; set; }

        public string ZoneCode { get; set; }

        public int? LogicalLoc { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public string StatusDisplay {
            get {
                return ((LocationStatus)Status).ToDescription();
            }
        }
    }
}
