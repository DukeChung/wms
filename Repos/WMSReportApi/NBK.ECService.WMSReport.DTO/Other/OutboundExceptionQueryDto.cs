using System;

using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class OutboundExceptionQueryDto : BaseQuery
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OutboundSysId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SkuName { get; set; }
    }
}
