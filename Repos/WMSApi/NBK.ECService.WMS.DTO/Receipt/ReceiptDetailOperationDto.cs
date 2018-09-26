using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptDetailOperationDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string SkuUPC { get; set; }
        public int PurchaseQty { get; set; }
        public int PurchaseGiftQty { get; set; }
        public int ReceivedQty { get; set; }
        /// <summary>
        /// 破损数量
        /// </summary>
        public int AdjustmentQty { get; set; }
        public decimal GiftQty { get; set; }
        public decimal RejectedQty { get; set; }
        public decimal RejectedGiftQty { get; set; }
        public string Descr { get; set; }

        public string ToLoc { get; set; }
        public string ToLot { get; set; }
        public string ToLpn { get; set; }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        public string ExternalLot { get; set; }
        public DateTime? ProduceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public LotTemplateDto LotTemplateDto { get; set; }
    }
}