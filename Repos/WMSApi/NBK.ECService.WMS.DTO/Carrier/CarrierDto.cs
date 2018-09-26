using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CarrierDto
    {
        public Guid SysId { get; set; }

        public string CarrierName { get; set; }

        public string CarrierPhone { get; set; }

        public string OtherCarrierId { get; set; }

        public string CarrierContacts { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public string UpdateUserName { get; set; }

        public string CreateUserName { get; set; }
    }
}
