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
    public class SkuBorrowRepository : CrudRepository, ISkuBorrowRepository
    {
        public SkuBorrowRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        public Pages<SkuBorrowListDto> GetSkuBorrowListByPage(SkuBorrowQuery skuborrowQuery)
        {
            var query = from skuBorrow in Context.skuborrow
                        join skuBorrowDetail in Context.skuborrowdetail on skuBorrow.SysId equals skuBorrowDetail.SkuBorrowSysId
                        join s in Context.skus on skuBorrowDetail.SkuSysId equals s.SysId
                        select new { skuBorrow, s.SkuCode, s.SkuDescr, s.SkuName, s.UPC };

            query = query.Where(p => p.skuBorrow.WareHouseSysId == skuborrowQuery.WarehouseSysId);

            if (!skuborrowQuery.SkuBorrowOrder.IsNull())
            {
                skuborrowQuery.SkuBorrowOrder = skuborrowQuery.SkuBorrowOrder.Trim();
                query = query.Where(p => p.skuBorrow.BorrowOrder.Equals(skuborrowQuery.SkuBorrowOrder, StringComparison.OrdinalIgnoreCase));
            }
            if (skuborrowQuery.Status.HasValue)
            {
                query = query.Where(p => p.skuBorrow.Status == skuborrowQuery.Status);
            }
            if (!skuborrowQuery.CreateUserName.IsNull())
            {
                skuborrowQuery.CreateUserName = skuborrowQuery.CreateUserName.Trim();
                query = query.Where(p => p.skuBorrow.CreateUserName.Equals(skuborrowQuery.CreateUserName, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuborrowQuery.UPC.IsNull())
            {
                skuborrowQuery.UPC = skuborrowQuery.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(skuborrowQuery.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuborrowQuery.SkuCode.IsNull())
            {
                skuborrowQuery.SkuCode = skuborrowQuery.SkuCode.Trim();
                query = query.Where(p => p.SkuCode.Equals(skuborrowQuery.SkuCode, StringComparison.OrdinalIgnoreCase));
            }
            if (!skuborrowQuery.SkuName.IsNull())
            {
                skuborrowQuery.SkuName = skuborrowQuery.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(skuborrowQuery.SkuName));
            }

            if (!skuborrowQuery.BorrowName.IsNull())
            {
                skuborrowQuery.BorrowName = skuborrowQuery.BorrowName.Trim();
                query = query.Where(p => p.skuBorrow.BorrowName.Contains(skuborrowQuery.BorrowName));
            }

            if (!skuborrowQuery.LendingDepartment.IsNull())
            {
                skuborrowQuery.LendingDepartment = skuborrowQuery.LendingDepartment.Trim();
                query = query.Where(p => p.skuBorrow.LendingDepartment.Contains(skuborrowQuery.LendingDepartment));
            }

            var skuborrows = query.Select(p => new SkuBorrowListDto()
            {
                SysId = p.skuBorrow.SysId,
                BorrowOrder = p.skuBorrow.BorrowOrder,
                Status = p.skuBorrow.Status,
                BorrowName = p.skuBorrow.BorrowName,
                LendingDepartment = p.skuBorrow.LendingDepartment,
                BorrowStartTime = p.skuBorrow.BorrowStartTime,
                BorrowEndTime = p.skuBorrow.BorrowEndTime,
                CreateDate = p.skuBorrow.CreateDate,
                CreateUserName = p.skuBorrow.CreateUserName,
                Remark = p.skuBorrow.Remark,
            }).Distinct();

            skuborrowQuery.iTotalDisplayRecords = skuborrows.Count();
            skuborrows = skuborrows.OrderByDescending(p => p.CreateDate).Skip(skuborrowQuery.iDisplayStart).Take(skuborrowQuery.iDisplayLength);
            return ConvertPages(skuborrows, skuborrowQuery);
        }

        public SkuBorrowViewDto GetSkuBorrowBySysId(Guid skuborrowSysId)
        {
            var query = from skuBorrow in Context.skuborrow
                        join warehouse in Context.warehouses on skuBorrow.WareHouseSysId equals warehouse.SysId into tempWarehouse
                        from tw in tempWarehouse.DefaultIfEmpty()
                        where skuBorrow.SysId == skuborrowSysId
                        select new SkuBorrowViewDto()
                        {
                            SysId = skuBorrow.SysId,
                            SkuBorrowOrder = skuBorrow.BorrowOrder,
                            WareHouseSysId = skuBorrow.WareHouseSysId,
                            WareHouseName = tw.Name,
                            BorrowStartTime = skuBorrow.BorrowStartTime,
                            BorrowEndTime = skuBorrow.BorrowEndTime,
                            BorrowName = skuBorrow.BorrowName,
                            LendingDepartment = skuBorrow.LendingDepartment,
                            OtherId = skuBorrow.OtherId,
                            Channel = skuBorrow.Channel,
                            Remark = skuBorrow.Remark,
                            Status = skuBorrow.Status,
                            CreateDate = skuBorrow.CreateDate,
                            CreateUserName = skuBorrow.CreateUserName
                        };

            return query.FirstOrDefault();
        }

        public List<SkuBorrowDetailViewDto> GetSkuBorrowDetails(Guid skuborrowSysId)
        {
            var query = from skuBorrowDetail in Context.skuborrowdetail
                        join s in Context.skus on skuBorrowDetail.SkuSysId equals s.SysId
                        join pack in Context.packs on s.PackSysId equals pack.SysId
                        join uom1 in Context.uoms on pack.FieldUom01 equals uom1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join uom2 in Context.uoms on pack.FieldUom02 equals uom2.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join p in Context.packs on s.PackSysId equals p.SysId into t3
                        from p1 in t3.DefaultIfEmpty()

                        where skuBorrowDetail.SkuBorrowSysId == skuborrowSysId
                        select new SkuBorrowDetailViewDto()
                        {
                            SysId = skuBorrowDetail.SysId,
                            SkuSysId = skuBorrowDetail.SkuSysId.Value,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti2.UOMCode : ti1.UOMCode,
                            Loc = skuBorrowDetail.Loc,
                            Lot = skuBorrowDetail.Lot,
                            Lpn = skuBorrowDetail.Lpn,
                            Qty = skuBorrowDetail.Qty,
                            BorrowStartTime = skuBorrowDetail.BorrowStartTime,
                            BorrowEndTime = skuBorrowDetail.BorrowEndTime,
                            IsDamage = skuBorrowDetail.IsDamage,
                            DamageReason = skuBorrowDetail.DamageReason,
                            Remark = skuBorrowDetail.Remark ?? "",
                            ReturnQty = skuBorrowDetail.ReturnQty,
                            DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * skuBorrowDetail.Qty * 1.00m) / p1.FieldValue01.Value), 3) : skuBorrowDetail.Qty,
                            DisplayReturnQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * skuBorrowDetail.ReturnQty * 1.00m) / p1.FieldValue01.Value), 3) : skuBorrowDetail.ReturnQty
                        };
            return query.ToList();
        }


        public SkuBorrowViewDto GetSkuBorrowByOrder(string borrowOrder)
        {
            var query = from skuBorrow in Context.skuborrow
                        join warehouse in Context.warehouses on skuBorrow.WareHouseSysId equals warehouse.SysId into tempWarehouse
                        from tw in tempWarehouse.DefaultIfEmpty()
                        where skuBorrow.BorrowOrder == borrowOrder
                        select new SkuBorrowViewDto()
                        {
                            SysId = skuBorrow.SysId,
                            SkuBorrowOrder = skuBorrow.BorrowOrder,
                            WareHouseSysId = skuBorrow.WareHouseSysId,
                            WareHouseName = tw.Name,
                            BorrowStartTime = skuBorrow.BorrowStartTime,
                            BorrowEndTime = skuBorrow.BorrowEndTime,
                            BorrowName = skuBorrow.BorrowName,
                            LendingDepartment = skuBorrow.LendingDepartment,
                            OtherId = skuBorrow.OtherId,
                            Status = skuBorrow.Status,
                            CreateDate = skuBorrow.CreateDate,
                            CreateUserName = skuBorrow.CreateUserName
                        };

            return query.FirstOrDefault();
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
                        where inv.WareHouseSysId == skuQuery.WarehouseSysId
                        select new
                        {
                            s,
                            inv.Loc,
                            inv.Lpn,
                            inv.Lot,
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
                UOMCode = p.UOMCode2 ?? p.UOMCode1,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.Qty
            }).Where(p => p.DisplayQty > 0).Distinct();
            skuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderByDescending(p => p.SkuName).Skip(skuQuery.iDisplayStart).Take(skuQuery.iDisplayLength);
            return ConvertPages(skus, skuQuery);
        }
    }
}
