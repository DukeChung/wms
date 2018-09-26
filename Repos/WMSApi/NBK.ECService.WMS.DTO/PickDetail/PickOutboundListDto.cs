using System;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
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

            get { return ((OutboundType)OutboundType).ToDescription(); }
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