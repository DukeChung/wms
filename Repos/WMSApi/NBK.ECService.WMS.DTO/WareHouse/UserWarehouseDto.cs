using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class UserWarehouseDto : BaseDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public Guid MapWarehouseSysId { get; set; }

        public DateTime? CreateDate { get; set; }
        public string WarehouseName { get; set; }
    }

    public class UserWarehouseQuery : BaseQuery
    {
        public int? UserId { get; set; }
    }
}
