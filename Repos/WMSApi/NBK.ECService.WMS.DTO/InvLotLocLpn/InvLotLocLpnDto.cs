using System;

namespace NBK.ECService.WMS.DTO.InvLotLocLpn
{
    public class InvLotLocLpnDto
    {
        public Guid InvLotLocLpnSysId { get; set; }
        public Guid InvLotSysId { get; set; }
        public Guid InvSkuLocSysId { get; set; }

        public int Qty { get; set; }
        public int AllocatedQty { get; set; }

        public int PickedQty { get; set; }
        public Guid SkuSysId { get; set; }

        public int FrozenQty { get; set; }

        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
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
        public DateTime? ReceiptDate { get; set; }

        //public int Qty { get; set; }
        //public int AllocatedQty { get; set; }
        //public int PickedQty { get; set; }
        //public int Status { get; set; }
        //public long CreateBy { get; set; }
        //public DateTime CreateDate { get; set; }
        //public long UpdateBy { get; set; }
        //public DateTime UpdateDate { get; set; }
    }
}