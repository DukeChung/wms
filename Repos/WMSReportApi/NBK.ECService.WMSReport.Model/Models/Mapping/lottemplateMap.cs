using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class lottemplateMap : EntityTypeConfiguration<lottemplate>
    {
        public lottemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.LotCode)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(128);

            this.Property(t => t.Lot01)
                .HasMaxLength(32);

            this.Property(t => t.LotType01)
                .HasMaxLength(16);

            this.Property(t => t.LotValue01)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN01)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT01)
                .HasMaxLength(32);

            this.Property(t => t.Lot02)
                .HasMaxLength(32);

            this.Property(t => t.LotType02)
                .HasMaxLength(16);

            this.Property(t => t.LotValue02)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN02)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT02)
                .HasMaxLength(32);

            this.Property(t => t.Lot03)
                .HasMaxLength(32);

            this.Property(t => t.LotType03)
                .HasMaxLength(16);

            this.Property(t => t.LotValue03)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN03)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT03)
                .HasMaxLength(32);

            this.Property(t => t.Lot04)
                .HasMaxLength(32);

            this.Property(t => t.LotType04)
                .HasMaxLength(16);

            this.Property(t => t.LotValue04)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN04)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT04)
                .HasMaxLength(32);

            this.Property(t => t.Lot05)
                .HasMaxLength(32);

            this.Property(t => t.LotType05)
                .HasMaxLength(16);

            this.Property(t => t.LotValue05)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN05)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT05)
                .HasMaxLength(32);

            this.Property(t => t.Lot06)
                .HasMaxLength(32);

            this.Property(t => t.LotType06)
                .HasMaxLength(16);

            this.Property(t => t.LotValue06)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN06)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT06)
                .HasMaxLength(32);

            this.Property(t => t.Lot07)
                .HasMaxLength(32);

            this.Property(t => t.LotType07)
                .HasMaxLength(16);

            this.Property(t => t.LotValue07)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN07)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT07)
                .HasMaxLength(32);

            this.Property(t => t.Lot08)
                .HasMaxLength(32);

            this.Property(t => t.LotType08)
                .HasMaxLength(16);

            this.Property(t => t.LotValue08)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN08)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT08)
                .HasMaxLength(32);

            this.Property(t => t.Lot09)
                .HasMaxLength(32);

            this.Property(t => t.LotType09)
                .HasMaxLength(16);

            this.Property(t => t.LotValue09)
                .HasMaxLength(32);

            this.Property(t => t.DefaultIN09)
                .HasMaxLength(32);

            this.Property(t => t.DefaultOT09)
                .HasMaxLength(32);

            this.Property(t => t.Lot10)
                .HasMaxLength(32);

            this.Property(t => t.Lot11)
                .HasMaxLength(32);

            this.Property(t => t.Lot12)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("lottemplate", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.LotCode).HasColumnName("LotCode");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.SeqNo01).HasColumnName("SeqNo01");
            this.Property(t => t.Lot01).HasColumnName("Lot01");
            this.Property(t => t.LotVisible01).HasColumnName("LotVisible01");
            this.Property(t => t.LotRFVisible01).HasColumnName("LotRFVisible01");
            this.Property(t => t.LotMandatory01).HasColumnName("LotMandatory01");
            this.Property(t => t.LotMandatoryOT01).HasColumnName("LotMandatoryOT01");
            this.Property(t => t.InvCountLotRFVisible01).HasColumnName("InvCountLotRFVisible01");
            this.Property(t => t.LotType01).HasColumnName("LotType01");
            this.Property(t => t.LotValue01).HasColumnName("LotValue01");
            this.Property(t => t.DefaultIN01).HasColumnName("DefaultIN01");
            this.Property(t => t.DefaultOT01).HasColumnName("DefaultOT01");
            this.Property(t => t.SeqNo02).HasColumnName("SeqNo02");
            this.Property(t => t.Lot02).HasColumnName("Lot02");
            this.Property(t => t.LotVisible02).HasColumnName("LotVisible02");
            this.Property(t => t.LotRFVisible02).HasColumnName("LotRFVisible02");
            this.Property(t => t.LotMandatory02).HasColumnName("LotMandatory02");
            this.Property(t => t.LotMandatoryOT02).HasColumnName("LotMandatoryOT02");
            this.Property(t => t.InvCountLotRFVisible02).HasColumnName("InvCountLotRFVisible02");
            this.Property(t => t.LotType02).HasColumnName("LotType02");
            this.Property(t => t.LotValue02).HasColumnName("LotValue02");
            this.Property(t => t.DefaultIN02).HasColumnName("DefaultIN02");
            this.Property(t => t.DefaultOT02).HasColumnName("DefaultOT02");
            this.Property(t => t.SeqNo03).HasColumnName("SeqNo03");
            this.Property(t => t.Lot03).HasColumnName("Lot03");
            this.Property(t => t.LotVisible03).HasColumnName("LotVisible03");
            this.Property(t => t.LotRFVisible03).HasColumnName("LotRFVisible03");
            this.Property(t => t.LotMandatory03).HasColumnName("LotMandatory03");
            this.Property(t => t.LotMandatoryOT03).HasColumnName("LotMandatoryOT03");
            this.Property(t => t.InvCountLotRFVisible03).HasColumnName("InvCountLotRFVisible03");
            this.Property(t => t.LotType03).HasColumnName("LotType03");
            this.Property(t => t.LotValue03).HasColumnName("LotValue03");
            this.Property(t => t.DefaultIN03).HasColumnName("DefaultIN03");
            this.Property(t => t.DefaultOT03).HasColumnName("DefaultOT03");
            this.Property(t => t.SeqNo04).HasColumnName("SeqNo04");
            this.Property(t => t.Lot04).HasColumnName("Lot04");
            this.Property(t => t.LotVisible04).HasColumnName("LotVisible04");
            this.Property(t => t.LotRFVisible04).HasColumnName("LotRFVisible04");
            this.Property(t => t.LotMandatory04).HasColumnName("LotMandatory04");
            this.Property(t => t.LotMandatoryOT04).HasColumnName("LotMandatoryOT04");
            this.Property(t => t.InvCountLotRFVisible04).HasColumnName("InvCountLotRFVisible04");
            this.Property(t => t.LotType04).HasColumnName("LotType04");
            this.Property(t => t.LotValue04).HasColumnName("LotValue04");
            this.Property(t => t.DefaultIN04).HasColumnName("DefaultIN04");
            this.Property(t => t.DefaultOT04).HasColumnName("DefaultOT04");
            this.Property(t => t.SeqNo05).HasColumnName("SeqNo05");
            this.Property(t => t.Lot05).HasColumnName("Lot05");
            this.Property(t => t.LotVisible05).HasColumnName("LotVisible05");
            this.Property(t => t.LotRFVisible05).HasColumnName("LotRFVisible05");
            this.Property(t => t.LotMandatory05).HasColumnName("LotMandatory05");
            this.Property(t => t.LotMandatoryOT05).HasColumnName("LotMandatoryOT05");
            this.Property(t => t.InvCountLotRFVisible05).HasColumnName("InvCountLotRFVisible05");
            this.Property(t => t.LotType05).HasColumnName("LotType05");
            this.Property(t => t.LotValue05).HasColumnName("LotValue05");
            this.Property(t => t.DefaultIN05).HasColumnName("DefaultIN05");
            this.Property(t => t.DefaultOT05).HasColumnName("DefaultOT05");
            this.Property(t => t.SeqNo06).HasColumnName("SeqNo06");
            this.Property(t => t.Lot06).HasColumnName("Lot06");
            this.Property(t => t.LotVisible06).HasColumnName("LotVisible06");
            this.Property(t => t.LotRFVisible06).HasColumnName("LotRFVisible06");
            this.Property(t => t.LotMandatory06).HasColumnName("LotMandatory06");
            this.Property(t => t.LotMandatoryOT06).HasColumnName("LotMandatoryOT06");
            this.Property(t => t.InvCountLotRFVisible06).HasColumnName("InvCountLotRFVisible06");
            this.Property(t => t.LotType06).HasColumnName("LotType06");
            this.Property(t => t.LotValue06).HasColumnName("LotValue06");
            this.Property(t => t.DefaultIN06).HasColumnName("DefaultIN06");
            this.Property(t => t.DefaultOT06).HasColumnName("DefaultOT06");
            this.Property(t => t.SeqNo07).HasColumnName("SeqNo07");
            this.Property(t => t.Lot07).HasColumnName("Lot07");
            this.Property(t => t.LotVisible07).HasColumnName("LotVisible07");
            this.Property(t => t.LotRFVisible07).HasColumnName("LotRFVisible07");
            this.Property(t => t.LotMandatory07).HasColumnName("LotMandatory07");
            this.Property(t => t.LotMandatoryOT07).HasColumnName("LotMandatoryOT07");
            this.Property(t => t.InvCountLotRFVisible07).HasColumnName("InvCountLotRFVisible07");
            this.Property(t => t.LotType07).HasColumnName("LotType07");
            this.Property(t => t.LotValue07).HasColumnName("LotValue07");
            this.Property(t => t.DefaultIN07).HasColumnName("DefaultIN07");
            this.Property(t => t.DefaultOT07).HasColumnName("DefaultOT07");
            this.Property(t => t.SeqNo08).HasColumnName("SeqNo08");
            this.Property(t => t.Lot08).HasColumnName("Lot08");
            this.Property(t => t.LotVisible08).HasColumnName("LotVisible08");
            this.Property(t => t.LotRFVisible08).HasColumnName("LotRFVisible08");
            this.Property(t => t.LotMandatory08).HasColumnName("LotMandatory08");
            this.Property(t => t.LotMandatoryOT08).HasColumnName("LotMandatoryOT08");
            this.Property(t => t.InvCountLotRFVisible08).HasColumnName("InvCountLotRFVisible08");
            this.Property(t => t.LotType08).HasColumnName("LotType08");
            this.Property(t => t.LotValue08).HasColumnName("LotValue08");
            this.Property(t => t.DefaultIN08).HasColumnName("DefaultIN08");
            this.Property(t => t.DefaultOT08).HasColumnName("DefaultOT08");
            this.Property(t => t.SeqNo09).HasColumnName("SeqNo09");
            this.Property(t => t.Lot09).HasColumnName("Lot09");
            this.Property(t => t.LotVisible09).HasColumnName("LotVisible09");
            this.Property(t => t.LotRFVisible09).HasColumnName("LotRFVisible09");
            this.Property(t => t.LotMandatory09).HasColumnName("LotMandatory09");
            this.Property(t => t.LotMandatoryOT09).HasColumnName("LotMandatoryOT09");
            this.Property(t => t.InvCountLotRFVisible09).HasColumnName("InvCountLotRFVisible09");
            this.Property(t => t.LotType09).HasColumnName("LotType09");
            this.Property(t => t.LotValue09).HasColumnName("LotValue09");
            this.Property(t => t.DefaultIN09).HasColumnName("DefaultIN09");
            this.Property(t => t.DefaultOT09).HasColumnName("DefaultOT09");
            this.Property(t => t.SeqNo10).HasColumnName("SeqNo10");
            this.Property(t => t.Lot10).HasColumnName("Lot10");
            this.Property(t => t.LotVisible10).HasColumnName("LotVisible10");
            this.Property(t => t.LotRFVisible10).HasColumnName("LotRFVisible10");
            this.Property(t => t.LotMandatory10).HasColumnName("LotMandatory10");
            this.Property(t => t.LotMandatoryOT10).HasColumnName("LotMandatoryOT10");
            this.Property(t => t.SeqNo11).HasColumnName("SeqNo11");
            this.Property(t => t.Lot11).HasColumnName("Lot11");
            this.Property(t => t.LotVisible11).HasColumnName("LotVisible11");
            this.Property(t => t.LotRFVisible11).HasColumnName("LotRFVisible11");
            this.Property(t => t.LotMandatory11).HasColumnName("LotMandatory11");
            this.Property(t => t.LotMandatoryOT11).HasColumnName("LotMandatoryOT11");
            this.Property(t => t.SeqNo12).HasColumnName("SeqNo12");
            this.Property(t => t.Lot12).HasColumnName("Lot12");
            this.Property(t => t.LotVisible12).HasColumnName("LotVisible12");
            this.Property(t => t.LotRFVisible12).HasColumnName("LotRFVisible12");
            this.Property(t => t.LotMandatory12).HasColumnName("LotMandatory12");
            this.Property(t => t.LotMandatoryOT12).HasColumnName("LotMandatoryOT12");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
