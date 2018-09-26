using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class outboundexceptionMap : EntityTypeConfiguration<outboundexception>
    {
        public outboundexceptionMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.ToTable("outboundexception", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.OutboundDetailSysId).HasColumnName("OutboundDetailSysId");
            this.Property(t => t.ExceptionQty).HasColumnName("ExceptionQty");
            this.Property(t => t.ExceptionReason).HasColumnName("ExceptionReason");
            this.Property(t => t.ExceptionDesc).HasColumnName("ExceptionDesc");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.Responsibility).HasColumnName("Responsibility");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsSettlement).HasColumnName("IsSettlement");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
