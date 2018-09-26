using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyViewDto
    {
        public Guid SysId { get; set; }

        public string AssemblyOrder { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuUPC { get; set; }

        public int Status { get; set; }

        public string StatusText { get { return ((AssemblyStatus)Status).ToDescription(); } }

        public string Remark { get; set; }

        public DateTime? PlanProcessingDate { get; set; }

        public string PlanProcessingDateText { get { return PlanProcessingDate.HasValue ? PlanProcessingDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? PlanCompletionDate { get; set; }

        public string PlanCompletionDateText { get { return PlanCompletionDate.HasValue ? PlanCompletionDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public int PlanQty { get; set; }

        public int ActualQty { get; set; }

        public DateTime? ActualProcessingDate { get; set; }

        public string ActualProcessingDateText { get { return ActualProcessingDate.HasValue ? ActualProcessingDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? ActualCompletionDate { get; set; }

        public string ActualCompletionDateText { get { return ActualCompletionDate.HasValue ? ActualCompletionDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public Guid WareHouseSysId { get; set; }

        public string WareHouseName { get; set; }

        public List<AssemblyDetailDto> AssemblyDetails { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUserName { get; set; }

        public string Lot { get; set; }

        public string Packing { get; set; }

        public string PackWeight { get; set; }

        public string PackGrade { get; set; }

        public string StorageConditions { get; set; }

        public string PackSpecification { get; set; }

        public string PackDescr { get; set; }

        public string Source { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }

        public int ShelvesQty { get; set; }

        public int ShelvesStatus { get; set; }

        public string ShelvesStatusDisplay { get { return ((ShelvesStatus)ShelvesStatus).ToDescription(); } }
    }
}
