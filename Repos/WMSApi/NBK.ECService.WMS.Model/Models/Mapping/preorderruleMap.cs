using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class preorderruleMap : EntityTypeConfiguration<preorderrule>
    {
        public preorderruleMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("preorderrule", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.MatchingRate).HasColumnName("MatchingRate");
            this.Property(t => t.MatchingMaxRate).HasColumnName("MatchingMaxRate");
            this.Property(t => t.MatchingSku).HasColumnName("MatchingSku");
            this.Property(t => t.MatchingQty).HasColumnName("MatchingQty");
            this.Property(t => t.DeliveryIntercept).HasColumnName("DeliveryIntercept");
            this.Property(t => t.ExceedQty).HasColumnName("ExceedQty");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.ServiceStation).HasColumnName("ServiceStation");
        }
    }
}
