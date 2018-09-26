using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PickOutboundListDto
    {
        public Guid? SysId { get; set; }
        public string OutboundOrder { get; set; }
        public string ExternOrderId { get; set; }
        public int? OutboundType { get; set; }
        public DateTime? OutboundDate { get; set; }
        public DateTime? AuditingDate { get; set; }
        public int? SkuTypeQty { get; set; }
        public int? TotalQyt { get; set; }
        public string Remark { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }

        public string ServiceStationName { get; set; }

        public string OutboundTypeText
        {
            get
            {
                if (OutboundType.HasValue)
                {
                    return ConvertType.Outbound(OutboundType.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string ExternOrderDateText
        {
            get
            {
                if (OutboundDate.HasValue)
                {
                    return OutboundDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string AuditingDateText
        {
            get
            {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OutboundChildType { get; set; }
    }
}
