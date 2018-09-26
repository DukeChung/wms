using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class AdjustmentRepository : CrudRepository, IAdjustmentRepository
    {
        public AdjustmentRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<AdjustmentListDto> GetAdjustmentListByPage(AdjustmentQuery adjustmentQuery)
        {
            var query = from adjust in Context.adjustments
                        join adjustDetail in Context.adjustmentdetails on adjust.SysId equals adjustDetail.AdjustmentSysId
                        join s in Context.skus on adjustDetail.SkuSysId equals s.SysId
                        select new { adjust, s.SkuCode, s.SkuDescr, s.SkuName, s.UPC };

            query = query.Where(p => p.adjust.WareHouseSysId == adjustmentQuery.WarehouseSysId);

            if (!adjustmentQuery.AdjustmentOrder.IsNull())
            {
                adjustmentQuery.AdjustmentOrder = adjustmentQuery.AdjustmentOrder.Trim();
                query = query.Where(p => p.adjust.AdjustmentOrder.Equals(adjustmentQuery.AdjustmentOrder, StringComparison.OrdinalIgnoreCase));
            }
            if (adjustmentQuery.Status.HasValue)
            {
                query = query.Where(p => p.adjust.Status == adjustmentQuery.Status);
            }
            if (adjustmentQuery.Type.HasValue)
            {
                query = query.Where(p => p.adjust.Type == adjustmentQuery.Type);
            }
            if (!adjustmentQuery.CreateUserName.IsNull())
            {
                adjustmentQuery.CreateUserName = adjustmentQuery.CreateUserName.Trim();
                query = query.Where(p => p.adjust.CreateUserName.Equals(adjustmentQuery.CreateUserName, StringComparison.OrdinalIgnoreCase));
            }
            if (adjustmentQuery.Type.HasValue)
            {
                query = query.Where(p => p.adjust.Type == adjustmentQuery.Type);
            }
            if (!adjustmentQuery.UPC.IsNull())
            {
                adjustmentQuery.UPC = adjustmentQuery.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(adjustmentQuery.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(adjustmentQuery.SourceOrder))
            {
                adjustmentQuery.SourceOrder = adjustmentQuery.SourceOrder.Trim();
                query = query.Where(p => p.adjust.SourceOrder == adjustmentQuery.SourceOrder);
            }
            if (!adjustmentQuery.SkuCode.IsNull())
            {
                adjustmentQuery.SkuCode = adjustmentQuery.SkuCode.Trim();
                query = query.Where(p => p.SkuCode.Equals(adjustmentQuery.SkuCode, StringComparison.OrdinalIgnoreCase));
            }
            if (!adjustmentQuery.SkuName.IsNull())
            {
                adjustmentQuery.SkuName = adjustmentQuery.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(adjustmentQuery.SkuName));
            }

            var adjustments = query.Select(p => new AdjustmentListDto()
            {
                SysId = p.adjust.SysId,
                AdjustmentOrder = p.adjust.AdjustmentOrder,
                Status = p.adjust.Status,
                SourceOrder = p.adjust.SourceOrder,
                Type = p.adjust.Type,
                CreateDate = p.adjust.CreateDate,
                CreateUserName = p.adjust.CreateUserName
            }).Distinct();

            adjustmentQuery.iTotalDisplayRecords = adjustments.Count();
            adjustments = adjustments.OrderByDescending(p => p.CreateDate).Skip(adjustmentQuery.iDisplayStart).Take(adjustmentQuery.iDisplayLength);
            return ConvertPages(adjustments, adjustmentQuery);
        }

        public AdjustmentViewDto GetAdjustmentBySysId(Guid adjustmentSysId)
        {
            var query = from adjust in Context.adjustments
                        join warehouse in Context.warehouses on adjust.WareHouseSysId equals warehouse.SysId into tempWarehouse
                        from tw in tempWarehouse.DefaultIfEmpty()
                        where adjust.SysId == adjustmentSysId
                        select new AdjustmentViewDto()
                        {
                            SysId = adjust.SysId,
                            AdjustmentOrder = adjust.AdjustmentOrder,
                            WareHouseSysId = adjust.WareHouseSysId,
                            WareHouseName = tw.Name,
                            Status = adjust.Status,
                            Type = adjust.Type,
                            SourceOrder = adjust.SourceOrder,
                            CreateDate = adjust.CreateDate,
                            CreateUserName = adjust.CreateUserName
                        };

            return query.FirstOrDefault();
        }

        public List<AdjustmentDetailViewDto> GetAdjustmentDetails(Guid adjustmentSysId, Guid warehouseSysId)
        {
            var syscodeQuery = from syscode in Context.syscodes
                               join syscodedetail in Context.syscodedetails on syscode.SysId equals syscodedetail.SysCodeSysId
                               where syscode.SysCodeType == PublicConst.AdjustmentLevel
                               select new { syscodedetail };

            var query = from adjustDetail in Context.adjustmentdetails
                        join s in Context.skus on adjustDetail.SkuSysId equals s.SysId
                        join pack in Context.packs on s.PackSysId equals pack.SysId
                        join uom1 in Context.uoms on pack.FieldUom01 equals uom1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join uom2 in Context.uoms on pack.FieldUom02 equals uom2.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join syscode in syscodeQuery on adjustDetail.AdjustlevelCode equals syscode.syscodedetail.Code into syscodeTemp
                        from syscodeDetail in syscodeTemp.DefaultIfEmpty()
                        join p in Context.packs on s.PackSysId equals p.SysId into t3
                        from p1 in t3.DefaultIfEmpty() 
                        join invlots in Context.invlots on new { SkuSysId = (adjustDetail.SkuSysId == null ? Guid.Empty : (Guid)adjustDetail.SkuSysId), adjustDetail.Lot, WareHouseSysId = warehouseSysId } equals new { invlots.SkuSysId, invlots.Lot, invlots.WareHouseSysId } into t4
                        from ti4 in t4.DefaultIfEmpty() 
                        where adjustDetail.AdjustmentSysId == adjustmentSysId
                        select new AdjustmentDetailViewDto()
                        {
                            SysId = adjustDetail.SysId,
                            SkuSysId = adjustDetail.SkuSysId.Value,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti2.UOMCode : ti1.UOMCode,
                            Loc = adjustDetail.Loc,
                            Lot = adjustDetail.Lot,
                            Lpn = adjustDetail.Lpn,
                            Channel = ti4.LotAttr01,
                            Qty = adjustDetail.Qty,
                            Remark = adjustDetail.Remark ?? "",
                            DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * adjustDetail.Qty * 1.00m) / p1.FieldValue01.Value), 3) : adjustDetail.Qty,
                            AdjustlevelCode = adjustDetail.AdjustlevelCode,
                            AdjustlevelDisplay = syscodeDetail.syscodedetail.Descr
                        };
            return query.ToList();
        }

        public Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery)
        {
            var query = from s in Context.skus
                        join inv in Context.invlotloclpns on s.SysId equals inv.SkuSysId
                        join warehouse in Context.warehouses on inv.WareHouseSysId equals warehouse.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        join uom1 in Context.uoms on p.FieldUom01 equals uom1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join uom2 in Context.uoms on p.FieldUom02 equals uom2.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join p1 in Context.packs on s.PackSysId equals p1.SysId into t3
                        from pk in t3.DefaultIfEmpty()
                        join invlots in Context.invlots on new { SkuSysId = s.SysId, inv.Lot, WareHouseSysId = skuQuery.WarehouseSysId } equals new { invlots.SkuSysId, invlots.Lot, invlots.WareHouseSysId } into t4
                        from ti4 in t4.DefaultIfEmpty()

                        where inv.WareHouseSysId == skuQuery.WarehouseSysId
                        select new
                        {
                            s,
                            inv.Loc,
                            inv.Lpn,
                            inv.Lot,
                            Channel = ti4.LotAttr01,
                            inv.Qty,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02,
                            UOMCode1 = ti1.UOMCode,
                            UOMCode2 = ti2.UOMCode,
                            inv.WareHouseSysId,
                            WarehouseName = warehouse.Name
                        };

            if (!skuQuery.SkuCode.IsNull())
            {
                skuQuery.SkuCode = skuQuery.SkuCode.Trim();
                query = query.Where(p => p.s.SkuCode.Equals(skuQuery.SkuCode, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuQuery.SkuName.IsNull())
            {
                skuQuery.SkuName = skuQuery.SkuName.Trim();
                query = query.Where(p => p.s.SkuName.Contains(skuQuery.SkuName));
            }
            if (!skuQuery.UPC.IsNull())
            {
                skuQuery.UPC = skuQuery.UPC.Trim();
                query = query.Where(p => p.s.UPC.Equals(skuQuery.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuQuery.Loc.IsNull())
            {
                skuQuery.Loc = skuQuery.Loc.Trim();
                query = query.Where(p => p.Loc.Equals(skuQuery.Loc, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuQuery.Lpn.IsNull())
            {
                skuQuery.Lpn = skuQuery.Lpn.Trim();
                query = query.Where(p => p.Lpn.Equals(skuQuery.Lpn, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuQuery.Lot.IsNull())
            {
                skuQuery.Lot = skuQuery.Lot.Trim();
                query = query.Where(p => p.Lot.Equals(skuQuery.Lot, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuQuery.Channel.IsNull())
            {
                skuQuery.Channel = skuQuery.Channel.Trim();
                query = query.Where(p => p.Channel.Contains(skuQuery.Channel));
            }

            var skus = query.Select(p => new SkuInvLotLocLpnDto()
            {
                SkuSysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                WareHouseSysId = p.WareHouseSysId,
                WareHouseName = p.WarehouseName,
                UPC = p.s.UPC,
                Loc = p.Loc,
                Lpn = p.Lpn,
                Lot = p.Lot,
                Channel = p.Channel,
                UOMCode = p.UOMCode2 ?? p.UOMCode1,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.Qty
            }).Distinct();
            skuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderByDescending(p => p.SkuName).ThenBy(p => p.Loc).ThenBy(p => p.Lot).Skip(skuQuery.iDisplayStart).Take(skuQuery.iDisplayLength);
            return ConvertPages(skus, skuQuery);
        }
    }
}
