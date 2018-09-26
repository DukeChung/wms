using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class UpdateReceiptDetailDto : BaseDto
    {
        public Guid SysId { get; set; }

        public int ShelvesQty { get; set; }

        public int ShelvesStatus { get; set; }

        public Guid OldTS { get; set; }

        public Guid NewTS { get; set; }


    }
}
