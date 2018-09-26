using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
  
    public class ThirdPartyPurchaseDto
    {
        /// <summary>
        /// 交货期限, ETP
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 外部单据号, SysNo
        /// </summary>
        public string ExternalOrder { get; set; }

        /// <summary>
        /// 供应商ID, VendorSysNo
        /// </summary>
        public string VendorSysId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商联系电话
        /// </summary>
        public string VendorPhone { get; set; }

        /// <summary>
        /// 供应商联系人
        /// </summary>
        public string VendorContacts { get; set; }

        /// <summary>
        /// 采购单备注, Memo
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// 下单时间, InDate
        /// </summary>
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        /// 审核时间, AuditDate
        /// </summary>
        public DateTime? AuditingDate { get; set; }

        /// <summary>
        /// 审核人, AuditBy
        /// </summary>
        public string AuditingBy { get; set; }

        /// <summary>
        /// 审核人姓名, AuditName
        /// </summary>
        public string AuditingName { get; set; }

        /// <summary>
        /// Type, POType
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 单据来源, GZDS
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 仓库编号, WarehouseSysId
        /// </summary>
        public string WarehouseSysId { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get; set; }

        public string ExternOutboundOrder { get; set; }


        /// <summary>
        /// 采购单明细
        /// </summary>
        public List<ThirdPartyPurchaseDetailDto> PurchaseDetailDtoList { get; set; }
    }
}
