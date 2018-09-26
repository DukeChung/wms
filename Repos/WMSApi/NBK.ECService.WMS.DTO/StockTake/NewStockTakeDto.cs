using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class NewStockTakeDto : BaseDto
    {
        public int StockTakeType { get; set; }

        public Guid? ZoneSysId { get; set; }

        public Guid? LocSysId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<Guid> SkuSysIds { get; set; }

        public int? AssignBy { get; set; }

        public string AssignUserName { get; set; }
    }
}
