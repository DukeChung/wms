using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseDetailReturnDto
    {
        public Guid? SysId { get; set; }
        public Guid PurchaseSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public Guid? SkuClassSysId { get; set; }
        public string UomCode { get; set; }
        public Guid UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string PackCode { get; set; }
        public int Qty { get; set; }
        public int ReceivedQty { get; set; }
        public int RejectedQty { get; set; }
        public decimal PurchasePrice { get; set; }
        public string Remark { get; set; }
        public string OtherSkuId { get; set; }
        public string PackFactor { get; set; }

        public DateTime UpdateDate { get; set; }

        public long UpdateBy { get; set; }

        public string UpdateUserName { get; set; }
    }
}
