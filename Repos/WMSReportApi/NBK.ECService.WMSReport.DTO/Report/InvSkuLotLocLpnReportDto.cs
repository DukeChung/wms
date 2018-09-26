using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuLotLocLpnReportDto
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }
        public string Lot { get; set; }

        public string Loc { get; set; }

        public string Lpn { get; set; }

        public int Qty { get; set; }

        public int AllocatedQty { get; set; }

        public int PickedQty { get; set; }

        public int FrozenQty { get; set; }

        public decimal DisplayQty { get; set; }

        public decimal DisplayAllocatedQty { get; set; }

        public decimal DisplayPickedQty { get; set; }

        public decimal DisplayFrozenQty { get; set; }

        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string LotAttr01 { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ProduceDate { get; set; }

        public string ProduceDateDisplay
        {
            get
            {
                return ProduceDate.HasValue ? ProduceDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        public string ExpiryDateDisplay
        {
            get
            {
                return ExpiryDate.HasValue ? ExpiryDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }
    }

    public class InvSkuLotLocLpnReportQuery : BaseQuery
    {
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string Lot { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string LotAttr01 { get; set; }

        public string Loc { get; set; }
    }
}
