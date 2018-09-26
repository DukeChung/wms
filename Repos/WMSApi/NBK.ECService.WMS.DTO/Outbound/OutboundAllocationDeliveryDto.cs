using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class OutboundAllocationDeliveryDto : BaseDto
    {
        public Guid? SysId { get; set; }

        public string OutboundOrder { get; set; }
        public string PickSysIds { get; set; }

        /// <summary>
        /// 0：单个出库单ID，1:List型出库单Ids
        /// </summary>
        public int Type { get; set; }

        public List<string> OutboundSysIdList
        {
            get
            {

                if (!string.IsNullOrEmpty(PickSysIds))
                {
                    var ids = PickSysIds.Substring(0, PickSysIds.Length - 1);

                    var list = ids.Split(',');
                    return new List<string>(list);
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> SNList { get; set; }
    }
}
