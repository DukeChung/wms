using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class OutboundBizLogDto
    {
        /// <summary>
        /// 出库访问总次数
        /// </summary>
        public int OutboundPorcessTotalCount { get; set; }

        /// <summary>
        /// 出库访问成功次数
        /// </summary>
        public int OutboundPorcessSuccessCount { get; set; }

        public string OutboundPorcessSuccessPercent
        {
            get
            {
                if (OutboundPorcessTotalCount > 0)
                {
                    return (OutboundPorcessSuccessCount / OutboundPorcessTotalCount).ToString("P2");
                }

                return "0.00%";
            }
        }

        /// <summary>
        /// 出库访问失败次数
        /// </summary>
        public int OutboundPorcessFailCount { get; set; }

        public List<BizApiDisplayDto> OutboundBizApiDisplayDtoList { get; set; } = new List<BizApiDisplayDto>();
    }
}
