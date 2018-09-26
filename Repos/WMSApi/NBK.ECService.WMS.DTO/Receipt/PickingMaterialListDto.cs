using NBK.ECService.WMS.Utility;
using System;

namespace NBK.ECService.WMS.DTO
{
    public class PickingMaterialListDto : BaseDto
    {
        public Guid SysId { get; set; }

        public Guid ReceiptSysId { get; set; }

        public string ReceiptOrder { get; set; }

        public Guid SkuSysId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public int PickingNumber { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string PickingUserName { get; set; }

        public DateTime PickingDate { get; set; }

        public string PickingDateText
        {
            get
            {
                if (PickingDate != null)
                {
                    return PickingDate.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return PublicConst.NotInbound;
                }
            }
        }
    }
}
