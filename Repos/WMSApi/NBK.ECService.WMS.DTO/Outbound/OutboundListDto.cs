using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class OutboundListDto
    {
        public Guid SysId { get; set; }
        public string OutboundOrder { get; set; }

        public int Status { get; set; }

        public string StatusName
        {
            get { return ((OutboundStatus)Status).ToDescription(); }
        }

        public int OutboundType { get; set; }

        public string OutboundTypeName
        {
            get { return ((OutboundType)OutboundType).ToDescription(); }
        }

        public string OutboundChildType { get; set; }

        public DateTime ExternOrderDate { get; set; }

        public string ExternOrderId { get; set; }

        public DateTime CreateDate { get; set; }



        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.ToString(PublicConst.DateTimeFormat);
            }
        }

        public string ExternOrderDateDisplay
        {
            get { return ExternOrderDate.ToString(PublicConst.DateTimeFormat); }
        }
        public DateTime? DepartureDate { get; set; }
        public string DepartureDateDisplay
        {
            get
            {
                if (DepartureDate.HasValue)
                {
                    return DepartureDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        public DateTime? AuditingDate { get; set; }

        public string AuditingDateDisplay
        {
            get
            {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateDisplay
        {
            get
            {
                if (ActualShipDate.HasValue)
                {
                    return ActualShipDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }

        public int TotalQty { get; set; }

        public decimal DisplayTotalQty { get; set; }

        public decimal DisplayTotalShippedQty { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneeAddress { get; set; }

        public string ConsigneeProvince { get; set; }

        public string ConsigneeCity { get; set; }

        public string ConsigneeArea { get; set; }

        /// <summary>
        /// 根据订单业务进行定制拼接 B2C 去 省市区+地址  B2B 直接取地址返回
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

        public string Remark { get; set; }

        public string ServiceStationCode { get; set; }
        public string ServiceStationName { get; set; }


        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrder { get; set; }


        public int? IsReturn { get; set; }

        public string RetnrnName
        {
            get
            {
                if (IsReturn == (int)Utility.Enum.OutboundReturnStatus.AllReturn || IsReturn == (int)Utility.Enum.OutboundReturnStatus.PartReturn || IsReturn == (int)Utility.Enum.OutboundReturnStatus.B2CReturn)
                {
                    return "退";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// TMS装车号
        /// </summary>
        public int? SortNumber { get; set; }
        /// <summary>
        /// TMS运单号
        /// </summary>
        public string TMSOrder { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string AppointUserNames { get; set; }
        public bool? Exception { get; set; }

        public string ExceptionName
        {
            get
            {
                if (Exception.HasValue && Exception.Value)
                {
                    return "异";
                }
                return string.Empty;
            }
        }

        public bool IsInvoice { get; set; }

        public string IsInvoiceName
        {
            get
            {
                if (IsInvoice)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 分配时间
        /// </summary>
        public DateTime? PickdetailCreateDate { get; set; }

        public string PickdetailCreateDateDisplay
        {
            get
            {
                if (PickdetailCreateDate.HasValue)
                {
                    return PickdetailCreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }

    }
}
