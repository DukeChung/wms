using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptOperationDto : BaseDto
    {
        public Guid SysId { get; set; }

        public int PurcharType { get; set; }

        public int Qty { get; set; }
        public Guid PurchaseSysId { get; set; }

        public string ReceiptOrder { get; set; }
        public string ExternalOrder { get; set; }
        public int ReceiptType { get; set; }
        public DateTime? ExpectedReceiptDate { get; set; }
        public int? Status { get; set; }
        public string Descr { get; set; }
        public string VendorName { get; set; }
        public string VendorContacts { get; set; }
        public string VendorPhone { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string SkuUPC { get; set; }

        public string ExpectedReceiptDateText
        {
            get
            {
                if (ExpectedReceiptDate.HasValue)
                {
                    return ExpectedReceiptDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string StatusText
        {
            get
            {
                if (Status.HasValue)
                {
                    return ConverStatus.Receipt(Status.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 扫描单位数量
        /// </summary>
        public int UnitQty { get; set; }

        public string ScanBarCode { get; set; }

        /// <summary>
        /// 采购单备注
        /// </summary>
        public string PurchaseDescr { get; set; }
        /// <summary>
        /// 作业人
        /// </summary>
        public string AppointUserNames { get; set; }

        public List<PurchaseDetailViewDto> PurchaseDetailViewDto { get; set; }
        public SkuCurrentDto SkuCurrentDto { get; set; }

        public List<ReceiptSNDto> ReceiptSNDto { get; set; }
        public List<ReceiptDetailOperationDto> ReceiptDetailOperationDto { get; set; }

        public List<LotTemplateValueDto> LotTemplateValueDtos { get; set; }

        public List<PurchaseDetailSkuDto> PurchaseDetailSkuDto { get; set; }

        public List<string> SNList { get; set; }

        public bool IsScanSNOrder
        {
            get
            {
                if (PurchaseDetailViewDto != null && PurchaseDetailViewDto.Count > 0)
                {
                    foreach (var item in PurchaseDetailViewDto)
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
    }
}