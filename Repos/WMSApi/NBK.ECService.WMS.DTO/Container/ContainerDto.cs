using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ContainerDto: BaseDto
    {
        public Guid? SysId { get; set; }
        public string ContainerName { get; set; }
        public string ContainerDescr { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Cube { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public bool IsActive { get; set; }

        public string IsActiveDisplay
        {
            get
            {
                return IsActive ? "是" : "否";
            }
        }
    }
}
