using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class PrePackCrudRepository : CrudRepository, IPrePackCrudRepository
    {
        public PrePackCrudRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        /// <summary>
        /// 分页获取与包装单
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>

        public Pages<PrePackListDto> GetPrePackByPage(PrePackQuery prePackQuery)
        {

            var query = from perPack in Context.prepacks
                        join pkDetail in Context.prepackdetails on perPack.SysId equals pkDetail.PrePackSysId
                        into p
                        from perPackDetail in p.DefaultIfEmpty()
                        join sku in Context.skus on perPackDetail.SkuSysId equals sku.SysId into t0
                        from p0 in t0.DefaultIfEmpty()
                        where perPack.WareHouseSysId == prePackQuery.WarehouseSysId
                        select new { perPack, p0 };

            if (!string.IsNullOrEmpty(prePackQuery.PrePackOrder))
            {
                prePackQuery.PrePackOrder = prePackQuery.PrePackOrder.Trim();
                query = query.Where(x => prePackQuery.PrePackOrder.Contains(x.perPack.PrePackOrder));
            }

            if (!string.IsNullOrEmpty(prePackQuery.ServiceStationName))
            {
                prePackQuery.ServiceStationName = prePackQuery.ServiceStationName.Trim();
                query = query.Where(x => x.perPack.ServiceStationName.Contains(prePackQuery.ServiceStationName));
            }
            if (!string.IsNullOrEmpty(prePackQuery.BatchNumber))
            {
                prePackQuery.BatchNumber = prePackQuery.BatchNumber.Trim();
                query = query.Where(x => prePackQuery.BatchNumber.Contains(x.perPack.BatchNumber));
            }

            if (!string.IsNullOrEmpty(prePackQuery.PrePackOrder))
            {
                prePackQuery.PrePackOrder = prePackQuery.PrePackOrder.Trim();
                query = query.Where(x => prePackQuery.PrePackOrder.Contains(x.perPack.PrePackOrder));
            }


            if (!string.IsNullOrEmpty(prePackQuery.OutboundOrder))
            {
                prePackQuery.OutboundOrder = prePackQuery.OutboundOrder.Trim();
                query = query.Where(x => prePackQuery.OutboundOrder.Contains(x.perPack.OutboundOrder));
            }

            if (prePackQuery.CreateDateFrom.HasValue)
            {
                query = query.Where(x => x.perPack.CreateDate > prePackQuery.CreateDateFrom.Value);
            }
            if (prePackQuery.CreateDateTo.HasValue)
            {
                query = query.Where(x => x.perPack.CreateDate < prePackQuery.CreateDateTo.Value);
            }
            if (!string.IsNullOrEmpty(prePackQuery.StorageLoc))
            {
                prePackQuery.StorageLoc = prePackQuery.StorageLoc.Trim();
                query = query.Where(x => x.perPack.StorageLoc.Contains(prePackQuery.StorageLoc));
            }

            if (!string.IsNullOrEmpty(prePackQuery.SkuName))
            {
                prePackQuery.SkuName = prePackQuery.SkuName.Trim();
                query = query.Where(x => x.p0.SkuName.Contains(prePackQuery.SkuName));
            }
            if (prePackQuery.Status.HasValue)
            {
                query = query.Where(x => x.perPack.Status == prePackQuery.Status.Value);
            }
            if (!string.IsNullOrEmpty(prePackQuery.SkuUPC))
            {
                prePackQuery.SkuUPC = prePackQuery.SkuUPC.Trim();
                query = query.Where(x => x.p0.UPC == prePackQuery.SkuUPC);
            }

            var prePackList = query.Select(x => new PrePackListDto()
            {
                SysId = x.perPack.SysId,
                PrePackOrder = x.perPack.PrePackOrder,
                StorageLoc = x.perPack.StorageLoc,
                ServiceStationName = x.perPack.ServiceStationName,
                BatchNumber = x.perPack.BatchNumber,
                OutboundOrder = x.perPack.OutboundOrder,
                Status = x.perPack.Status,
                CreateDate = x.perPack.CreateDate,
            }).Distinct();

            prePackQuery.iTotalDisplayRecords = prePackList.Count();
            prePackList = prePackList.OrderByDescending(p => p.CreateDate).Skip(prePackQuery.iDisplayStart).Take(prePackQuery.iDisplayLength);
            return ConvertPages<PrePackListDto>(prePackList, prePackQuery);
        }

        /// <summary>
        /// 获取预包装库存
        /// </summary>
        /// <param name="prePackSkuQuery"></param>
        /// <returns></returns>
        public Pages<PrePackSkuListDto> GetPrePackSkuByPage(PrePackSkuQuery prePackSkuQuery)
        {
            var query = from inv in Context.invlotloclpns
                        join invlot in Context.invlots on new { inv.Lot, inv.WareHouseSysId } equals new { invlot.Lot, invlot.WareHouseSysId }
                        join sku in Context.skus on inv.SkuSysId equals sku.SysId into t2
                        from p2 in t2.DefaultIfEmpty()
                        select new { inv, invlot, p2 };
            if (!string.IsNullOrEmpty(prePackSkuQuery.SkuCodeSearch))
            {
                prePackSkuQuery.SkuCodeSearch = prePackSkuQuery.SkuCodeSearch.Trim();
                query = query.Where(x => x.p2.SkuCode.Contains(prePackSkuQuery.SkuCodeSearch));
            }
            if (!string.IsNullOrEmpty(prePackSkuQuery.SkuNameSearch))
            {
                prePackSkuQuery.SkuNameSearch = prePackSkuQuery.SkuNameSearch.Trim();
                query = query.Where(x => x.p2.SkuName.Contains(prePackSkuQuery.SkuNameSearch));
            }
            if (!string.IsNullOrEmpty(prePackSkuQuery.UPCSearch))
            {
                prePackSkuQuery.UPCSearch = prePackSkuQuery.UPCSearch.Trim();
                query = query.Where(x => x.p2.UPC.Contains(prePackSkuQuery.UPCSearch));
            }
            var prePackSkuList = query.Select(x => new PrePackSkuListDto()
            {
                SysId = x.inv.SysId,
                SkuSysId = x.inv.SkuSysId,
                Qty = x.inv.Qty,
                Loc = x.inv.Loc,
                Lot = x.inv.Lot,
                SkuCode = x.p2.SkuCode,
                SkuName = x.p2.SkuName,
                UPC = x.p2.UPC,
                ReceiptDate = x.invlot.ReceiptDate,
                ProduceDate = x.invlot.ProduceDate,
                ExpiryDate = x.invlot.ExpiryDate,
                LotAttr01 = x.invlot.LotAttr01,
                LotAttr02 = x.invlot.LotAttr02,
                LotAttr03 = x.invlot.LotAttr03,
                LotAttr04 = x.invlot.LotAttr04,
                LotAttr05 = x.invlot.LotAttr05,
                LotAttr06 = x.invlot.LotAttr06,
                LotAttr07 = x.invlot.LotAttr07,
                LotAttr08 = x.invlot.LotAttr08,
                LotAttr09 = x.invlot.LotAttr09
            }).Distinct();

            prePackSkuQuery.iTotalDisplayRecords = prePackSkuList.Count();
            prePackSkuList = prePackSkuList.OrderByDescending(p => p.ReceiptDate).Skip(prePackSkuQuery.iDisplayStart).Take(prePackSkuQuery.iDisplayLength);
            return ConvertPages<PrePackSkuListDto>(prePackSkuList, prePackSkuQuery);
        }

        /// <summary>
        /// 获取预包装单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrePackSkuDto GetPrePackBySysId(Guid sysId)
        {
            var query = from pp in Context.prepacks
                        join wh in Context.warehouses on pp.OutboundSysId equals wh.SysId into t1
                        from p1 in t1.DefaultIfEmpty()
                        where pp.SysId == sysId
                        select new PrePackSkuDto()
                        {
                            SysId = pp.SysId,
                            Source = pp.Source,
                            StorageLoc = pp.StorageLoc,
                            PrePackOrder = pp.PrePackOrder,
                            Status = pp.Status,
                            OutboundOrder = pp.OutboundOrder,
                            CreateDate = pp.CreateDate,
                            OutboundSysName = p1.Name,
                            ServiceStationName = pp.ServiceStationName,
                            BatchNumber = pp.BatchNumber
                        };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取预包装单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<PrePackDetailDto> GetPrePackDetailBySysId(Guid sysId, string batchNumber)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT p.SysId,p.SkuSysId,s.SkuName,s.SkuDescr,s.UPC,s.OtherId ,s.SkuCode, IFNULL(i.Qty, 0) AS MaxQty, p.PreQty,p.Loc,p.Lot,p.Qty,p.ProduceDate,p.ExpiryDate,p2.PackCode,");
            sqlStr.Append(@"CASE WHEN ((((p2.InLabelUnit01 IS NOT NULL)  AND ((1 = p2.InLabelUnit01) AND 
                            (p2.InLabelUnit01 IS NOT NULL)))
                            AND (p2.FieldValue01 > 0))  AND (p2.FieldValue02 > 0)) 
                            THEN (u1.UOMCode) ELSE (u.UOMCode) END AS UOMCode");
            sqlStr.Append(" FROM  prepackdetail p");
            sqlStr.Append(" LEFT JOIN prepack p1 ON p.PrePackSysId = p1.SysId");
            sqlStr.Append(" LEFT JOIN (SELECT lot.SkuSysId, Lot.LotAttr02, SUM(Lot.Qty) Qty, lot.WarehouseSysId FROM invlot lot ");
            if (!string.IsNullOrEmpty(batchNumber))
            {
                sqlStr.AppendFormat(" WHERE lot.LotAttr02  ='{0}'", batchNumber);
            }
            sqlStr.Append(" group by lot.skuSysId, lot.WarehouseSysId ) AS i ON i.SkuSysId = p.SkuSysId AND i.WarehouseSysId = p1.WareHouseSysId ");
            sqlStr.Append(" LEFT JOIN sku s ON p.SkuSysId = s.SysId");
            sqlStr.Append(" LEFT JOIN uom u ON p.UOMSysId = u.SysId ");
            sqlStr.Append(" LEFT JOIN pack p2 ON p.PackSysId = p2.SysId");
            sqlStr.Append(" LEFT JOIN uom u1 ON p2.FieldUom02 = u1.SysId ");
            sqlStr.Append(" where p.PrePackSysId =@PrePackSysId;");


            var queryList = base.Context.Database.SqlQuery<PrePackDetailDto>(sqlStr.ToString()
                        , new MySqlParameter("@PrePackSysId", sysId));
            return queryList.ToList();

        }

        public List<PrePackDetailDto> GetPrePackDetailByOutboundSysId(Guid outboundSysId)
        {
            var query = from pd in Context.prepackdetails
                        join p in Context.prepacks on pd.PrePackSysId equals p.SysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join u in Context.uoms on pd.UOMSysId equals u.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where p.OutboundSysId == outboundSysId
                        group pd by new { s, pd.Loc, ti.UOMCode } into t1
                        select new PrePackDetailDto
                        {
                            UPC = t1.Key.s.UPC,
                            SkuName = t1.Key.s.SkuName,
                            SkuDescr = t1.Key.s.SkuDescr,
                            UomCode = t1.Key.UOMCode,
                            Qty = t1.Sum(p => p.Qty),
                        };
            return query.ToList();
        }

        /// <summary>
        /// 删除预包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool DeletePrePack(List<Guid> sysId)
        {
            if (sysId.Count > 0)
            {
                var sysIdList = string.Empty;
                foreach (var item in sysId)
                {
                    sysIdList += "'" + item.ToString() + "',";
                }
                if (!string.IsNullOrEmpty(sysIdList))
                {
                    sysIdList = sysIdList.Substring(0, sysIdList.Length - 1);
                }
                var sql = new StringBuilder();
                sql.AppendFormat("DELETE FROM prepackdetail WHERE PrePackSysId IN({0});", sysIdList);
                sql.AppendFormat("DELETE FROM prepack WHERE SysId IN({0}); ", sysIdList);
                Context.Database.ExecuteSqlCommand(sql.ToString());
            }
            return true;
        }
    }
}
