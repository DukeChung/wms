using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class HandoverGroupDto
    {
        //public Guid SysId { get; set; }

        public string HandoverGroupOrder { get; set; }

        public int TotalCount { get; set; }

        public decimal TotalWeight { get; set; }

        public string CarrierNumber { get; set; }

        public string CarrierName { get; set; }

        public DateTime? HandoverCreateDate { get; set; }

        public string HandoverCreateDateDisplay
        {
            get
            {
                if (HandoverCreateDate.HasValue)
                {
                    return HandoverCreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }

        public List<HandoverGroupDetailDto> HandoverGroupDetailList { get; set; }
    }
}
