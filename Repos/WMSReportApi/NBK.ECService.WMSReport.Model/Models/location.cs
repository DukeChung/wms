using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class location : SysIdEntity
    { 
        public string Loc { get; set; }
        public string LocUsage { get; set; }
        public string LocCategory { get; set; }
        public string LocFlag { get; set; }
        public string LocHandling { get; set; }
        public Nullable<System.Guid> ZoneSysId { get; set; }
        public Nullable<int> LogicalLoc { get; set; }
        public Nullable<decimal> XCoord { get; set; }
        public Nullable<decimal> YCoord { get; set; }
        public Nullable<int> LocLevel { get; set; }
        public Nullable<decimal> Cube { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<decimal> CubicCapacity { get; set; }
        public Nullable<decimal> WeightCapacity { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public virtual zone zone { get; set; }

        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }

        public Guid WarehouseSysId { get; set; }
    }
}
