using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CancelPickQtyDto : BaseDto
    {
        public Guid SysId { get; set; }

        public int Qty { get; set; }

        public string StorageCase { get; set; }
    }
}
