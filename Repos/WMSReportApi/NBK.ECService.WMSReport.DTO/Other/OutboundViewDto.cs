using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class OutboundViewDto
    {
        public Guid SysId { get; set; }
        public string OutboundOrder { get; set; }

        public string ExternOrderId { get; set; }

        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return ((OutboundStatus)Status).ToDescription();
            }
        }

        public int OutboundType { get; set; }

        public string OutboundTypeName
        {
            get
            {
                return ((OutboundType)OutboundType).ToDescription();
            }
        }

        public DateTime OutboundDate { get; set; }

        public string OutboundDateDisplay
        {
            get
            {
                return OutboundDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public DateTime? AuditingDate { get; set; }

        public string AuditingDateDisplay
        {
            get
            {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return string.Empty;
            }
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

        public int TotalQty { get; set; }

        public decimal DisplayTotalQty { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneeAddress { get; set; }

        public string DetailedAddress { get; set; }

        public string Remark { get; set; }

        public string ServiceStationName { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string AppointUserNames { get; set; }

        /// <summary>
        /// 预包装单
        /// </summary>
        public string PrePackOrder { get; set; }

        public List<OutboundDetailViewDto> OutboundDetailList { get; set; }

        public bool IsScanSNOrder
        {
            get
            {
                if (OutboundDetailList != null && OutboundDetailList.Count > 0)
                {
                    foreach (var item in OutboundDetailList)
                    {
                        if (item.SkuSpecialTypes != (int)SkuSpecialTypes.RedCard)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 来源仓
        /// </summary>
        public string FromWareHouseName { get; set; }
        /// <summary>
        /// 目标仓
        /// </summary>
        public string ToWareHouseName { get; set; }
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }

        /// <summary>
        /// 对应入库单sysid
        /// </summary>
        public Guid? ReceiptSysId { get; set; }

        /// <summary>
        /// 对应入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }
        /// <summary>
        /// TMS装车号
        /// </summary>
        public int? SortNumber { get; set; }
        /// <summary>
        /// TMS运单号
        /// </summary>
        public string TMSOrder { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
