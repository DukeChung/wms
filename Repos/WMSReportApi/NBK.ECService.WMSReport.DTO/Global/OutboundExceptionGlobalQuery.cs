using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundExceptionGlobalQuery: BaseQuery
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

        public Guid SearchWarehouseSysId { get; set; }
    }
}
