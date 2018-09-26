using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.MQ
{
    [Serializable]
    [DataContract]
    public class MQTransferInventoryDto
    {
        [DataMember]
        public string TransferInventoryOrder { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public Guid ReceiptSysId { get; set; }

        [DataMember]
        public Guid PurchaseSysId { get; set; }

        [DataMember]
        public List<PurchaseDetailViewDto> PurchaseDetailViewDto { get; set; }

        [DataMember]
        public DateTime? TransferOutboundDate { get; set; }

        [DataMember]
        public DateTime? AuditingDate { get; set; }

        [DataMember]
        public string AuditingBy { get; set; }
        [DataMember]
        public string AuditingName { get; set; }
        [DataMember]
        public Guid ToWareHouseSysId { get; set; }

        [DataMember]
        public string PurchaseOrder { get; set; }

        [DataMember]
        public Guid FromWareHouseSysId { get; set; }

        [DataMember]
        public int CurrentUserId { get; set; }
        [DataMember]
        public string CurrentDisplayName { get; set; }
        [DataMember]
        public Guid WarehouseSysId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        [DataMember]
        public string Channel { get; set; }

        [DataMember]
        public List<TransferInventoryDetailDto> transferinventorydetails;

        [DataMember]
        public List<MQTransferinventoryReceiptExtendDto> Transferinventoryreceiptextends;
    }
}
