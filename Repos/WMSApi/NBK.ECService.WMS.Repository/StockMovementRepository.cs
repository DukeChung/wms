using Abp.EntityFramework;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Repository
{
    public class StockMovementRepository : CrudRepository, IStockMovementRepository
    {
        public StockMovementRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        public Pages<StockMovementSkuDto> GetStockMovementSkuListByPaging(StockMovementSkuQuery stockMovementSkuQuery)
        {
            var query = from s in Context.skus
                        join il in Context.invlotloclpns on s.SysId equals il.SkuSysId
                        join pk in Context.packs on s.PackSysId equals pk.SysId
                        where (il.Qty - il.AllocatedQty - il.PickedQty) != 0
                        select new
                        {
                            s,
                            il,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02
                        };

            query = query.Where(p => p.il.WareHouseSysId == stockMovementSkuQuery.WarehouseSysId);

            if (!stockMovementSkuQuery.SkuNameSearch.IsNull())
            {
                stockMovementSkuQuery.SkuNameSearch = stockMovementSkuQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(stockMovementSkuQuery.SkuNameSearch));
            }
            if (!stockMovementSkuQuery.SkuUPCSearch.IsNull())
            {
                stockMovementSkuQuery.SkuUPCSearch = stockMovementSkuQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.s.UPC.Equals(stockMovementSkuQuery.SkuUPCSearch, StringComparison.OrdinalIgnoreCase));
            }
            if (!stockMovementSkuQuery.LocSearch.IsNull())
            {
                stockMovementSkuQuery.LocSearch = stockMovementSkuQuery.LocSearch.Trim();
                query = query.Where(p => p.il.Loc == stockMovementSkuQuery.LocSearch);
            }
            if (!stockMovementSkuQuery.LotSearch.IsNull())
            {
                stockMovementSkuQuery.LotSearch = stockMovementSkuQuery.LotSearch.Trim();
                query = query.Where(p => p.il.Lot == stockMovementSkuQuery.LotSearch);
            }
            var skus = query.Select(p => new StockMovementSkuDto()
            {
                SysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                UPC = p.s.UPC,
                Loc = p.il.Loc,
                Qty = p.il.Qty - p.il.AllocatedQty - p.il.PickedQty,
                Lot = p.il.Lot,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * (p.il.Qty - p.il.AllocatedQty - p.il.PickedQty) * 1.00m) / p.FieldValue01.Value), 3) : (p.il.Qty - p.il.AllocatedQty - p.il.PickedQty)

            }).Distinct();
            stockMovementSkuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderByDescending(p => p.SkuCode).Skip(stockMovementSkuQuery.iDisplayStart).Take(stockMovementSkuQuery.iDisplayLength);
            return ConvertPages(skus, stockMovementSkuQuery);
        }

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        public StockMovementDto GetStockMovement(Guid skuSysId, string loc, string lot, Guid wareHouseSysId)
        {
            var query = from s in Context.skus
                        join il in Context.invlotloclpns on s.SysId equals il.SkuSysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        where s.SysId == skuSysId && il.Loc == loc && il.Lot == lot && il.WareHouseSysId == wareHouseSysId
                        select new { s, il, p };
            StockMovementDto stockMovementDto = query.Select(p => new StockMovementDto()
            {
                SkuName = p.s.SkuName,
                UPC = p.s.UPC,
                Lot = p.il.Lot,
                Qty = p.il.Qty - p.il.AllocatedQty - p.il.PickedQty,
                FromLoc = p.il.Loc,
                WareHouseSysId = p.il.WareHouseSysId,
                Status = (int)StockMovementStatus.New,
                SkuSysId = p.s.SysId,
                Lpn = p.il.Lpn,
                DisplayQty = p.p.InLabelUnit01.HasValue && p.p.InLabelUnit01.Value == true
                            && p.p.FieldValue01 > 0 && p.p.FieldValue02 > 0
                            ? Math.Round(((p.p.FieldValue02.Value * (p.il.Qty - p.il.AllocatedQty - p.il.PickedQty) * 1.00m) / p.p.FieldValue01.Value), 3) : (p.il.Qty - p.il.AllocatedQty - p.il.PickedQty)
            }).FirstOrDefault();
            return stockMovementDto;
        }

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        public Pages<StockMovementDto> GetStockMovementList(StockMovementQuery stockMovementQuery)
        {
            var query = from sm in Context.stockmovements
                        join i in Context.invlotloclpns
                        on new { WarehouseSysId = sm.WareHouseSysId, skuSysId = sm.SkuSysId, loc = sm.FromLoc, lot = sm.Lot } equals new { WarehouseSysId = i.WareHouseSysId, skuSysId = i.SkuSysId, loc = i.Loc, lot = i.Lot }
                        join s in Context.skus on sm.SkuSysId equals s.SysId
                        join pk in Context.packs on s.PackSysId equals pk.SysId
                        select new
                        {
                            sm,
                            s,
                            Qty = i.Qty - i.AllocatedQty - i.PickedQty - i.FrozenQty,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02
                        };

            query = query.Where(p => p.sm.WareHouseSysId == stockMovementQuery.WarehouseSysId);

            if (!stockMovementQuery.SkuNameSearch.IsNull())
            {
                stockMovementQuery.SkuNameSearch = stockMovementQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(stockMovementQuery.SkuNameSearch));
            }
            if (!stockMovementQuery.SkuUPCSearch.IsNull())
            {
                stockMovementQuery.SkuUPCSearch = stockMovementQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.s.UPC.Equals(stockMovementQuery.SkuUPCSearch, StringComparison.OrdinalIgnoreCase));
            }
            if (!stockMovementQuery.ToLocSearch.IsNull())
            {
                stockMovementQuery.ToLocSearch = stockMovementQuery.ToLocSearch.Trim();
                query = query.Where(p => p.sm.ToLoc == stockMovementQuery.ToLocSearch);
            }
            if (stockMovementQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.sm.Status == stockMovementQuery.StatusSearch);
            }
            var stockMovements = query.Select(p => new StockMovementDto()
            {
                SysId = p.sm.SysId,
                StockMovementOrder = p.sm.StockMovementOrder,
                WareHouseSysId = p.sm.WareHouseSysId,
                Status = p.sm.Status,
                Descr = p.sm.Descr,
                SkuSysId = p.sm.SkuSysId,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                UPC = p.s.UPC,
                Lot = p.sm.Lot,
                Lpn = p.sm.Lpn,
                Qty = p.Qty,
                FromLoc = p.sm.FromLoc,
                ToLoc = p.sm.ToLoc,
                FromQty = p.sm.FromQty,
                ToQty = p.sm.ToQty,
                CreateBy = p.sm.CreateBy,
                CreateDate = p.sm.CreateDate,
                CreateUserName = p.sm.CreateUserName,
                UpdateBy = p.sm.UpdateBy,
                UpdateDate = p.sm.UpdateDate,
                UpdateUserName = p.sm.UpdateUserName,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.Qty
            }).Distinct();
            stockMovementQuery.iTotalDisplayRecords = stockMovements.Count();
            stockMovements = stockMovements.OrderByDescending(p => p.CreateDate).Skip(stockMovementQuery.iDisplayStart).Take(stockMovementQuery.iDisplayLength);
            return ConvertPages(stockMovements, stockMovementQuery);
        }

        /// <summary>
        /// 获取SkuPackUom
        /// </summary>
        /// <param name="skuSysIds"></param>
        /// <returns></returns>
        public List<SkuPackUomDto> GetSkuPackUomList(IEnumerable<Guid> skuSysIds)
        {
            var query = from s in Context.skus
                        join p in Context.packs on s.PackSysId equals p.SysId into t1
                        from it1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on it1.FieldUom01 equals u.SysId into t2
                        from it2 in t2.DefaultIfEmpty()
                        where skuSysIds.Contains(s.SysId)
                        select new { SkuSysId = s.SysId, SkuCode = s.SkuCode, PackSysId = it1.SysId, PackCode = it1.PackCode, UOMSysId = it2.SysId, UOMCode = it2.UOMCode };
            return query.Select(p => new SkuPackUomDto
            {
                SkuSysId = p.SkuSysId,
                SkuCode = p.SkuCode,
                PackSysId = p.PackSysId,
                PackCode = p.PackCode,
                UOMSysId = p.UOMSysId,
                UOMCode = p.UOMCode
            }).ToList();
        }
    }
}
