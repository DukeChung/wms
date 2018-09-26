using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReplayStockTakeDto : BaseDto
    {
        public Guid StockTakeSysId { get; set; }

        public int ReplayBy { get; set; }

        public string ReplayUserName { get; set; }

        public long UpdateBy { get; set; }

        public string UpdateUserName { get; set; }
    }
}
