using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundLngLatDto
    {
        /// <summary>
        /// 主见
        /// </summary>
        public Guid SysId { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string ConsigneeProvince { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string ConsigneeCity { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string ConsigneeArea { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ConsigneeAddress { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string lat { get; set; }
        /// <summary>
        /// 出库单类型
        /// </summary>
        public int OutboundType { get; set; }

        /// <summary>
        /// 全地址
        /// </summary>
        public string FullAddress
        {
            get
            {
                if (OutboundType == (int)Utility.Enum.OutboundType.B2B || OutboundType == (int)Utility.Enum.OutboundType.Material || OutboundType == (int)Utility.Enum.OutboundType.Fertilizer)
                {
                    return ConsigneeAddress;
                }
                else
                {
                    return ConsigneeProvince + ConsigneeCity + ConsigneeArea + ConsigneeAddress;
                }
            }
        }
    }
}
