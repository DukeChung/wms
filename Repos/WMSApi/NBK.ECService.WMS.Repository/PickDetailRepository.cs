using System;
using System.Collections.Generic;
using System.Linq;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System.Text;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMS.Repository
{
    public class PickDetailRepository : CrudRepository, IPickDetailRepository
    {
        /// <param name="dbContextProvider"></param>
        public PickDetailRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            var strSqlCount = new StringBuilder(@"select count(1) from pickDetail pd @Join
                                    where 1=1 ");

            var strSqlHead = @"SELECT pd.SysId,pd.PickDetailOrder,pd.Status,pd.SkuSysId,pd.SkuName,pd.SkuDescr,pd.Lot,pd.Loc,pd.Qty,
                                            pd.PickDate, 
                                            CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                                            THEN ROUND(p.FieldValue02 * (IFNULL(pd.Qty,0)/p.FieldValue01),3)
                                            ELSE IFNULL(pd.Qty,0) END AS DisplayQty,
                                            CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                                            THEN u.UomCode
                                            ELSE pd.UomCode END AS UomCode
                                            FROM (";

            var strSql = new StringBuilder(@"select pd.SysId,pd.PickDetailOrder,pd.Status,pd.SkuSysId,s.SkuName,s.SkuDescr,pd.Lot,pd.Loc,pd.Qty,u.UomCode,pd.PickDate,s.PackSysId                                 
                                    from pickDetail pd 
                                    @Join
                                    inner join sku s on s.sysid = pd.Skusysid
                                    inner join uom u on u.sysid = pd.UomSysId
                                    where 1=1 ");

            var strSqlBottom = ") pd inner join pack p ON p.SysId = pd.PackSysId left join uom u on u.SysId = p.FieldUom02 Order by pd.PickDetailOrder desc";

            strSqlCount = strSqlCount.AppendFormat(" and pd.WareHouseSysId = '{0}'", pickDetailQuery.WarehouseSysId);
            strSql = strSql.AppendFormat(" and pd.WareHouseSysId = '{0}'", pickDetailQuery.WarehouseSysId);
            if (!string.IsNullOrEmpty(pickDetailQuery.OutboundOrderSearch))
            {
                var join = @"inner join
                                 (select SysId, OutboundOrder from outbound
                                 union all
                                 select SysId, AssemblyOrder as OutboundOrder from assembly) o on o.SysId = pd.OutboundSysId";
                strSqlCount = strSqlCount.Replace("@Join", join);
                strSql = strSql.Replace("@Join", join);

                strSqlCount = strSqlCount.AppendFormat(" and o.OutboundOrder like '%{0}%'", pickDetailQuery.OutboundOrderSearch.Trim());
                strSql = strSql.AppendFormat(" and o.OutboundOrder like '%{0}%'", pickDetailQuery.OutboundOrderSearch.Trim());
            }
            else
            {
                strSqlCount = strSqlCount.Replace("@Join", "");
                strSql = strSql.Replace("@Join", "");
            }

            if (!string.IsNullOrEmpty(pickDetailQuery.PickDetailOrderSearch))
            {
                strSqlCount = strSqlCount.AppendFormat(" and pd.PickDetailOrder like '%{0}%'", pickDetailQuery.PickDetailOrderSearch.Trim());
                strSql = strSql.AppendFormat(" and pd.PickDetailOrder like '%{0}%'", pickDetailQuery.PickDetailOrderSearch.Trim());
            }

            if (pickDetailQuery.StatusSearch.HasValue)
            {
                strSqlCount = strSqlCount.AppendFormat(" and pd.Status = '{0}'", pickDetailQuery.StatusSearch);
                strSql = strSql.AppendFormat(" and pd.Status = '{0}'", pickDetailQuery.StatusSearch);
            }

            var rowCount = base.Context.Database.SqlQuery<int>(strSqlCount.ToString()).FirstOrDefault();

            strSql = strSql.Append(" Order by pd.PickDetailOrder desc LIMIT @Start, @Length");
            var queryList = base.Context.Database.SqlQuery<PickDetailListDto>(strSqlHead + strSql.ToString() + strSqlBottom
                , new MySqlParameter("@Start", pickDetailQuery.iDisplayStart)
                , new MySqlParameter("@Length", pickDetailQuery.iDisplayLength)).AsQueryable();

            pickDetailQuery.iTotalDisplayRecords = rowCount;
            return ConvertPages(queryList, pickDetailQuery);
        }

        public List<PickDetailListDto> GetSummaryPickDetailListDto(List<Guid?> pickDetailSysIds)
        {
            var query = from pd in (
                                  from pd in Context.pickdetails
                                  where pickDetailSysIds.Contains(pd.SysId)
                                  group new { pd.OutboundSysId, pd.PickDetailOrder } by new { pd.OutboundSysId, pd.PickDetailOrder } into g
                                  select new { g.Key.OutboundSysId, g.Key.PickDetailOrder })
                        group new { pd.PickDetailOrder } by new { pd.PickDetailOrder } into g
                        select new PickDetailListDto
                        {
                            PickDetailOrder = g.Key.PickDetailOrder,
                            OutboundCount = g.Count()
                        };

            return query.ToList();
        }

        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            var query = from ob in Context.outbounds
                        join obd in Context.outbounddetails on ob.SysId equals obd.OutboundSysId
                        join s in Context.skus on obd.SkuSysId equals s.SysId
                        select new { ob, obd, s };

            #region 查询条件
            query = query.Where(x => x.ob.WareHouseSysId == pickDetailQuery.WarehouseSysId);
            query = query.Where(x => x.ob.Status == (int)OutboundStatus.New);

            //UPC
            if (!string.IsNullOrEmpty(pickDetailQuery.SkuUPCSearch))
            {
                List<string> skuUPC = pickDetailQuery.SkuUPCSearch.Split(',').ToList();
                query = query.Where(x => skuUPC.Contains(x.s.UPC));
            }

            //承运商
            if (!string.IsNullOrEmpty(pickDetailQuery.CarrierNameSearch))
            {
                pickDetailQuery.CarrierNameSearch = pickDetailQuery.CarrierNameSearch.Trim();
                query = query.Where(x => x.ob.ShippingMethod.Contains(pickDetailQuery.CarrierNameSearch));
            }

            //服务站名称
            if (!string.IsNullOrEmpty(pickDetailQuery.ServiceStationNameSearch))
            {
                pickDetailQuery.CarrierNameSearch = pickDetailQuery.ServiceStationNameSearch.Trim();
                query = query.Where(x => x.ob.ServiceStationName.Contains(pickDetailQuery.ServiceStationNameSearch));
            }

            //商品数量
            if (!string.IsNullOrEmpty(pickDetailQuery.SkuQtyCountSearch))
            {
                pickDetailQuery.SkuQtyCountSearch = pickDetailQuery.SkuQtyCountSearch.Trim();
                var skuQtyCountSearch = Convert.ToInt32(pickDetailQuery.SkuQtyCountSearch);
                //小于
                if (pickDetailQuery.SkuQtyCountSymbol == (int)SymbolType.LessThan)
                {
                    query = query.Where(x => x.ob.TotalQty < skuQtyCountSearch);
                }

                //等于
                if (pickDetailQuery.SkuQtyCountSymbol == (int)SymbolType.Equal)
                {
                    query = query.Where(x => x.ob.TotalQty == skuQtyCountSearch);
                }

                //大于
                if (pickDetailQuery.SkuQtyCountSymbol == (int)SymbolType.GreaterThan)
                {
                    query = query.Where(x => x.ob.TotalQty > skuQtyCountSearch);
                }
            }

            //SKU数量
            if (!string.IsNullOrEmpty(pickDetailQuery.SkuTypeCountSearch))
            {
                var detailQuery = from obd in Context.outbounddetails
                                  group obd by new { obd.OutboundSysId } into g
                                  select new
                                  {
                                      OutboundSysId = g.Key.OutboundSysId,
                                      SkuTypeQty = g.Count()
                                  };

                var skuTypeCountSearch = Convert.ToInt32(pickDetailQuery.SkuTypeCountSearch);
                //小于
                if (pickDetailQuery.SkuTypeCountSymbol == (int)SymbolType.LessThan)
                {
                    detailQuery = detailQuery.Where(x => x.SkuTypeQty < skuTypeCountSearch);
                }

                //等于
                if (pickDetailQuery.SkuTypeCountSymbol == (int)SymbolType.Equal)
                {
                    detailQuery = detailQuery.Where(x => x.SkuTypeQty == skuTypeCountSearch);
                }

                //大于
                if (pickDetailQuery.SkuTypeCountSymbol == (int)SymbolType.GreaterThan)
                {
                    detailQuery = detailQuery.Where(x => x.SkuTypeQty > skuTypeCountSearch);
                }

                query = from q in query
                        join dq in detailQuery on q.ob.SysId equals dq.OutboundSysId
                        select new { ob = q.ob, obd = q.obd, s = q.s };
            }

            if (!string.IsNullOrEmpty(pickDetailQuery.OutboundOrderSearch))
            {
                pickDetailQuery.OutboundOrderSearch = pickDetailQuery.OutboundOrderSearch.Trim();
                query = query.Where(x => x.ob.OutboundOrder.Contains(pickDetailQuery.OutboundOrderSearch));
            }
            if (!string.IsNullOrEmpty(pickDetailQuery.ExternOrderSearch))
            {
                pickDetailQuery.ExternOrderSearch = pickDetailQuery.ExternOrderSearch.Trim();
                query = query.Where(x => x.ob.ExternOrderId.Contains(pickDetailQuery.ExternOrderSearch));
            }

            if (pickDetailQuery.OutboundTypeSearch.HasValue)
            {
                query = query.Where(x => x.ob.OutboundType == pickDetailQuery.OutboundTypeSearch.Value);
            }

            if (pickDetailQuery.OutboundTypeSearch.HasValue)
            {
                query = query.Where(x => x.ob.OutboundType == pickDetailQuery.OutboundTypeSearch.Value);
            }
            if (pickDetailQuery.StartOutboundDateSearch.HasValue)
            {
                query = query.Where(x => x.ob.OutboundDate >= pickDetailQuery.StartOutboundDateSearch.Value);
            }

            if (pickDetailQuery.EndOutboundDateSearch.HasValue)
            {
                query = query.Where(x => x.ob.OutboundDate <= pickDetailQuery.EndOutboundDateSearch.Value);
            }

            if (!string.IsNullOrEmpty(pickDetailQuery.ConsigneeNameSearch))
            {
                pickDetailQuery.ConsigneeNameSearch = pickDetailQuery.ConsigneeNameSearch.Trim();
                query = query.Where(x => x.ob.ConsigneeName == pickDetailQuery.ConsigneeNameSearch);
            }
            #endregion

            var pickDetail = query.Select(x => new PickOutboundListDto()
            {
                SysId = x.ob.SysId,
                OutboundOrder = x.ob.OutboundOrder,
                ExternOrderId = x.ob.ExternOrderId,
                OutboundType = x.ob.OutboundType,
                OutboundDate = x.ob.OutboundDate,
                AuditingDate = x.ob.AuditingDate,
                TotalQyt = x.ob.TotalQty,
                Remark = x.ob.Remark,
                ConsigneeName = x.ob.ConsigneeName,
                ConsigneeAddress = x.ob.ConsigneeAddress,
                ServiceStationName = x.ob.ServiceStationName
            }).Distinct();

            pickDetailQuery.iTotalDisplayRecords = pickDetail.Count();
            pickDetail = pickDetail.OrderByDescending(p => p.AuditingDate).Skip(pickDetailQuery.iDisplayStart).Take(pickDetailQuery.iDisplayLength);
            return ConvertPages<PickOutboundListDto>(pickDetail, pickDetailQuery);

        }

        public List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid?> outboundSysIds)
        {
            var query = from obd in Context.outbounddetails
                        where outboundSysIds.Contains(obd.OutboundSysId)
                        group obd by new { obd.OutboundSysId } into g
                        select new PickOutboundDetailListDto()
                        {
                            OutboundSysId = g.Key.OutboundSysId,
                            SkuTypeQty = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取拣货信息
        /// </summary>
        /// <param name="pickDetailOrder">拣货单号 或者 出库单号</param>
        /// <returns></returns>
        public List<PickDetailOperationDto> GetPickDetailOperationDto(string pickDetailOrder, Guid wareHouseSysId)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT p.SysId,p.WareHouseSysId,p.OutboundSysId,p.OutboundDetailSysId,p.PickDetailOrder,
                          p.PickDate, p.Status, p.SkuSysId, p.UOMSysId, p.PackSysId, p.Loc, p.Lot, p.Lpn
                          , p.Qty,o.OutboundOrder,s.SkuName,s.UPC,
                          s.SkuDescr,p1.UPC01,p1.UPC02,p1.UPC03,
                          p1.UPC04,p1.UPC05,p1.FieldValue01,p1.FieldValue02,
                          p1.FieldValue03,p1.FieldValue04,p1.FieldValue05
                          FROM pickdetail p 
                          LEFT JOIN outbound o ON p.OutboundSysId=o.SysId
                          LEFT JOIN sku s  ON p.SkuSysId = s.SysId
                          LEFT JOIN pack p1  ON s.PackSysId = p1.SysId
                          WHERE p.Status!=-999  and  p.WareHouseSysId = @WareHouseSysId and 
                          p.PickDetailOrder=@OutboundOrder ; ");
            var pickList = base.Context.Database.SqlQuery<PickDetailOperationDto>(sql.ToString(),
                new MySqlParameter("@OutboundOrder", pickDetailOrder),
                new MySqlParameter("@WareHouseSysId", wareHouseSysId)).AsQueryable().ToList();

            var outSql = new StringBuilder();
            outSql.Append(@" SELECT p.SysId,p.WareHouseSysId,p.OutboundSysId,p.OutboundDetailSysId,p.PickDetailOrder,
                          o.OutboundOrder,p.PickDate,p.Status,p.SkuSysId,p.UOMSysId,p.PackSysId,p.Loc,p.Lot,p.Lpn,
                          p.Qty,s.SkuName,s.UPC,
                          s.SkuDescr,p1.UPC01,p1.UPC02,p1.UPC03,
                          p1.UPC04,p1.UPC05,p1.FieldValue01,p1.FieldValue02,
                          p1.FieldValue03,p1.FieldValue04,p1.FieldValue05
                      FROM  pickdetail p
                      LEFT JOIN outbound o ON p.OutboundSysId=o.SysId
                      LEFT JOIN sku s  ON p.SkuSysId = s.SysId
                      LEFT JOIN pack p1  ON s.PackSysId = p1.SysId
                      WHERE p.Status!=-999  and o.OutboundOrder=@OutboundOrder AND o.WareHouseSysId = @WareHouseSysId");
            var outbound = base.Context.Database.SqlQuery<PickDetailOperationDto>(outSql.ToString(),
                new MySqlParameter("@OutboundOrder", pickDetailOrder),
                new MySqlParameter("@WareHouseSysId", wareHouseSysId)).AsQueryable().ToList();

            pickList.AddRange(outbound);
            return pickList;

            #region 屏蔽原始查询方法
            //var query = from pd in Context.pickdetails
            //            join ob in Context.outbounds on pd.OutboundSysId equals ob.SysId
            //            join s in Context.skus on pd.SkuSysId equals s.SysId
            //            join p in Context.packs on s.PackSysId equals p.SysId into t2
            //            from p1 in t2.DefaultIfEmpty()
            //            where ((pd.PickDetailOrder == pickDetailOrder && pd.WareHouseSysId == wareHouseSysId) || (ob.OutboundOrder == pickDetailOrder && ob.WareHouseSysId == wareHouseSysId))
            //            && pd.Status != (int)PickDetailStatus.Cancel
            //            select new PickDetailOperationDto
            //            {
            //                SysId = pd.SysId,
            //                WareHouseSysId = pd.WareHouseSysId,
            //                OutbounOrder = ob.OutboundOrder,
            //                OutboundSysId = pd.OutboundSysId,
            //                OutboundDetailSysId = pd.OutboundDetailSysId,
            //                PickDetailOrder = pd.PickDetailOrder,
            //                PickDate = pd.PickDate,
            //                Status = pd.Status,
            //                SkuSysId = pd.SkuSysId,
            //                UOMSysId = pd.UOMSysId,
            //                PackSysId = pd.PackSysId,
            //                Loc = pd.Loc,
            //                Lot = pd.Lot,
            //                Lpn = pd.Lpn,
            //                Qty = pd.Qty,
            //                SkuName = s.SkuName,
            //                UPC = s.UPC,
            //                SkuDescr = s.SkuDescr,
            //                UPC01 = p1.UPC01,
            //                UPC02 = p1.UPC02,
            //                UPC03 = p1.UPC03,
            //                UPC04 = p1.UPC04,
            //                UPC05 = p1.UPC05,
            //                FieldValue01 = p1.FieldValue01,
            //                FieldValue02 = p1.FieldValue02,
            //                FieldValue03 = p1.FieldValue03,
            //                FieldValue04 = p1.FieldValue04,
            //                FieldValue05 = p1.FieldValue05,
            //            };
            //return query.ToList();
            #endregion
        }

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        public List<PickingOperationDetail> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery)
        {
            var query = from pd in Context.pickdetails
                        join o in Context.outbounds on pd.OutboundSysId equals o.SysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        where pd.PickDetailOrder.Equals(pickingOperationQuery.PickDetailOrder, StringComparison.OrdinalIgnoreCase)
                        select new PickingOperationDetail()
                        {
                            SysId = pd.SysId,
                            PickDetailOrder = pd.PickDetailOrder,
                            OutboundOrder = o.OutboundOrder,
                            SkuSysId = pd.SkuSysId,
                            SkuName = s.SkuName,
                            UPC = s.UPC,
                            SkuDescr = s.SkuDescr,
                            Loc = pd.Loc,
                            Lot = pd.Lot,
                            Qty = pd.Qty.HasValue ? pd.Qty.Value : 0,
                            DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                            ? Math.Round(((p.FieldValue02.Value * (pd.Qty.HasValue ? pd.Qty.Value : 0) * 1.00m) / p.FieldValue01.Value), 3) : (pd.Qty.HasValue ? pd.Qty.Value : 0),
                            PickedQty = pd.PickedQty != 0 ? pd.PickedQty : (pd.Qty.HasValue ? pd.Qty.Value : 0),
                            DisplayPickedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                            ? Math.Round(((p.FieldValue02.Value * (pd.PickedQty != 0 ? pd.PickedQty : (pd.Qty.HasValue ? pd.Qty.Value : 0)) * 1.00m) / p.FieldValue01.Value), 3) : (pd.PickedQty != 0 ? pd.PickedQty : (pd.Qty.HasValue ? pd.Qty.Value : 0))
                        };
            return query.Distinct().OrderBy(p => new { p.Loc, p.UPC, p.Lot }).ToList();
        }

    }
}