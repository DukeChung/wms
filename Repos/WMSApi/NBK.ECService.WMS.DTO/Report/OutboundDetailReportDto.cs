using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundDetailReportDto : BaseDto
    {
        public Guid SysId { get; set; }
        public string OutboundOrder { get; set; }

        public int Status { get; set; }

        public string StatusDisplay { get { return ((Utility.Enum.OutboundStatus)Status).ToDescription(); } }

        public int OutboundType { get; set; }

        public string OutboundTypeDisplay { get { return ((Utility.Enum.OutboundType)OutboundType).ToDescription(); } }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneePhone { get; set; }

        public string ConsigneeAddress { get; set; }

        public string FullAddress { get; set; }


        public string AddressDisplay
        {
            get
            {
                if (OutboundType == (int)Utility.Enum.OutboundType.B2B || OutboundType == (int)Utility.Enum.OutboundType.Fertilizer)
                {
                    return ConsigneeAddress;
                }
                else
                {
                    return FullAddress;
                }
            }
        }

        public decimal Qty { get; set; }

        public decimal ShippedQty { get; set; }

        public DateTime? OutboundDate { get; set; }

        public string OutboundDateDisplay { get { return OutboundDate.HasValue ? OutboundDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateDisplay { get { return ActualShipDate.HasValue ? ActualShipDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }
        /// <summary>
        /// 服务站编码
        /// </summary>
        public string ServiceStationCode { get; set; }

        /// <summary>
        /// 服务站
        /// </summary>
        public string ServiceStationName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OutboundChildType { get; set; }

        /// <summary>
        /// TMS运单号
        /// </summary>
        public string TMSOrder { get; set; }
        /// <summary>
        /// TMS装车顺序
        /// </summary>
        public int? SortNumber { get; set; }


        public string SortNumberDisplay
        {
            get
            {
                return SortNumber.HasValue ? SortNumber.Value.ToString() : string.Empty; ;
            }
        }

        /// <summary>
        /// TMS装车时间
        /// </summary>
        public DateTime? DepartureDate { get; set; }


        public string DepartureDateDisplay { get { return DepartureDate.HasValue ? DepartureDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }
    }

    public class OutboundDetailReportQuery : BaseQuery
    {
        public string OutboundOrder { get; set; }

        public string ExternOrderId { get; set; }

        public int? Status { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int? OutboundType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OutboundChildType { get; set; }

        public DateTime? ActualShipDateFrom { get; set; }

        public DateTime? ActualShipDateTo { get; set; }

        public bool? IsMaterial { get; set; }
        /// <summary>
        /// 服务站编码
        /// </summary>
        public string ServiceStationCode { get; set; }
        /// <summary>
        /// 服务站
        /// </summary>
        public string ServiceStationName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ConsigneeAddress { get; set; }
        /// <summary>
        /// 装车开始时间
        /// </summary>
        public DateTime? DepartureDateFrom { get; set; }
        /// <summary>
        /// 装车结束时间
        /// </summary>
        public DateTime? DepartureDateTo { get; set; }

        /// <summary>
        /// 是否导出
        /// </summary>
        public bool IsExport { get; set; }
    }

}
