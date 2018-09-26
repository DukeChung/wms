using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class QualityControlDetailDto
    {
        public Guid? SysId { get; set; }
        public Guid QualityControlSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid? UOMSysId { get; set; }
        public string UOMCode { get; set; }
        public Guid? PackSysId { get; set; }
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
        public int? Qty { get; set; }
        public decimal? DisplayQty { get; set; }
        public string Descr { get; set; }
    }
}
