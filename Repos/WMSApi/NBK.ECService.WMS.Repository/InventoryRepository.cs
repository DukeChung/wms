using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Repository
{
    public class InventoryRepository : CrudRepository, IInventoryRepository
    {
        /// <param name="dbContextProvider"></param>
        public InventoryRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 根据SkuSysId 获取invlotlocLpn 按照批次明细排序
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="pickRule"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnBySkuSysIdOrderByLotDetail(List<Guid> skuSysId, Guid wareHouseSysId, outbound ob, outboundrule outboundRule)
        {
            string strSql =
               "SELECT i.SysId as InvLotlocLpnSysId,lot.SysId AS InvLotSysId ,loc.SysId AS InvSkuLocSysId,i.Qty," +
               " i.AllocatedQty,i.PickedQty,i.FrozenQty,i.SkuSysId,i.Loc,i.Lot,i.Lpn,lot.LotAttr01,lot.LotAttr02,lot.LotAttr03,lot.LotAttr04" +
               ",lot.LotAttr05,lot.LotAttr06,lot.LotAttr07,lot.LotAttr08,lot.LotAttr09,lot.ExternalLot,lot.ProduceDate,lot.ExpiryDate,lot.ReceiptDate FROM invlotloclpn i LEFT JOIN invlot lot ON i.SkuSysId = lot.SkuSysId AND i.Lot = lot.Lot AND i.WareHouseSysId = lot.WareHouseSysId " +
               " LEFT JOIN invskuloc loc ON i.SkuSysId = loc.SkuSysId AND i.Loc = loc.Loc AND i.WareHouseSysId = loc.WareHouseSysId " +
               "  WHERE i.Qty > 0 AND i.SkuSysId in(" + skuSysId.GuidListToIds() + ") AND i.WareHouseSysId = @WareHouseSysId";

            string strOrderby = string.Empty;

            List<MySqlParameter> paraList = new List<MySqlParameter>();
            paraList.Add(new MySqlParameter($"@WareHouseSysId", wareHouseSysId));
            if (outboundRule != null)
            {
                //是否启用
                if (outboundRule.Status == true)
                {
                    //是否启用批次属性筛选
                    if (outboundRule.MatchingLotAttr == true && ob != null && ob.SysId != Guid.Empty)
                    {
                        strSql += string.Format(" and ifnull(lot.LotAttr01,'') =@LotAttr01");
                        strSql += string.Format(" and ifnull(lot.LotAttr02,'') =@LotAttr02 ");
                        paraList.Add(new MySqlParameter($"@LotAttr01", string.IsNullOrEmpty(ob.Channel) ? "" : ob.Channel));
                        paraList.Add(new MySqlParameter($"@LotAttr02", string.IsNullOrEmpty(ob.BatchNumber) ? "" : ob.BatchNumber));
                    }

                    #region 排序规则
                    //优先分配领料分拣货位
                    if (outboundRule.IsPickingSkuLoc == true)
                    {
                        strOrderby = string.Format(" ORDER BY i.loc = '{0}' desc ", PublicConst.PickingSkuLoc);
                    }

                    //先生产先出库
                    if (outboundRule.DeliverySortRules.HasValue && outboundRule.DeliverySortRules.Value == (int)DeliverySortRules.FirstProduceFirstOutbound)
                    {
                        if (outboundRule.IsPickingSkuLoc == true)
                        {
                            strOrderby += " ,lot.ProduceDate asc";
                        }
                        else
                        {
                            strOrderby = " ORDER BY lot.ProduceDate asc";
                        }
                    }
                    //后生产先出库
                    if (outboundRule.DeliverySortRules.HasValue && outboundRule.DeliverySortRules.Value == (int)DeliverySortRules.AfterProduceFirstOutbound)
                    {
                        if (outboundRule.IsPickingSkuLoc == true)
                        {
                            strOrderby += " ,lot.ProduceDate desc";
                        }
                        else
                        {
                            strOrderby = " ORDER BY lot.ProduceDate desc";
                        }
                    }
                    //先入库先出库
                    if (outboundRule.DeliverySortRules.HasValue && outboundRule.DeliverySortRules.Value == (int)DeliverySortRules.FirstReceiptFirstOutbound)
                    {
                        if (outboundRule.IsPickingSkuLoc == true)
                        {
                            strOrderby += " ,lot.ReceiptDate asc";
                        }
                        else
                        {
                            strOrderby = " ORDER BY lot.ReceiptDate asc";
                        }
                    }
                    #endregion
                }
            }

            return base.Context.Database.SqlQuery<InvLotLocLpnDto>(strSql + strOrderby, paraList.ToArray()).ToList();
        }


        /// <summary>
        /// 根据SkuSysId 获取invlotlocLpn 按照批次明细排序
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="pickRule"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnBySkuSysIdOrderByLotDetail(Guid skuSysId, string pickRule,
            Guid wareHouseSysId, outbound ob, outboundrule outboundRule)
        {

            string strSql =
                "SELECT i.SysId as InvLotlocLpnSysId,lot.SysId AS InvLotSysId ,loc.SysId AS InvSkuLocSysId,i.Qty," +
                " i.AllocatedQty,i.PickedQty,i.FrozenQty,i.SkuSysId,i.Loc,i.Lot,i.Lpn,lot.LotAttr01,lot.LotAttr02,lot.LotAttr03,lot.LotAttr04" +
                ",lot.LotAttr05,lot.LotAttr06,lot.LotAttr07,lot.LotAttr08,lot.LotAttr09,lot.ExternalLot,lot.ProduceDate,lot.ExpiryDate,lot.ReceiptDate FROM invlotloclpn i LEFT JOIN invlot lot ON i.SkuSysId = lot.SkuSysId AND i.Lot = lot.Lot  AND i.WareHouseSysId = lot.WareHouseSysId " +
                " LEFT JOIN invskuloc loc ON i.SkuSysId = loc.SkuSysId AND i.Loc = loc.Loc AND i.WareHouseSysId = loc.WareHouseSysId " +
                "  WHERE i.Qty > 0 AND i.SkuSysId = @SkuSysId AND i.WareHouseSysId = @WareHouseSysId";


            List<MySqlParameter> paraList = new List<MySqlParameter>();
            paraList.Add(new MySqlParameter("@SkuSysId", skuSysId));
            paraList.Add(new MySqlParameter($"@WareHouseSysId", wareHouseSysId));

            #region 匹配规则
            if (outboundRule != null)
            {
                //是否启用
                if (outboundRule.Status == true && outboundRule.MatchingLotAttr == true)
                {
                    //是否启用批次属性筛选
                    strSql += string.Format(" and ifnull(lot.LotAttr01,'') = @LotAttr01");
                    strSql += string.Format(" and ifnull(lot.LotAttr02,'') = @LotAttr02");

                    paraList.Add(new MySqlParameter($"@LotAttr01", string.IsNullOrEmpty(ob.Channel) ? "" : ob.Channel));
                    paraList.Add(new MySqlParameter($"@LotAttr02", string.IsNullOrEmpty(ob.BatchNumber) ? "" : ob.BatchNumber));

                }
            }
            #endregion

            #region 排序规则 

            string strOrderBy = "";
            switch (pickRule.Trim())
            {
                case PublicConst.PickRuleLO:
                    strOrderBy = " ORDER BY lot.ProduceDate,i.loc DESC";
                    break;
                case PublicConst.PickRuleFO:
                    strOrderBy = " ORDER BY lot.ProduceDate ASC ,i.loc DESC ";
                    break;
            }
            #endregion

            return base.Context.Database.SqlQuery<InvLotLocLpnDto>(strSql + strOrderBy, paraList.ToArray()).ToList();
        }


        /// <summary>
        /// 根据SkuSysId 获取invlotlocLpn 按照批次明细排序
        /// </summary>
        /// <param name="lotCode"></param>
        /// <param name="locCode"></param>
        /// <param name="lpnCode"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDto(Guid skuSysId, string lotCode, string locCode, string lpnCode, Guid wareHouseSysId)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on lpn.Lot equals lot.Lot
                        join sloc in Context.invskulocs on lpn.SkuSysId equals sloc.SkuSysId
                        where lpn.Loc == sloc.Loc && lpn.SkuSysId == lot.SkuSysId && lpn.Loc == locCode && lpn.Lot == lotCode && lpn.SkuSysId == skuSysId
                        && lpn.WareHouseSysId == wareHouseSysId && lot.WareHouseSysId == wareHouseSysId && sloc.WareHouseSysId == wareHouseSysId
                        select new { lpn, lot, sloc };
            return query.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
                InvSkuLocSysId = x.sloc.SysId,
            }).ToList();
        }

        /// <summary>
        /// 获取储区下 所有 invlotlocLpn 信息 (冻结业务使用)
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="lotCode"></param>
        /// <param name="locCode"></param>
        /// <param name="lpnCode"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDtoByZone(Guid zoneSysId, Guid wareHouseSysId)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on new { lpn.Lot, lpn.SkuSysId, lpn.WareHouseSysId } equals new { lot.Lot, lot.SkuSysId, lot.WareHouseSysId }
                        join sloc in Context.invskulocs on new { lpn.SkuSysId, lpn.WareHouseSysId, lpn.Loc } equals new { sloc.SkuSysId, sloc.WareHouseSysId, sloc.Loc }
                        join location in Context.locations on new { lpn.Loc, WarehouseSysId = lpn.WareHouseSysId } equals new { location.Loc, location.WarehouseSysId }
                        where lpn.WareHouseSysId == wareHouseSysId && location.ZoneSysId == zoneSysId
                            && !((from skuFrozen in Context.stockfrozen
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
            }).ToList();

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

        /// <summary>
        /// 获取储区货位下 所有 invlotlocLpn 信息
        /// </summary>
        /// <param name="zoneSysId"></param>
        /// <param name="loc"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDtoByZoneLoc(Guid zoneSysId, string loc, Guid wareHouseSysId)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on new { lpn.Lot, lpn.SkuSysId, lpn.WareHouseSysId } equals new { lot.Lot, lot.SkuSysId, lot.WareHouseSysId }
                        join sloc in Context.invskulocs on new { lpn.SkuSysId, lpn.WareHouseSysId, lpn.Loc } equals new { sloc.SkuSysId, sloc.WareHouseSysId, sloc.Loc }
                        join location in Context.locations on new { lpn.Loc, WarehouseSysId = lpn.WareHouseSysId } equals new { location.Loc, location.WarehouseSysId }
                        where lpn.WareHouseSysId == wareHouseSysId && location.ZoneSysId == zoneSysId && lpn.Loc == loc
                            && !((from skuFrozen in Context.stockfrozen
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
            }).ToList();

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

        /// <summary>
        /// 根据SkuSysId 获取invlotlocLpn 按照批次明细排序
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="lotCode"></param>
        /// <param name="locCode"></param>
        /// <param name="lpnCode"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDto(List<Guid> skuSysId, Guid warehouseSysId)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on lpn.Lot equals lot.Lot
                        join sloc in Context.invskulocs on lpn.SkuSysId equals sloc.SkuSysId
                        where lpn.Loc == sloc.Loc && lpn.SkuSysId == lot.SkuSysId
                            // && lpn.Loc == locCode && lpn.Lot == lotCode
                            // && lpn.SkuSysId == skuSysId
                            && lpn.WareHouseSysId == warehouseSysId
                            && lot.WareHouseSysId == warehouseSysId
                            && sloc.WareHouseSysId == warehouseSysId
                            && skuSysId.Contains(lpn.SkuSysId)
                        select new { lpn, lot, sloc };

            return query.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
                InvSkuLocSysId = x.sloc.SysId,
                Loc = x.lpn.Loc,
                Lot = x.lpn.Lot,
                SkuSysId = x.lpn.SkuSysId
            }).ToList();
        }


        /// <summary>
        ///  根据SkuSysId 获取invlotlocLpn 按照批次明细排序
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="lotCode"></param>
        /// <param name="locCode"></param>
        /// <param name="lpnCode"></param>
        /// <returns></returns>
        public InvLotLocLpnDto GetlotloclpnDto(Guid skuSysId, Guid warehouseSysId, string lotCode, string locCode, string lpnCode)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on lpn.Lot equals lot.Lot
                        join sloc in Context.invskulocs on lpn.SkuSysId equals sloc.SkuSysId
                        where lpn.Loc == sloc.Loc && lpn.SkuSysId == lot.SkuSysId
                            && lpn.Loc == locCode && lpn.Lot == lotCode
                            && lpn.SkuSysId == skuSysId
                            && lpn.WareHouseSysId == warehouseSysId
                            && lot.WareHouseSysId == warehouseSysId
                            && sloc.WareHouseSysId == warehouseSysId
                        select new { lpn, lot, sloc };
            return query.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
                InvSkuLocSysId = x.sloc.SysId,
            }).FirstOrDefault();
        }


        public Guid Getinvlotloclpn(Guid skuSysId, Guid warehouseSysId, string lotCode, string locCode, string lpnCode)
        {
            var query = from invlotloclpn in Context.invlotloclpns
                        where invlotloclpn.SkuSysId == skuSysId
                            && invlotloclpn.WareHouseSysId == warehouseSysId
                            && invlotloclpn.Lot == lotCode
                            && invlotloclpn.Loc == locCode
                        select invlotloclpn.SysId;
            return query.FirstOrDefault();
        }

        public List<Guid> Getinvlotloclpn(Guid skuSysId, Guid warehouseSysId, string locCode, string lpnCode)
        {
            var query = from invlotloclpn in Context.invlotloclpns
                        where invlotloclpn.SkuSysId == skuSysId
                            && invlotloclpn.WareHouseSysId == warehouseSysId
                        && invlotloclpn.Loc == locCode
                        select invlotloclpn.SysId;
            return query.ToList();
        }

        public Guid Getinvlot(Guid skuSysId, Guid warehouseSysId, string lotCode)
        {
            var query = from invlot in Context.invlots
                        where invlot.SkuSysId == skuSysId
                            && invlot.WareHouseSysId == warehouseSysId
                            && invlot.Lot == lotCode
                        select invlot.SysId;
            return query.FirstOrDefault();
        }

        public List<invlot> Getinvlotlist(Guid skuSysId, Guid warehouseSysId)
        {
            var query = from invlot in Context.invlots
                        where invlot.SkuSysId == skuSysId
                            && invlot.WareHouseSysId == warehouseSysId
                        select invlot;
            return query.ToList();
        }

        public Guid Getinvskuloc(Guid skuSysId, Guid warehouseSysId, string locCode)
        {
            var query = from invskuloc in Context.invskulocs
                        where invskuloc.SkuSysId == skuSysId
                            && invskuloc.WareHouseSysId == warehouseSysId
                            && invskuloc.Loc == locCode
                        select invskuloc.SysId;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 库存转移根据批次属性查询库存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        public List<InvLotLocLpnDto> GetlotloclpnDtoList(Guid skuSysId, Guid warehouseSysId, StockTransferDto stockTransferDto)
        {
            var query = from lpn in Context.invlotloclpns
                        join lot in Context.invlots on new { lpn.WareHouseSysId, lpn.SkuSysId, lpn.Lot } equals new { lot.WareHouseSysId, lot.SkuSysId, lot.Lot }
                        where lpn.SkuSysId == skuSysId && lpn.WareHouseSysId == warehouseSysId
                        select new { lpn, lot };

            //渠道
            if (string.IsNullOrEmpty(stockTransferDto.FromLotAttr01))
            {
                query = query.Where(x => x.lot.LotAttr01 == stockTransferDto.FromLotAttr01 || x.lot.LotAttr01 == null);
            }
            else
            {
                query = query.Where(x => x.lot.LotAttr01 == stockTransferDto.FromLotAttr01);
            }

            //批次
            if (string.IsNullOrEmpty(stockTransferDto.FromLotAttr02))
            {
                query = query.Where(x => x.lot.LotAttr02 == "" || x.lot.LotAttr02 == null);
            }
            else
            {
                query = query.Where(x => x.lot.LotAttr02 == stockTransferDto.FromLotAttr02);
            }

            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr03))
            //{
            //    query = query.Where(x => x.lot.LotAttr03 == stockTransferDto.FromLotAttr03);
            //}
            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr04))
            //{
            //    query = query.Where(x => x.lot.LotAttr04 == stockTransferDto.FromLotAttr04);
            //}
            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr05))
            //{
            //    query = query.Where(x => x.lot.LotAttr05 == stockTransferDto.FromLotAttr05);
            //}
            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr06))
            //{
            //    query = query.Where(x => x.lot.LotAttr06 == stockTransferDto.FromLotAttr06);
            //}
            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr07))
            //{
            //    query = query.Where(x => x.lot.LotAttr08== stockTransferDto.FromLotAttr08);
            //}
            //if (!string.IsNullOrEmpty(stockTransferDto.FromLotAttr09))
            //{
            //    query = query.Where(x => x.lot.LotAttr09 == stockTransferDto.FromLotAttr09);
            //}

            return query.Select(x => new InvLotLocLpnDto
            {
                InvLotLocLpnSysId = x.lpn.SysId,
                InvLotSysId = x.lot.SysId,
            }).ToList();
        }
    }
}