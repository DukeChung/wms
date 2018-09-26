using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PurchaseDto
    {
        public Guid? SysId { get; set; }
        public string PurchaseOrder { get; set; }
        public string ExternalOrder { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string Descr { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? AuditingDate { get; set; }

        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public long CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Type { get; set; }

        public int? Status { get; set; }
        public string Source { get; set; }

        public string StatusText
        {
            get
            {
                if (Status.HasValue)
                {
                    return ConverStatus.Purchase(Status.Value);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public virtual List<PurchaseDetailDto> PurchaseDetailDto { get; set; }

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
        /// 对应出库单sysid
        /// </summary>
        public Guid? OutboundSysId { get; set; }

        /// <summary>
        /// 对应出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

    }
}
