using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Home
{
    public class ChannelInvOutChartQueryDto
    {
        /// <summary>
        /// 类型：1发货，2入库
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>

        public Guid WareHouseSysId { get; set; }

        public int Type { get; set; }
    }
}
