using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InboundBizLogDto
    {
        /// <summary>
        /// 入库访问总次数
        /// </summary>
        public int InboundPorcessTotalCount { get; set; }

        /// <summary>
        /// 入库访问成功次数
        /// </summary>
        public int InboundPorcessSuccessCount { get; set; }

        /// <summary>
        /// 入库访问失败次数
        /// </summary>
        public int InboundPorcessFailCount { get; set; }

        public string InboundPorcessSuccessPercent
        {
            get
            {
                if (InboundPorcessTotalCount > 0)
                {
                    return (InboundPorcessSuccessCount / InboundPorcessTotalCount).ToString("P2");
                }

                return "0.00%";
            }
        }

        public List<BizApiDisplayDto> InboundBizApiDisplayDtoList { get; set; } = new List<BizApiDisplayDto>();
    }

    public class BizApiDisplayDto
    {

        public string BussinessName { get; set; }

        public int ProcessCount { get; set; }
    }
}
