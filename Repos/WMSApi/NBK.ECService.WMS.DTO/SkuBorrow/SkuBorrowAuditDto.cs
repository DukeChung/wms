using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuBorrowAuditDto : BaseDto
    {
        public Guid SysId { get; set; }
        public int AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public string Memo { get; set; }
    }
}
