using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class FrozenRepository : CrudRepository, IFrozenRepository
    {
        public FrozenRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        public Pages<FrozenRequestSkuDto> GetFrozenRequestSkuByPage(FrozenRequestQuery request)
        {
            var query = from skuinv in Context.invlotloclpns
                        join sku in Context.skus on skuinv.SkuSysId equals sku.SysId
                        join pack in Context.packs on sku.PackSysId equals pack.SysId
                        join loc in Context.locations on new { skuinv.Loc, WarehouseSysId = skuinv.WareHouseSysId } equals new { loc.Loc, loc.WarehouseSysId }
                        //join skuLot in Context.invlots on new { skuinv.SkuSysId , skuinv.Lot , skuinv.WareHouseSysId } equals new { skuLot.SkuSysId, skuLot.Lot, skuLot.WareHouseSysId }
                        join zone in Context.zones on loc.ZoneSysId equals zone.SysId

                        join u1 in Context.uoms on pack.FieldUom01 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        join u2 in Context.uoms on pack.FieldUom02 equals u2.SysId into t4
                        from ti4 in t4.DefaultIfEmpty()

                        where skuinv.WareHouseSysId == request.WarehouseSysId
                            && skuinv.Qty > 0
                        select new { skuinv, pack, sku, loc, zone, ti3, ti4 };

            if (request.Type == (int)FrozenType.Zone)
            {
                if (request.Zone.HasValue)
                {
                    query = query.Where(p => p.zone.SysId == request.Zone.Value);
                }
            }
            else if (request.Type == (int)FrozenType.Location)
            {
                if (request.Zone.HasValue)
                {
                    query = query.Where(p => p.zone.SysId == request.Zone.Value);
                }

                if (!string.IsNullOrEmpty(request.Loc))
                {
                    query = query.Where(p => p.loc.Loc == request.Loc);
                }
            }
            else if (request.Type == (int)FrozenType.Sku || request.Type == (int)FrozenType.LocSku)
            {
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    request.UPC = request.UPC.Trim();
                    query = query.Where(p => p.sku.UPC == request.UPC);
                }

                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    request.SkuName = request.SkuName.Trim();
                    query = query.Where(p => p.sku.SkuName.Contains(request.SkuName));
                }
            }

            var response = query.Select(p => new FrozenRequestSkuDto()
            {
                SysId = p.sku.SysId,
                SkuSysId = p.skuinv.SkuSysId,
                WareHouseSysId = p.skuinv.WareHouseSysId,
                SkuCode = p.sku.SkuCode,
                SkuName = p.sku.SkuName,
                UPC = p.sku.UPC,
                UOMCode = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0 ? p.ti4.UOMCode : p.ti3.UOMCode,
                Loc = p.skuinv.Loc,
                Lot = p.skuinv.Lot,
                Lpn = p.skuinv.Lpn,
                //Channel = p.LotAttr01,
                Qty = (p.skuinv.Qty - p.skuinv.PickedQty - p.skuinv.AllocatedQty - p.skuinv.FrozenQty),
                DisplayQty = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true
                                  && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0
                                  ? Math.Round(((p.pack.FieldValue02.Value * (p.skuinv.Qty - p.skuinv.PickedQty - p.skuinv.AllocatedQty - p.skuinv.FrozenQty) * 1.00m) / p.pack.FieldValue01.Value), 3) : (p.skuinv.Qty - p.skuinv.PickedQty - p.skuinv.AllocatedQty - p.skuinv.FrozenQty),
                DisplayFrozenQty = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true
                                  && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0
                                  ? Math.Round(((p.pack.FieldValue02.Value * (p.skuinv.FrozenQty) * 1.00m) / p.pack.FieldValue01.Value), 3) : (p.skuinv.FrozenQty)
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderBy(p => p.Lot).Skip(request.iDisplayStart).Take(request.iDisplayLength);

            var result = ConvertPages(response, request);

            if (response.Count() > 0)
            {
                var skuList = result.TableResuls.aaData.Select(p => p.SkuSysId).Distinct().ToList();
                var lotList = result.TableResuls.aaData.Select(p => p.Lot).Distinct().ToList();

                var invLotList = this.GetQuery<invlot>(p => skuList.Contains(p.SkuSysId) && lotList.Contains(p.Lot) && p.WareHouseSysId == request.WarehouseSysId).ToList();

                var queryResult = from res in result.TableResuls.aaData
                                  join skuLot in invLotList on new { res.SkuSysId, res.Lot, res.WareHouseSysId } equals new { skuLot.SkuSysId, skuLot.Lot, skuLot.WareHouseSysId }
                                  select new FrozenRequestSkuDto()
                                  {
                                      SysId = res.SysId,
                                      SkuSysId = res.SkuSysId,
                                      WareHouseSysId = res.WareHouseSysId,
                                      SkuCode = res.SkuCode,
                                      SkuName = res.SkuName,
                                      UPC = res.UPC,
                                      UOMCode = res.UOMCode,
                                      Loc = res.Loc,
                                      Lot = res.Lot,
                                      Lpn = res.Lpn,
                                      Channel = skuLot.LotAttr01,
                                      Qty = res.Qty,
                                      DisplayQty = res.DisplayQty,
                                      DisplayFrozenQty = res.DisplayFrozenQty
                                  };
                result.TableResuls.aaData = queryResult.ToList();
            }

            return result;
        }

        public Pages<FrozenRequestSkuDto> GetFrozenDetailByPage(FrozenRequestQuery request)
        {
            var stockFrozen = this.GetQuery<stockfrozen>(p => p.SysId == request.StockFrozenSysId).FirstOrDefault();
            if (stockFrozen == null)
            {
                throw new Exception("请传入冻结请求sysid");
            }

            var query = from skuinv in Context.invlotloclpns
                        join lot in Context.invlots on new { skuinv.WareHouseSysId,skuinv.Lot,skuinv.SkuSysId} equals new { lot.WareHouseSysId,lot.Lot,lot.SkuSysId}
                        join sku in Context.skus on skuinv.SkuSysId equals sku.SysId
                        join pack in Context.packs on sku.PackSysId equals pack.SysId
                        join loc in Context.locations on new { skuinv.Loc, WarehouseSysId = skuinv.WareHouseSysId } equals new { loc.Loc, loc.WarehouseSysId }
                        join zone in Context.zones on loc.ZoneSysId equals zone.SysId

                        join u1 in Context.uoms on pack.FieldUom01 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        join u2 in Context.uoms on pack.FieldUom02 equals u2.SysId into t4
                        from ti4 in t4.DefaultIfEmpty()

                        where skuinv.WareHouseSysId == request.WarehouseSysId
                        select new { skuinv, lot, pack, sku, loc, zone, ti3, ti4 };

            if (stockFrozen.Type == (int)FrozenType.Zone)
            {
                query = query.Where(p => p.zone.SysId == stockFrozen.ZoneSysId);
            }
            else if (stockFrozen.Type == (int)FrozenType.Location)
            {
                query = query.Where(p => p.loc.Loc == stockFrozen.Loc);
            }

            var response = query.Select(p => new FrozenRequestSkuDto()
            {
                SysId = p.sku.SysId,
                SkuSysId = p.skuinv.SkuSysId,
                SkuCode = p.sku.SkuCode,
                SkuName = p.sku.SkuName,
                UPC = p.sku.UPC,
                UOMCode = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0 ? p.ti4.UOMCode : p.ti3.UOMCode,
                Loc = p.skuinv.Loc,
                Lot = p.skuinv.Lot,
                Lpn = p.skuinv.Lpn,
                Qty = p.skuinv.Qty,
                Channel = p.lot.LotAttr01,
                DisplayQty = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true
                                  && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0
                                  ? Math.Round(((p.pack.FieldValue02.Value * p.skuinv.Qty * 1.00m) / p.pack.FieldValue01.Value), 3) : p.skuinv.Qty,
                DisplayFrozenQty = p.pack.InLabelUnit01.HasValue && p.pack.InLabelUnit01.Value == true
                                  && p.pack.FieldValue01 > 0 && p.pack.FieldValue02 > 0
                                  ? Math.Round(((p.pack.FieldValue02.Value * (p.skuinv.FrozenQty) * 1.00m) / p.pack.FieldValue01.Value), 3) : (p.skuinv.FrozenQty)
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderBy(p => p.Lot).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }

        public Pages<FrozenListDto> GetFrozenRequestList(FrozenListQuery request)
        {
            var query = from frozen in Context.stockfrozen
                        join tempZone in Context.zones on frozen.ZoneSysId equals tempZone.SysId into t1
                        from zone in t1.DefaultIfEmpty()
                        join tempSku in Context.skus on frozen.SkuSysId equals tempSku.SysId into t2
                        from Sku in t2.DefaultIfEmpty()
                        where frozen.WarehouseSysId == request.WarehouseSysId
                        select new { frozen, zone.ZoneCode, Sku };

            if (request.Status.HasValue)
            {
                query = query.Where(p => p.frozen.Status == request.Status);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(p => p.frozen.Type == request.Type);
            }

            if (request.ZoneSysId.HasValue)
            {
                query = query.Where(p => p.frozen.ZoneSysId == request.ZoneSysId);
            }

            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.Sku.SkuName.Contains(request.SkuName));
            }

            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.Sku.UPC == request.UPC);
            }

            var response = query.Select(p => new FrozenListDto()
            {
                SysId = p.frozen.SysId,
                ZoneName = p.ZoneCode,
                Loc = p.frozen.Loc,
                Type = p.frozen.Type,
                Status = p.frozen.Status,
                CreateUserName = p.frozen.CreateUserName,
                CreateDate = p.frozen.CreateDate,
                Memo = p.frozen.Memo,
                SkuSysId = p.frozen.SkuSysId,
                SkuName = p.Sku.SkuName,
                UPC = p.Sku.UPC,
                FrozenSource = p.frozen.FrozenSource
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }

        /// <summary>
        /// 查询请求的商品中是否有正在分配或者拣货的库存
        /// </summary>
        /// <param name="skuList"></param>
        /// <returns></returns>
        public List<FrozenSkuDto> FilterUsedSkuList(List<Guid> skuList, Guid warehouseSysId)
        {
            var query = from skuLoc in Context.invskulocs
                        join sku in Context.skus on skuLoc.SkuSysId equals sku.SysId
                        where skuList.Contains(skuLoc.SkuSysId)
                            && skuLoc.WareHouseSysId == warehouseSysId
                            && (skuLoc.PickedQty > 0 || skuLoc.AllocatedQty > 0)
                        select new FrozenSkuDto
                        {
                            SkuSysId = skuLoc.SkuSysId,
                            UPC = sku.UPC,
                            SkuName = sku.SkuName,
                            Loc = skuLoc.Loc
                        };

            return query.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locSkuList"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<FrozenSkuDto> GetUsedLocSkuList(List<FrozenLocSkuDto> locSkuList, Guid warehouseSysId)
        {
            var skuList = locSkuList.Select(p => p.SkuSysId);
            var locList = locSkuList.Select(p => p.Loc);

            //由于关联条件拼接麻烦，先过滤出大概的相关商品和货位的数据（可能会有多余的垃圾数据）
            var firstQuery = (from skuLoc in Context.invlotloclpns
                              where skuList.Contains(skuLoc.SkuSysId)
                                  && locList.Contains(skuLoc.Loc)
                                  && skuLoc.WareHouseSysId == warehouseSysId
                              select skuLoc).Distinct().ToList();

            //内存中再进行匹配过滤
            var secondQuery = (from T1 in firstQuery
                               join T2 in locSkuList on new { T1.SkuSysId, T1.Loc } equals new { T2.SkuSysId, T2.Loc }
                               select T1).Distinct();

            var lots = firstQuery.Select(p => p.Lot).ToList();
            var lotList = this.GetQuery<invlot>(p => lots.Contains(p.Lot) && p.WareHouseSysId == warehouseSysId).ToList();

            var skuQuery = (from sku in Context.skus
                            where skuList.Contains(sku.SysId)
                            select sku).ToList();

            var locQuery = (from location in Context.locations
                            where locList.Contains(location.Loc)
                            select location).ToList();

            var thirdQuery = from skuLoc in secondQuery
                             join sku in skuQuery on skuLoc.SkuSysId equals sku.SysId
                             join location in locQuery on new { skuLoc.Loc, WarehouseSysId = skuLoc.WareHouseSysId } equals new { location.Loc, WarehouseSysId = location.WarehouseSysId }
                             join skuLot in lotList on skuLoc.Lot equals skuLot.Lot
                             select new FrozenSkuDto
                             {
                                 SkuSysId = skuLoc.SkuSysId,
                                 UPC = sku.UPC,
                                 SkuName = sku.SkuName,
                                 Loc = skuLoc.Loc,
                                 Lot = skuLoc.Lot,
                                 AllocatedQty = skuLoc.AllocatedQty,
                                 PickedQty = skuLoc.PickedQty,
                                 Qty = skuLoc.Qty,
                                 ZoneSysId = location.ZoneSysId.HasValue ? location.ZoneSysId.Value : Guid.Empty,
                                 LotAttr01 = skuLot.LotAttr01
                             };

            return thirdQuery.ToList();
        }

        /// <summary>
        /// 更新 商品 级别库存冻结
        /// </summary>
        /// <param name="skuList"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void UpdateFrozenQtyBySku(List<Guid> skuList, Guid warehouseSysId, int updateId, string updateName)
        {
            StringBuilder skusb = new StringBuilder();
            skuList.ForEach(p =>
            {
                skusb.Append($"'{p.ToString()}',");
            });
            string skus = skusb.ToString().TrimEnd(',');

            string strSqlInvLocLotLpn = $@" 
                UPDATE invlotloclpn i
                SET i.FrozenQty = i.Qty,
                    i.UpdateBy = @updateId,
                    i.UpdateDate = NOW(),
                    i.UpdateUserName = @updateName
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = @warehouseSysId;
                ";

            base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString(),
                new MySqlParameter("@updateId", updateId),
                new MySqlParameter("@updateName", updateName),
                new MySqlParameter("@warehouseSysId", warehouseSysId));


            string strSqlInvLot = $@" 
                UPDATE invlot i
                SET i.FrozenQty = i.Qty,
                    i.UpdateBy = {updateId},
                    i.UpdateDate = NOW(),
                    i.UpdateUserName = @updateName
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = '{warehouseSysId}';
                ";

            base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString(),
                new MySqlParameter("@updateName", updateName));


            string strSqlSkuLoc = $@"
                UPDATE invskuloc i
                SET i.FrozenQty = i.Qty,
                    i.UpdateBy = {updateId},
                    i.UpdateDate = NOW(),
                    i.UpdateUserName = @updateName
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = '{warehouseSysId}';
                ";

            base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString(),
                new MySqlParameter("@updateName", updateName));


            string strSqlInvLocLotLpnSearch = $@"
                SELECT *
                FROM invlotloclpn i
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = '{warehouseSysId}'
                    AND (i.AllocatedQty > 0 || i.PickedQty > 0);            
            ";

            var invlotloclpnList = base.Context.Database.SqlQuery<invlotloclpn>(strSqlInvLocLotLpnSearch).ToList();
            if (invlotloclpnList != null && invlotloclpnList.Count > 0)
            {
                throw new Exception("有库存正在业务处理中,无法冻结库存");
            }

            string strSqlInvLotSearch = $@"
                SELECT *
                FROM invlot i
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = '{warehouseSysId}'
                    AND (i.AllocatedQty > 0 || i.PickedQty > 0);            
            ";
            var invlotList = base.Context.Database.SqlQuery<invlot>(strSqlInvLotSearch).ToList();
            if (invlotList != null && invlotList.Count > 0)
            {
                throw new Exception("有库存正在业务处理中,无法冻结库存");
            }

            string strSqlSkuLocSearch = $@"
                SELECT *
                FROM invskuloc i
                WHERE i.SkuSysId IN ({skus})
                    AND i.WareHouseSysId = '{warehouseSysId}'
                    AND (i.AllocatedQty > 0 || i.PickedQty > 0);            
            ";
            var invskulocList = base.Context.Database.SqlQuery<invskuloc>(strSqlSkuLocSearch).ToList();
            if (invskulocList != null && invskulocList.Count > 0)
            {
                throw new Exception("有库存正在业务处理中,无法冻结库存");
            }
        }

        /// <summary>
        /// 更新 货位商品 级别 冻结库存 （冻结）
        /// </summary>
        /// <param name="locSkuList"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void UpdateFrozenQtyByLocSku(List<FrozenSkuDto> loclotSkuList, Guid warehouseSysId, int updateId, string updateName)
        {
            if (loclotSkuList.Count == 0)
                return;

            StringBuilder invlotBuilder = new StringBuilder();

            var intlotList = loclotSkuList.GroupBy(p => new { p.SkuSysId, p.Lot }).Select(p => (new { SkuSysId = p.Key.SkuSysId, Lot = p.Key.Lot, FrozenQty = p.Sum(q => q.Qty) })).ToList();
            intlotList.ForEach(p =>
            {
                invlotBuilder.AppendLine($@"
                    UPDATE invlot i
                    SET i.FrozenQty = i.FrozenQty + {p.FrozenQty},
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Lot = '{p.Lot}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");
            });

            StringBuilder invlotloclpnBuilder = new StringBuilder();
            StringBuilder invskulocBuilder = new StringBuilder();

            var locSkuList = loclotSkuList.GroupBy(p => new { p.SkuSysId, p.Loc }).Select(p => (new { SkuSysId = p.Key.SkuSysId, Loc = p.Key.Loc, FrozenQty = p.Sum(q => q.Qty) })).ToList();

            locSkuList.ForEach(p =>
            {
                invlotloclpnBuilder.AppendLine($@"
                    UPDATE invlotloclpn i
                    SET i.FrozenQty = i.Qty,
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Loc = '{p.Loc}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");

                invskulocBuilder.AppendLine($@"
                    UPDATE invskuloc i
                    SET i.FrozenQty = i.Qty,
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Loc = '{p.Loc}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");
            });

            base.Context.Database.ExecuteSqlCommand(invlotBuilder.ToString());
            base.Context.Database.ExecuteSqlCommand(invlotloclpnBuilder.ToString());
            base.Context.Database.ExecuteSqlCommand(invskulocBuilder.ToString());

        }

        /// <summary>
        /// 更新 货位商品 级别 冻结库存 （解冻）
        /// </summary>
        /// <param name="locSkuList"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void UpdateUnFrozenQtyByLocSku(List<FrozenSkuDto> loclotSkuList, Guid warehouseSysId, int updateId, string updateName)
        {
            if (loclotSkuList.Count == 0)
                return;

            StringBuilder invlotBuilder = new StringBuilder();

            var intlotList = loclotSkuList.GroupBy(p => new { p.SkuSysId, p.Lot }).Select(p => (new { SkuSysId = p.Key.SkuSysId, Lot = p.Key.Lot, FrozenQty = p.Sum(q => q.Qty) })).ToList();
            intlotList.ForEach(p =>
            {
                invlotBuilder.AppendLine($@"
                    UPDATE invlot i
                    SET i.FrozenQty = i.FrozenQty - {p.FrozenQty},
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Lot = '{p.Lot}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");
            });

            StringBuilder invlotloclpnBuilder = new StringBuilder();
            StringBuilder invskulocBuilder = new StringBuilder();

            var locSkuList = loclotSkuList.GroupBy(p => new { p.SkuSysId, p.Loc }).Select(p => (new { SkuSysId = p.Key.SkuSysId, Loc = p.Key.Loc, FrozenQty = p.Sum(q => q.Qty) })).ToList();

            locSkuList.ForEach(p =>
            {
                invlotloclpnBuilder.AppendLine($@"
                    UPDATE invlotloclpn i
                    SET i.FrozenQty = 0,
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Loc = '{p.Loc}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");

                invskulocBuilder.AppendLine($@"
                    UPDATE invskuloc i
                    SET i.FrozenQty = 0,
                        i.UpdateBy = {updateId},
                        i.UpdateDate = NOW(),
                        i.UpdateUserName = '{updateName}'
                    WHERE i.SkuSysId = '{p.SkuSysId}'
                        AND i.Loc = '{p.Loc}'
                        AND i.WareHouseSysId = '{warehouseSysId}';
                ");
            });

            base.Context.Database.ExecuteSqlCommand(invlotBuilder.ToString());
            base.Context.Database.ExecuteSqlCommand(invlotloclpnBuilder.ToString());
            base.Context.Database.ExecuteSqlCommand(invskulocBuilder.ToString());

        }

        /// <summary>
        /// 批量插入商品冻结请求
        /// </summary>
        /// <param name="request"></param>
        public void BatchInsertStockFrozenBySku(FrozenRequestDto request, FrozenSource frozenSource)
        {
            var existsFrozenRequest = this.GetQuery<stockfrozen>(p => request.SkuList.Contains(p.SkuSysId.Value) && p.WarehouseSysId == request.WarehouseSysId && p.Type == (int)FrozenType.Sku && p.Status == (int)FrozenStatus.Frozen).Select(p => p.SkuSysId).ToList();

            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("INSERT INTO stockfrozen(SysId, SkuSysId, Loc, ZoneSysId, Type, Status, WarehouseSysId, Lot, Lpn, CreateBy, CreateDate, CreateUserName, UpdateBy, UpdateDate, UpdateUserName, Memo, FrozenSource) VALUES ");

            bool hasNewData = false;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int i = 1;
            request.SkuList.ForEach(p =>
            {
                if (!existsFrozenRequest.Contains(p))
                {
                    hasNewData = true;
                    string paraNameMemo = $"@Memo{i}";
                    i++;
                    //由于 ZoneSysId 设计的时候 是非空的，所以对于sku 级别数据来说，ZoneSysId暂时赋值 一串0 的new guid，没什么影响
                    sqlsb.Append($@" (UUID(), '{p}', null, '{new Guid()}', {(int)FrozenType.Sku}, {(int)FrozenStatus.Frozen}, '{request.WarehouseSysId}', 
                    null, null, {request.CurrentUserId}, NOW(), '{request.CurrentDisplayName}', {request.CurrentUserId}, NOW(), '{request.CurrentDisplayName}', {paraNameMemo},{(int)frozenSource}),");

                    parameters.Add(new MySqlParameter(paraNameMemo, request.Memo));
                }
            });
            if (hasNewData)
            {
                var sql = sqlsb.ToString().TrimEnd(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sql, parameters.ToArray());
            }

        }

        /// <summary>
        /// 批量插入 货位商品 冻结请求
        /// </summary>
        /// <param name="locSkuList"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void BatchInsertStockFrozenByLocSku(List<FrozenSkuDto> locSkuList, Guid warehouseSysId, int updateId, string updateName, string memo, FrozenSource frozenSource)
        {
            if (locSkuList != null && locSkuList.Count > 0)
            {
                StringBuilder sqlsb = new StringBuilder();
                sqlsb.Append("INSERT INTO stockfrozen(SysId, SkuSysId, Loc, ZoneSysId, Type, Status, WarehouseSysId, Lot, Lpn, CreateBy, CreateDate, CreateUserName, UpdateBy, UpdateDate, UpdateUserName, Memo,FrozenSource) VALUES ");
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                int i = 1;
                locSkuList.ForEach(p =>
                {
                    string paraNameMemo = $"@Memo{i}";
                    i++;
                    //由于 ZoneSysId 设计的时候 是非空的，所以对于sku 级别数据来说，ZoneSysId暂时赋值 一串0 的new guid，没什么影响
                    sqlsb.Append($@" (UUID(), '{p.SkuSysId}', '{p.Loc}', '{new Guid()}', {(int)FrozenType.LocSku}, {(int)FrozenStatus.Frozen}, '{warehouseSysId}', 
                    null, null, {updateId}, NOW(), '{updateName}', {updateId}, NOW(), '{updateName}', {paraNameMemo},{(int)frozenSource}),");

                    parameters.Add(new MySqlParameter(paraNameMemo, memo));
                });

                var sql = sqlsb.ToString().TrimEnd(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sql, parameters.ToArray());
            }
        }

        /// <summary>
        /// 批量更新 货位商品 解冻请求
        /// </summary>
        /// <param name="locSkuList"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        /// <param name="memo"></param>
        public void BatchUpdateStockUnFrozenByLocSku(List<FrozenSkuDto> locSkuList, Guid warehouseSysId, int updateId, string updateName, string memo)
        {
            if (locSkuList != null && locSkuList.Count > 0)
            {
                StringBuilder sqlsb = new StringBuilder();

                locSkuList.ForEach(p =>
                {

                    sqlsb.AppendLine($@"UPDATE stockfrozen
                        SET Status = {(int)FrozenStatus.UnFrozen},
                            UpdateBy = {updateId},
                            UpdateDate = NOW(),
                            UpdateUserName = '{updateName}'
                        WHERE SkuSysId = '{p.SkuSysId}'
                            AND Loc = '{p.Loc}'
                            AND WarehouseSysId = '{warehouseSysId}'
                            AND Status = {(int)FrozenStatus.Frozen}
                            AND Type = {(int)FrozenType.LocSku};");

                });

                var sql = sqlsb.ToString().TrimEnd(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sql);
            }
        }

        /// <summary>
        /// 批量更新解冻请求状态
        /// </summary>
        public void BatchUnFrozenSku(List<Guid> skuList, Guid warehouseSysId)
        {
            StringBuilder skusb = new StringBuilder();
            skuList.ForEach(p =>
            {
                skusb.Append($"'{p.ToString()}',");
            });
            string skus = skusb.ToString().TrimEnd(',');

            string sql = $@"UPDATE stockfrozen
                        SET Status = {(int)FrozenStatus.UnFrozen}
                        WHERE SkuSysId IN ({skus})
                            AND WarehouseSysId = '{warehouseSysId}'
                            AND Status = {(int)FrozenStatus.Frozen}
                            AND Type = {(int)FrozenType.Sku};";

            base.Context.Database.ExecuteSqlCommand(sql);
        }

        /// <summary>
        /// 解冻库存，商品级别
        /// </summary>
        /// <param name="skuList"></param>
        /// <param name="warehouseSysId"></param>
        public List<InvLotLocLpnDto> GetBatchUpdateInventoryForUnFrozenSkuList(List<Guid> skuList, Guid warehouseSysId, int updateId, string updateName)
        {
            var updateQuery = from lpn in Context.invlotloclpns
                              join lot in Context.invlots on new { lpn.Lot, lpn.SkuSysId, lpn.WareHouseSysId } equals new { lot.Lot, lot.SkuSysId, lot.WareHouseSysId }
                              join sloc in Context.invskulocs on new { lpn.SkuSysId, lpn.WareHouseSysId, lpn.Loc } equals new { sloc.SkuSysId, sloc.WareHouseSysId, sloc.Loc }
                              join sku in Context.skus on lpn.SkuSysId equals sku.SysId
                              join location in Context.locations on new { lpn.Loc, WarehouseSysId = lpn.WareHouseSysId } equals new { location.Loc, location.WarehouseSysId }
                              where lpn.WareHouseSysId == warehouseSysId
                                  && skuList.Contains(lpn.SkuSysId)
                                  && location.Status == (int)LocationStatus.Normal
                                  && lpn.FrozenQty > 0
                              select new { lpn, lot, sloc };

            var updateList = updateQuery.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
                InvSkuLocSysId = x.sloc.SysId,
                Qty = x.lpn.Qty,
                AllocatedQty = x.lpn.AllocatedQty,
                PickedQty = x.lpn.PickedQty,
                FrozenQty = x.lpn.FrozenQty,
                Loc = x.lpn.Loc,
                Lot = x.lpn.Lot,
                Lpn = x.lpn.Lpn,
                LotAttr01 = x.lot.LotAttr01,
                SkuSysId = x.lpn.SkuSysId
            }).ToList();

            List<InvLotLocLpnDto> resultList = new List<InvLotLocLpnDto>();
            if (updateList.Count > 0)
            {
                //过滤 货位 商品级别数据
                var skuSysIdList = updateList.Select(p => p.SkuSysId).ToList();

                //#1.初步过滤
                var locskuFrozenList = this.GetQuery<stockfrozen>(p => p.WarehouseSysId == warehouseSysId
                    && p.Type == (int)FrozenType.LocSku && skuSysIdList.Contains(p.SkuSysId.Value) && p.Status == (int)FrozenStatus.Frozen).ToList();



                updateList.ForEach(p =>
                {
                    //#2.二次过滤
                    if (!locskuFrozenList.Exists(q => q.SkuSysId == p.SkuSysId && p.Loc == q.Loc))
                    {
                        resultList.Add(p);
                    }
                });
            }
            return resultList;
        }


        /// <summary>
        /// 解冻库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <param name="updateId"></param>
        /// <param name="updateName"></param>
        public void UpdateInvForUnFrozenRequest(List<InvLotLocLpnDto> updateInventoryList, int updateId, string updateName)
        {
            try
            {
                var strSqlInvLocLotLpn = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLocLotLpn.AppendFormat(" UPDATE invlotloclpn SET   invlotloclpn.FrozenQty = invlotloclpn.FrozenQty -{0},updateBy ={1},UpdateDate = now(),UpdateUserName = '{2}' ", info.FrozenQty, updateId, updateName);
                    strSqlInvLocLotLpn.AppendFormat(" where invlotloclpn.sysId='{0}' ; ", info.InvLotLocLpnSysId);

                }
                var invLotlocLpnResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLocLotLpn.ToString());
                if (updateInventoryList.Count() != invLotlocLpnResult)
                {
                    throw new Exception("批次货位库存发生变化,无法解冻库存");

                }

                var strSqlInvLot = new StringBuilder();
                foreach (var info in updateInventoryList)
                {
                    strSqlInvLot.AppendFormat(" UPDATE invlot SET invlot.FrozenQty = invlot.FrozenQty - {0} ,updateBy ={1},UpdateDate = now(),UpdateUserName ='{2}' ", info.FrozenQty, updateId, updateName);
                    strSqlInvLot.AppendFormat(" where invlot.sysId='{0}' ; ", info.InvLotSysId);

                }
                var invLotResult = base.Context.Database.ExecuteSqlCommand(strSqlInvLot.ToString());
                if (updateInventoryList.Count() != invLotResult)
                {
                    throw new Exception("批次库存发生变化,无法解冻库存");
                }

                var strSqlSkuLoc = new StringBuilder();


                foreach (var info in updateInventoryList)
                {
                    strSqlSkuLoc.AppendFormat(" UPDATE invSkuLoc SET   invSkuLoc.FrozenQty = invSkuLoc.FrozenQty -{0} ,updateBy ={1},UpdateDate = now(),UpdateUserName ='{2}' ", info.FrozenQty, updateId, updateName);
                    strSqlSkuLoc.AppendFormat(" where invSkuLoc.sysId='{0}' ;", info.InvSkuLocSysId);
                }
                var invSkuLocResult = base.Context.Database.ExecuteSqlCommand(strSqlSkuLoc.ToString());
                if (updateInventoryList.Count() != invSkuLocResult)
                {
                    throw new Exception("货位库存发生变化,无法解冻库存");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 商品冻结时， 获取需要更新的数据源，用于传给ECC做 占用
        /// </summary>
        /// <param name="skuSysIds"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDtoByFrozenSku(List<Guid> skuSysIds, Guid wareHouseSysId)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on new { lpn.Lot, lpn.SkuSysId, lpn.WareHouseSysId } equals new { lot.Lot, lot.SkuSysId, lot.WareHouseSysId }
                        join sloc in Context.invskulocs on new { lpn.SkuSysId, lpn.WareHouseSysId, lpn.Loc } equals new { sloc.SkuSysId, sloc.WareHouseSysId, sloc.Loc }
                        join location in Context.locations on new { lpn.Loc, WarehouseSysId = lpn.WareHouseSysId } equals new { location.Loc, location.WarehouseSysId }
                        where lpn.WareHouseSysId == wareHouseSysId
                            && skuSysIds.Contains(lpn.SkuSysId)
                            && location.Status == (int)LocationStatus.Normal    //过滤 货位 及 储区冻结
                            && !((from skuFrozen in Context.stockfrozen     //过滤 商品冻结
                                  where skuFrozen.Type == (int)FrozenType.Sku
                                      && skuFrozen.WarehouseSysId == wareHouseSysId
                                      && skuFrozen.Status == (int)FrozenStatus.Frozen
                                  select skuFrozen.SkuSysId.Value).ToList().Contains(lpn.SkuSysId))
                        select new { lpn, lot, sloc };
            var tempList = query.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
                InvSkuLocSysId = x.sloc.SysId,
                Qty = x.lpn.Qty,
                AllocatedQty = x.lpn.AllocatedQty,
                PickedQty = x.lpn.PickedQty,
                FrozenQty = x.lpn.FrozenQty,
                Loc = x.lpn.Loc,
                Lot = x.lpn.Lot,
                Lpn = x.lpn.Lpn,
                LotAttr01 = x.lot.LotAttr01,
                SkuSysId = x.lpn.SkuSysId
            }).Distinct().ToList();

            if (tempList.Count > 0)
            {
                //过滤 货位 商品级别数据
                var skuSysIdList = tempList.Select(p => p.SkuSysId).ToList();

                //#1.初步过滤
                var locskuFrozenList = this.GetQuery<stockfrozen>(p => p.WarehouseSysId == wareHouseSysId
                    && p.Type == (int)FrozenType.LocSku && skuSysIdList.Contains(p.SkuSysId.Value) && p.Status == (int)FrozenStatus.Frozen).ToList();

                List<InvLotLocLpnDto> resultList = new List<InvLotLocLpnDto>();

                tempList.ForEach(p =>
                {
                    //#2.二次过滤
                    if (!locskuFrozenList.Exists(q => q.SkuSysId == p.SkuSysId && p.Loc == q.Loc))
                    {
                        resultList.Add(p);
                    }
                });

                return resultList;
            }

            return tempList;
        }
    }
}
