using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class UpdatePurchaseDetailDto
    {
        public Guid SysId { get; set; }

        public int ReceivedQty { get; set; }

        public int ReceivedGiftQty { get; set; }

        public int RejectedQty { get; set; }

        public int RejectedGiftQty { get; set; }

        public string Remark { get; set; }

        public DateTime UpdateDate { get; set; }

        public long UpdateBy { get; set; }

        public string UpdateUserName { get; set; }
    }
}
