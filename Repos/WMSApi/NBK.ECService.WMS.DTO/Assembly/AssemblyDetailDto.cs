using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public  class AssemblyDetailDto
    {
        public Guid SysId { get; set; }

        public Guid AssemblySysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public string UOMCode { get; set; }

        public decimal UnitQty { get; set; }

        public int Qty { get; set; }

        public decimal LossQty { get; set; }

        public int Status { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUserName { get; set; }

        public string Grade { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }
    }
}
