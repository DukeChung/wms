using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockMovementDto
    {
        public Guid SysId { get; set; }

        public string StockMovementOrder { get; set; }

        public Guid WareHouseSysId { get; set; }

        public int Status { get; set; }

        public string StatusText { get { return ((Utility.Enum.StockMovementStatus)Status).ToDescription(); } }

        public string Descr { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string FromLoc { get; set; }

        public string ToLoc { get; set; }

        public string FromQty { get; set; }

        public string ToQty { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }

        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }

        public string UpdateUserName { get; set; }
    }
}
