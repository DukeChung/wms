using Microsoft.Extensions.Logging;
using Schubert.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Dto.DataSync;
using WMSBussinessApi.Repository;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using System.Text;

namespace WMSBussinessApi.RepositoryImp
{
    public class DataSyncRep : BaseRep, IDataSyncRep
    {
        public void SyncCreateSku(SkuDto sku)
        {
            //string query1 = "INSERT INTO Book(Name)VALUES(@name)";
            //Connection.Execute(query1, new { name = "C#本质论" });
            //var skuList = Connection.Query<SkuDto>("SELECT s.SysId,s.SkuName,s.UPC FROM sku s LIMIT 10;");


            string sql = @"
INSERT INTO sku(
SysId, SkuCode, SkuName, SkuClassSysId, ShelfLifeCodeType, 
SkuDescr, ShelfLifeOnReceiving, ShelfLife, PackSysId, DaysToExpire, 
LotTemplateSysId, ShelfLifeIndicator, Length, Width, Height, 
Cube, NetWeight, GrossWeight, CostPrice, SalePrice, 
Fresh, FragileArticles, Image, Color, Style, 
CreateBy, CreateDate, UpdateBy, UpdateDate, IsActive, 
OtherId, UPC, OtherUPC1, OtherUPC2, OtherUPC3, 
OtherUPC4, IsInvoices, IsRefunds, IsMaterial, RecommendLoc, CreateUserName, UpdateUserName)
VALUES (
@SysId, @SkuCode, @SkuName, @SkuClassSysId, @ShelfLifeCodeType, 
@SkuDescr, @ShelfLifeOnReceiving,@ShelfLife, @PackSysId, @DaysToExpire, 
@LotTemplateSysId, @ShelfLifeIndicator, @Length, @Width, @Height, 
@Cube, @NetWeight, @GrossWeight, @CostPrice, @SalePrice, 
@Fresh, @FragileArticles, @Image, @Color, @Style, 
@CreateBy, @CreateDate, @UpdateBy, @UpdateDate, @IsActive, 
@OtherId, @UPC, @OtherUPC1, @OtherUPC2, @OtherUPC3, 
@OtherUPC4, @IsInvoices, @IsRefunds, @IsMaterial, @RecommendLoc, @CreateUserName, @UpdateUserName);
";

            Connection.Execute(sql, new {
                SysId = sku.SysId, SkuCode = sku.SkuCode, SkuName = sku.SkuName,SkuClassSysId = sku.SkuClassSysId , ShelfLifeCodeType = sku.ShelfLifeCodeType,
                SkuDescr = sku.SkuDescr, ShelfLifeOnReceiving = sku.ShelfLifeOnReceiving, ShelfLife = sku.ShelfLife , PackSysId = sku.PackSysId,DaysToExpire = sku.DaysToExpire,
                LotTemplateSysId = sku.LotTemplateSysId, ShelfLifeIndicator = sku.ShelfLifeIndicator,Length = sku.Length, Width = sku.Width , Height = sku.Height,
                Cube = sku.Cube, NetWeight = sku.NetWeight,GrossWeight = sku.GrossWeight,CostPrice = sku.CostPrice,SalePrice = sku.SalePrice,
                Fresh = sku.Fresh , FragileArticles = sku.FragileArticles, Image = sku.Image, Color = sku.Color, Style = sku.Style,
                CreateBy = sku.CreateBy, CreateDate = sku.CreateDate,UpdateBy = sku.UpdateBy, UpdateDate = sku.UpdateDate, IsActive = sku.IsActive,
                OtherId = sku.OtherId, UPC = sku.UPC, OtherUPC1 = sku.OtherUPC1, OtherUPC2 = sku.OtherUPC2, OtherUPC3 = sku.OtherUPC3,
                OtherUPC4 = sku.OtherUPC4, IsInvoices = sku.IsInvoices, IsRefunds = sku.IsRefunds,IsMaterial = sku.IsMaterial,RecommendLoc = sku.RecommendLoc,CreateUserName = sku.CreateUserName,UpdateUserName = sku.UpdateUserName
            });
        }


        public void SyncUpdateSku(SkuDto sku)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendLine("UPDATE sku SET ");
            sbSql.AppendLine($" SkuName = '{sku.SkuName}', ");
            sbSql.AppendLine($" SkuClassSysId = '{sku.SkuClassSysId}', ");
            sbSql.AppendLine($" SkuDescr = '{sku.SkuDescr}', ");
            sbSql.AppendLine($" DaysToExpire = {sku.DaysToExpire ?? 0}, ");
            sbSql.AppendLine($" Length = {sku.Length ?? decimal.Zero}, ");
            sbSql.AppendLine($" Width = {sku.Width ?? decimal.Zero}, ");
            sbSql.AppendLine($" Height = {sku.Height ?? decimal.Zero}, ");
            sbSql.AppendLine($" Cube = {sku.Cube ?? decimal.Zero}, ");
            if (sku.NetWeight != null)
                sbSql.AppendLine($" NetWeight = {sku.NetWeight / 1000}, ");
            else
                sbSql.AppendLine(" NetWeight = 0 , ");

            if (sku.GrossWeight != null)
                sbSql.AppendLine($" GrossWeight = {sku.GrossWeight / 1000}, ");
            else
                sbSql.AppendLine(" GrossWeight = 0 , ");

            sbSql.AppendLine($" SalePrice = {sku.SalePrice }, ");
            sbSql.AppendLine($" IsInvoices = {sku.IsInvoices }, ");
            sbSql.AppendLine($" IsRefunds = {sku.IsRefunds }, ");
            sbSql.AppendLine($" IsMaterial = {sku.IsMaterial }, ");
            sbSql.AppendLine($" Image = '{sku.Image }', ");
            sbSql.AppendLine($" UPC = '{sku.UPC }', ");
            sbSql.AppendLine($" UpdateBy = {sku.UpdateBy }, ");
            sbSql.AppendLine($" UpdateDate = '{sku.UpdateDate }' ");

            sbSql.AppendLine(" WHERE SysId = @SysId ");

            Connection.Execute(sbSql.ToString(), new { SysId = sku.SysId });
        }

        public void SyncCreatePack(PackDto pack)
        {
            string sql = @"
INSERT INTO pack(
SysId, PackCode, Descr, 
FieldUom01, FieldValue01, Cartonize01, Replenish01, InLabelUnit01, OutLabelUnit01, 
FieldUom02, FieldValue02, Cartonize02, Replenish02, InLabelUnit02, OutLabelUnit02, 
FieldUom03, FieldValue03, Cartonize03, Replenish03, InLabelUnit03, OutLabelUnit03, 
QueryLabelUnit01, QueryLabelUnit02, QueryLabelUnit03, UPC01, UPC02, UPC03, 
FieldUom04, FieldValue04, UPC04, FieldUom05, FieldValue05, UPC05, 
Source, CoefficientId01, CoefficientId02, CoefficientId03, CoefficientId04, CoefficientId05)
VALUES (
@SysId, @PackCode, @Descr, 
@FieldUom01, @FieldValue01, @Cartonize01, @Replenish01, @InLabelUnit01, @OutLabelUnit01, 
@FieldUom02, @FieldValue02, @Cartonize02, @Replenish02, @InLabelUnit02, @OutLabelUnit02, 
@FieldUom03, @FieldValue03, @Cartonize03, @Replenish03, @InLabelUnit03, @OutLabelUnit03, 
@QueryLabelUnit01, @QueryLabelUnit02, @QueryLabelUnit03, @UPC01, @UPC02, @UPC03, 
@FieldUom04, @FieldValue04, @UPC04, @FieldUom05, @FieldValue05, @UPC05, 
@Source, @CoefficientId01, @CoefficientId02, @CoefficientId03, @CoefficientId04, @CoefficientId05);            
";

            Connection.Execute(sql, new
            {
                pack.SysId, pack.PackCode,pack.Descr,
                pack.FieldUom01, pack.FieldValue01, pack.Cartonize01, pack.Replenish01, pack.InLabelUnit01, pack.OutLabelUnit01,
                pack.FieldUom02, pack.FieldValue02, pack.Cartonize02, pack.Replenish02, pack.InLabelUnit02, pack.OutLabelUnit02,
                pack.FieldUom03, pack.FieldValue03, pack.Cartonize03, pack.Replenish03, pack.InLabelUnit03, pack.OutLabelUnit03,
                pack.QueryLabelUnit01, pack.QueryLabelUnit02, pack.QueryLabelUnit03, pack.UPC01, pack.UPC02, pack.UPC03, 
                pack.FieldUom04, pack.FieldValue04, pack.UPC04, pack.FieldUom05, pack.FieldValue05, pack.UPC05,
                pack.Source, pack.CoefficientId01, pack.CoefficientId02, pack.CoefficientId03, pack.CoefficientId04, pack.CoefficientId05
            });
        }

        public void SyncUpdatePack(PackDto pack)
        {
            string sql = @"
UPDATE pack SET 
    PackCode = @PackCode, 
    Descr = @Descr, 
    FieldUom01 = @FieldUom01, 
    FieldValue01 = @FieldValue01, 
    Cartonize01 = @Cartonize01, 
    Replenish01 = @Replenish01, 
    InLabelUnit01 = @InLabelUnit01, 
    OutLabelUnit01 = @OutLabelUnit01, 
    FieldUom02 = @FieldUom02, 
    FieldValue02 = @FieldValue02, 
    Cartonize02 = @Cartonize02, 
    Replenish02 = @Replenish02, 
    InLabelUnit02 = @InLabelUnit02, 
    OutLabelUnit02 = @OutLabelUnit02, 
    FieldUom03 = @FieldUom03, 
    FieldValue03 = @FieldValue03, 
    Cartonize03 = @Cartonize03, 
    Replenish03 = @Replenish03, 
    InLabelUnit03 = @InLabelUnit03, 
    OutLabelUnit03 = @OutLabelUnit03, 
    QueryLabelUnit01 = @QueryLabelUnit01, 
    QueryLabelUnit02 = @QueryLabelUnit02, 
    QueryLabelUnit03 = @QueryLabelUnit03, 
    UPC01 = @UPC01, 
    UPC02 = @UPC02, 
    UPC03 = @UPC03, 
    FieldUom04 = @FieldUom04, 
    FieldValue04 = @FieldValue04, 
    UPC04 = @UPC04, 
    FieldUom05 = @FieldUom05, 
    FieldValue05 = @FieldValue05, 
    UPC05 = @UPC05, 
    CoefficientId01 = @CoefficientId01, 
    CoefficientId02 = @CoefficientId02, 
    CoefficientId03 = @CoefficientId03, 
    CoefficientId04 = @CoefficientId04, 
    CoefficientId05 = @CoefficientId05
WHERE SysId = @SysId
";

            Connection.Execute(sql, new {
                pack.SysId, pack.PackCode,pack.Descr,
                pack.FieldUom01, pack.FieldValue01, pack.Cartonize01, pack.Replenish01, pack.InLabelUnit01, pack.OutLabelUnit01,
                pack.FieldUom02, pack.FieldValue02, pack.Cartonize02, pack.Replenish02, pack.InLabelUnit02, pack.OutLabelUnit02,
                pack.FieldUom03, pack.FieldValue03, pack.Cartonize03, pack.Replenish03, pack.InLabelUnit03, pack.OutLabelUnit03,
                pack.QueryLabelUnit01, pack.QueryLabelUnit02, pack.QueryLabelUnit03, pack.UPC01, pack.UPC02, pack.UPC03, 
                pack.FieldUom04, pack.FieldValue04, pack.UPC04, pack.FieldUom05, pack.FieldValue05, pack.UPC05,
                pack.CoefficientId01, pack.CoefficientId02, pack.CoefficientId03, pack.CoefficientId04, pack.CoefficientId05});
        }

        public void SyncDeletePack(List<Guid> sysIdList)
        {
            StringBuilder sysIdsb = new StringBuilder();
            sysIdList.ForEach(p => {
                sysIdsb.Append($"'{p.ToString()}',");
            });
            string sysIds = sysIdsb.ToString().TrimEnd(',');

            string sql = $@"
                DELETE FROM pack WHERE SysId IN ({sysIds});    
            ";

            Connection.Execute(sql);
        }

        public void SyncCreateSyscode(SyscodeDto syscode)
        {
            string sql = @"
INSERT INTO syscode(SysId, SysCodeType, Descr)
  VALUES (@SysId, @SysCodeType, @Descr);
";
            Connection.Execute(sql, new
            {
                syscode.SysId,
                syscode.SysCodeType,
                syscode.Descr
            });
        }

        public void SyncCreateSyscodeDetail(SyscodeDetailDto syscodedetail)
        {
            string sql = @"
INSERT INTO syscodedetail(
SysId, SysCodeSysId, SeqNo, Code, Descr, 
CreateBy, CreateDate, UpdateBy, UpdateDate, IsActive, 
CreateUserName, UpdateUserName)
  VALUES (
@SysId, @SysCodeSysId, @SeqNo,@Code, @Descr, 
@CreateBy, @CreateDate, @UpdateBy, @UpdateDate, @IsActive, 
@CreateUserName, @UpdateUserName);
";

            Connection.Execute(sql, new {
                syscodedetail.SysId,
                syscodedetail.SysCodeSysId,
                syscodedetail.SeqNo,
                syscodedetail.Code,
                syscodedetail.Descr,
                syscodedetail.CreateBy,
                syscodedetail.CreateDate,
                syscodedetail.UpdateBy,
                syscodedetail.UpdateDate,
                syscodedetail.IsActive,
                syscodedetail.CreateUserName,
                syscodedetail.UpdateUserName
            });
        }
    }
}
