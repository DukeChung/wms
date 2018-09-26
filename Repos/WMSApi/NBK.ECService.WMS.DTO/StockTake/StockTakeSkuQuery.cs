using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeSkuQuery : BaseQuery
    {
        public int StockTakeType { get; set; }

        public Guid? ZoneSysId { get; set; }

        public Guid? LocSysId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid? SkuClassSysId { get; set; }

        public string SkuUPC { get; set; }
    }
}
