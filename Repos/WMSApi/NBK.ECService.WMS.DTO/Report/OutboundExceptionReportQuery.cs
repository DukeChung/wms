using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundExceptionReportQuery : BaseQuery
    {
        /// <summary>
        /// 州/市
        /// </summary>
        public string ConsigneeCity { get; set; }
        /// <summary>
        /// 区/县
        /// </summary>
        public string ConsigneeArea { get; set; }
        /// <summary>
        /// 乡/镇
        /// </summary>
        public string ConsigneeTown { get; set; }

        public string ServiceStationName { get; set; }
        /// <summary>
        /// 服务站编码
        /// </summary>
        public string ServiceStationCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsSettlement { get; set; }
    }
}
