using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 渠道库存
    /// </summary>
    public class ChannelQtyDto
    {
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Channel { get; set; }
        public string ChannelName
        {
            get
            {
                if (!string.IsNullOrEmpty(Channel))
                {
                    return Channel;
                }
                else
                {
                    return "无渠道";
                }
            }
        }
        /// <summary>
        /// 库存
        /// </summary>
        public long Qty { get; set; }
    }
}
