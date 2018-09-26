using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class sku : SysIdEntity
    { 
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public System.Guid SkuClassSysId { get; set; }
        public Nullable<long> ShelfLifeCodeType { get; set; }
        public string SkuDescr { get; set; }
        public Nullable<int> ShelfLifeOnReceiving { get; set; }
        public Nullable<int> ShelfLife { get; set; }
        public System.Guid PackSysId { get; set; }
        public Nullable<int> DaysToExpire { get; set; }
        public System.Guid LotTemplateSysId { get; set; }
        public Nullable<bool> ShelfLifeIndicator { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Cube { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<bool> Fresh { get; set; }
        public Nullable<bool> FragileArticles { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Style { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string OtherId { get; set; }
        public string UPC { get; set; }
        public string OtherUPC1 { get; set; }
        public string OtherUPC2 { get; set; }
        public string OtherUPC3 { get; set; }
        public string OtherUPC4 { get; set; }
        public Nullable<bool> IsInvoices { get; set; }
        public Nullable<bool> IsRefunds { get; set; }
        public Nullable<bool> IsMaterial { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public string RecommendLoc { get; set; }

        public int SpecialTypes { get; set; }
    }
}
