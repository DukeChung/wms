using System;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO
{
    public class AdjustmentDetailGlobalDto: BaseDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string SkuCode { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateDisplay { get { return CreateDate.ToString(PublicConst.DateFormat); } }

        public string AdjustmentLevelDescription { get; set; }

        public string Loc { get; set; }
        public string Lot { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string LotAttr01 { get; set; }

        public string Lpn { get; set; }
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string CreateUserName { get; set; }

        public string WarehouseName { get; set; }
    }
}
