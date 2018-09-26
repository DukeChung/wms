using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Home
{
    public class EachChannelInvOutChartDto
    {
        public string[] Dates { get; set; }
        public List<string> LotAttr { get; set; }

        public List<EachChannelInvOutDataDto> EachChannelInvOutDataDto { get; set; }
    }

    public class EachChannelInvOutDataDto
    {
        public string Date { get; set; }  //2017-03-31
        public List<EachChannelInvOutDto> EachChannelInvOutDto { get; set; }
    }

    public class EachChannelInvOutDto
    {
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string LotAttr01 { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
