using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class assembly : SysIdEntity
    {
        public string AssemblyOrder { get; set; }
        public string ExternalOrder { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> PlanProcessingDate { get; set; }
        public Nullable<System.DateTime> PlanCompletionDate { get; set; }
        public int PlanQty { get; set; }
        public int ActualQty { get; set; }
        public Nullable<System.DateTime> ActualProcessingDate { get; set; }
        public Nullable<System.DateTime> ActualCompletionDate { get; set; }
        public int ShelvesQty { get; set; }
        public int ShelvesStatus { get; set; }
        public string Source { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public string Lot { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string Packing { get; set; }
        public string PackWeight { get; set; }
        public string PackGrade { get; set; }
        public string StorageConditions { get; set; }
        public string PackSpecification { get; set; }
        public string PackDescr { get; set; }
        public string Channel { get; set; }
        public string BatchNumber { get; set; }

    }
}
