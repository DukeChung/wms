using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintStockTakeDto : BaseDto
    {
        public PrintStockTakeDto()
        {
            PrintStockTakeDetails = new List<PrintStockTakeDetailDto>();
        }

        public Guid SysId { get; set; }

        public string AssignUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }

        public List<PrintStockTakeDetailDto> PrintStockTakeDetails { get; set; }
    }
}
