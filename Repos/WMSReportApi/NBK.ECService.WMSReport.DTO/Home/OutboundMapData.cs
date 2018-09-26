using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundMapData
    {
        /// <summary>
        /// 订单总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 服务站名称
        /// </summary>
        public string ServiceStationName { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string Lat { get; set; }
    }
}
