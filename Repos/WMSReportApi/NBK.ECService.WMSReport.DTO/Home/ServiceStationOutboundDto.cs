using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 服务站发货
    /// </summary>
    public class ServiceStationOutboundDto
    {
        /// <summary>
        /// 服务站名称
        /// </summary>
        public string ServiceStationName { get; set; }

        /// <summary>
        /// 发货单数量
        /// </summary>
        public int TotalOrder { get; set; }

        /// <summary>
        /// 发货商品数量
        /// </summary>
        public int TotalQty { get; set; }
    }
}
