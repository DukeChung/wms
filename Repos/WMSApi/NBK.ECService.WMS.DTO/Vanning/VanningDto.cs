using System;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class VanningDto
    {
        public Guid? SysId { get; set; }
        public string VanningOrder { get; set; }

        public string OutboundOrder { get; set; }

        public int? Status { get; set; }

        public int? VanningType { get; set; }

        public DateTime? VanningDate { get; set; }

        public string VanningDateText {
            get
            {
                if (VanningDate.HasValue && VanningDate != new DateTime())
                {
                    return VanningDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string TypeText
        {
            get { return VanningType.HasValue ? ((VanningType)VanningType).ToDescription() : string.Empty; }
        }

        public string StatusText
        {
            get { return Status.HasValue ? ((VanningStatus)Status.Value).ToDescription() : string.Empty; }
        }
    }
}