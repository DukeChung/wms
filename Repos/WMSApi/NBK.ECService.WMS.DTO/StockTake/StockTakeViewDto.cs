using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeViewDto
    {
        public Guid SysId { get; set; }

        public string StockTakeOrder { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateFormat); } }

        public string CreateUserName { get; set; }
         
        public int Status { get; set; }

        public string StatusText { get { return ((StockTakeStatus)Status).ToDescription(); } }

        public Guid WarehouseSysId { get; set; }

        public string WarehouseName { get; set; }

        public int StockTakeType { get; set; }
    }
}
