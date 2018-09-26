using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class LocationDto : BaseDto
    {
        public Guid SysId { get; set; }

        public string Loc { get; set; }

        public string LocUsage { get; set; }

        public string LocCategory { get; set; }

        public string LocFlag { get; set; }

        public string LocHandling { get; set; }

        public Guid? ZoneSysId { get; set; }

        public int? LogicalLoc { get; set; }

        public decimal? XCoord { get; set; }

        public decimal? YCoord { get; set; }

        public int? LocLevel { get; set; }

        public decimal? Cube { get; set; }

        public int? Length { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public decimal? CubicCapacity { get; set; }

        public decimal? WeightCapacity { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        public int Status { get; set; }
    }
}
