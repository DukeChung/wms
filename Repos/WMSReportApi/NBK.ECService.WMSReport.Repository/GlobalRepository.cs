using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using NBK.ECService.WMSReport.Utility.Redis;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.Repository
{
    public class GlobalRepository : CrudRepository, IGlobalRepository
    {

        /// <param name="dbContextProvider"></param>
        public GlobalRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 根据userId获取仓库信息
        /// </summary> 
        /// <returns></returns>
        public List<WareHouseDto> GetAllWarehouse()
        {
            var query = from w in Context.warehouses
                        where w.IsActive == true
                        orderby w.Name
                        select new WareHouseDto()
                        {
                            SysId = w.SysId,
                            Name = w.Name
                        };

            return query.ToList();
        }

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<PurchaseDetailGlobalDto> GetPurchaseDetailReport(PurchaseDetailGlobalQuery request)
        {
            Pages<PurchaseDetailGlobalDto> response = new Pages<PurchaseDetailGlobalDto>();
            var query = from p in Context.purchases
                        join w in Context.warehouses on p.WarehouseSysId equals w.SysId
                        join pd in Context.purchasedetails on p.SysId equals pd.PurchaseSysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId into t2
                        from p2 in t2.DefaultIfEmpty()
                        join v in Context.vendors on p.VendorSysId equals v.SysId into t3
                        from p3 in t3.DefaultIfEmpty()
                        join p4 in Context.packs on pd.PackSysId equals p4.SysId into t4
                        from packs in t4.DefaultIfEmpty()
                        select new
                        {
                            p.PurchaseOrder,
                            p.WarehouseSysId,
                            p.LastReceiptDate,
                            pd.Qty,
                            pd.ReceivedQty,
                            pd.RejectedQty,
                            p2.SkuName,
                            p2.UPC,
                            p3.VendorName,
                            packs,
                            WarehouseName = w.Name,
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(x => x.WarehouseSysId == request.SearchWarehouseSysId);
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrder))
            {
                request.PurchaseOrder = request.PurchaseOrder.Trim();
                query = query.Where(x => x.PurchaseOrder.Contains(request.PurchaseOrder));
            }
            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(x => x.SkuName.Contains(request.SkuName));
            }
            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(x => x.UPC.Contains(request.UPC));
            }
            if (!string.IsNullOrEmpty(request.VendorName))
            {
                request.VendorName = request.VendorName.Trim();
                query = query.Where(x => x.VendorName.Contains(request.VendorName));
            }
            if (request.LastReceiptDateFrom.HasValue)
            {
                query = query.Where(x => x.LastReceiptDate > request.LastReceiptDateFrom.Value);
            }
            if (request.LastReceiptDateTo.HasValue)
            {
                query = query.Where(x => x.LastReceiptDate < request.LastReceiptDateTo.Value);
            }
            var purchaseDetail = query.Select(p => new PurchaseDetailGlobalDto()
            {
                PurchaseOrder = p.PurchaseOrder,
                SkuName = p.SkuName,
                UPC = p.UPC,
                VendorName = p.VendorName,
                LastReceiptDate = p.LastReceiptDate,
                Qty = p.packs.InLabelUnit01.HasValue && p.packs.InLabelUnit01.Value == true && p.packs.FieldValue01 > 0 && p.packs.FieldValue02 > 0 ? Math.Round(((p.packs.FieldValue02.Value * (int)p.Qty * 1.00m) / p.packs.FieldValue01.Value), 3) : (decimal)p.Qty,
                ReceivedQty = p.packs.InLabelUnit01.HasValue && p.packs.InLabelUnit01.Value == true && p.packs.FieldValue01 > 0 && p.packs.FieldValue02 > 0 ? Math.Round(((p.packs.FieldValue02.Value * (int)p.ReceivedQty * 1.00m) / p.packs.FieldValue01.Value), 3) : (decimal)p.ReceivedQty,
                RejectedQty = p.packs.InLabelUnit01.HasValue && p.packs.InLabelUnit01.Value == true && p.packs.FieldValue01 > 0 && p.packs.FieldValue02 > 0 ? Math.Round(((p.packs.FieldValue02.Value * (int)p.RejectedQty * 1.00m) / p.packs.FieldValue01.Value), 3) : (decimal)p.RejectedQty,
                WarehouseName = p.WarehouseName

            }).Distinct();

            request.iTotalDisplayRecords = purchaseDetail.Count();
            purchaseDetail =
                purchaseDetail.OrderByDescending(p => p.LastReceiptDate)
                    .Skip(request.iDisplayStart)
                    .Take(request.iDisplayLength);
            response = ConvertPages(purchaseDetail, request);
            return response;
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLocGlobalDto> GetInvLocBySkuReport(InvSkuLocGlobalQuery request)
        {
            var query = from loc in Context.invskulocs
                        join sku in Context.skus on loc.SkuSysId equals sku.SysId
                        join pk in Context.packs on sku.PackSysId equals pk.SysId
                        join wh in Context.warehouses on loc.WareHouseSysId equals wh.SysId
                        select new
                        {
                            WarehouseName = wh.Name,
                            loc,
                            sku.SkuCode,
                            sku.SkuName,
                            sku.SkuDescr,
                            sku.UPC,
                            sku.IsMaterial,
                            sku.OtherId,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(p => p.loc.WareHouseSysId == request.SearchWarehouseSysId);
            }
            if (!request.SkuName.IsNull())
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(request.SkuName));
            }
            if (!request.SkuDescr.IsNull())
            {
                request.SkuDescr = request.SkuDescr.Trim();
                query = query.Where(p => p.SkuDescr.Contains(request.SkuDescr));
            }
            if (!request.OtherId.IsNull())
            {
                request.OtherId = request.OtherId.Trim();
                query = query.Where(p => p.OtherId == request.OtherId);
            }
            if (!request.UPC.IsNull())
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.Loc.IsNull())
            {
                request.Loc = request.Loc.Trim();
                query = query.Where(p => p.loc.Loc.Equals(request.Loc, StringComparison.OrdinalIgnoreCase));
            }
            if (request.IsMaterial.HasValue)
            {
                if (request.IsMaterial.Value)
                {
                    query = query.Where(p => p.IsMaterial == request.IsMaterial.Value);
                }
                else
                {
                    //不是原材料
                    query = query.Where(p => p.IsMaterial != (!request.IsMaterial.Value));
                }
            }
            if (request.IsStoreZero.HasValue && request.IsStoreZero.Value)
            {
                query = query.Where(p => p.loc.Qty > 0);
            }

            var skuLocs = query.Select(p => new InvSkuLocGlobalDto()
            {
                WarehouseName = p.WarehouseName,
                SysId = p.loc.SysId,
                SkuSysId = p.loc.SkuSysId,
                SkuCode = p.SkuCode,
                SkuName = p.SkuName,
                SkuDescr = p.SkuDescr,
                UPC = p.UPC,
                OtherId = p.OtherId,
                Loc = p.loc.Loc,
                Qty = p.loc.Qty,
                AllocatedQty = p.loc.AllocatedQty,
                PickedQty = p.loc.PickedQty,
                FrozenQty = p.loc.FrozenQty,
                UpdateDate = p.loc.UpdateDate,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.loc.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.loc.Qty,

                DisplayAllocatedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                          && p.FieldValue01 > 0 && p.FieldValue02 > 0
                          ? Math.Round(((p.FieldValue02.Value * p.loc.AllocatedQty * 1.00m) / p.FieldValue01.Value), 3) : p.loc.AllocatedQty,


                DisplayPickedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                          && p.FieldValue01 > 0 && p.FieldValue02 > 0
                          ? Math.Round(((p.FieldValue02.Value * p.loc.PickedQty * 1.00m) / p.FieldValue01.Value), 3) : p.loc.PickedQty,
                DisplayFrozenQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                          && p.FieldValue01 > 0 && p.FieldValue02 > 0
                          ? Math.Round(((p.FieldValue02.Value * p.loc.FrozenQty * 1.00m) / p.FieldValue01.Value), 3) : p.loc.FrozenQty

            }).Distinct();
            request.iTotalDisplayRecords = skuLocs.Count();
            skuLocs =
                skuLocs.OrderByDescending(p => p.UpdateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(skuLocs, request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotGlobalDto> GetInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            var query = from lot in Context.invlots
                        join w in Context.warehouses on lot.WareHouseSysId equals w.SysId
                        join sku in Context.skus on lot.SkuSysId equals sku.SysId
                        join pk in Context.packs on sku.PackSysId equals pk.SysId
                        select new
                        {
                            lot,
                            sku.SkuCode,
                            sku.SkuName,
                            sku.SkuDescr,
                            sku.UPC,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02,
                            WarehouseName = w.Name
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(p => p.lot.WareHouseSysId == request.SearchWarehouseSysId);
            }
            if (!request.SkuName.IsNull())
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(request.SkuName));
            }
            if (!request.SkuDescr.IsNull())
            {
                request.SkuDescr = request.SkuDescr.Trim();
                query = query.Where(p => p.SkuDescr.Contains(request.SkuDescr));
            }
            if (!request.UPC.IsNull())
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.Lot.IsNull())
            {
                request.Lot = request.Lot.Trim();
                query = query.Where(p => p.lot.Lot.Equals(request.Lot, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(request.LotAttr01))
            {
                request.LotAttr01 = request.LotAttr01.Trim();
                query = query.Where(p => p.lot.LotAttr01.Contains(request.LotAttr01));
            }

            var skuLots = query.Select(p => new InvSkuLotGlobalDto()
            {
                SysId = p.lot.SysId,
                SkuSysId = p.lot.SkuSysId,
                SkuCode = p.SkuCode,
                SkuName = p.SkuName,
                UPC = p.UPC,
                Lot = p.lot.Lot,
                LotAttr01 = p.lot.LotAttr01,
                LotAttr02 = p.lot.LotAttr02,
                ProduceDate = p.lot.ProduceDate,
                ExpiryDate = p.lot.ExpiryDate,
                Qty = p.lot.Qty,
                AllocatedQty = p.lot.AllocatedQty,
                PickedQty = p.lot.PickedQty,
                FrozenQty = p.lot.FrozenQty,
                UpdateDate = p.lot.UpdateDate,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                    && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                    ? Math.Round(((p.FieldValue02.Value * p.lot.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.Qty,
                DisplayAllocatedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.AllocatedQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.AllocatedQty,
                DisplayPickedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.PickedQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.PickedQty,
                DisplayFrozenQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.FrozenQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.FrozenQty,
                WarehouseName = p.WarehouseName

            }).Distinct();
            request.iTotalDisplayRecords = skuLots.Count();
            skuLots =
                skuLots.OrderByDescending(p => p.UpdateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(skuLots, request);
        }

        /// <summary>
        /// 货位批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotLocLpnGlobalDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request)
        {
            var query = from lot in Context.invlotloclpns
                        join w in Context.warehouses on lot.WareHouseSysId equals w.SysId
                        join l in Context.invlots on new { lot.SkuSysId, lot.Lot, lot.WareHouseSysId } equals new { l.SkuSysId, l.Lot, l.WareHouseSysId }
                        join sku in Context.skus on lot.SkuSysId equals sku.SysId
                        join pk in Context.packs on sku.PackSysId equals pk.SysId
                        select new
                        {
                            lot,
                            l,
                            sku.SkuCode,
                            sku.SkuName,
                            sku.SkuDescr,
                            sku.UPC,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02,
                            WarehouseName = w.Name
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(p => p.lot.WareHouseSysId == request.SearchWarehouseSysId);
            }
            if (!request.SkuName.IsNull())
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(request.SkuName));
            }
            if (!request.SkuDescr.IsNull())
            {
                request.SkuDescr = request.SkuDescr.Trim();
                query = query.Where(p => p.SkuDescr.Contains(request.SkuDescr));
            }
            if (!request.UPC.IsNull())
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.Lot.IsNull())
            {
                request.Lot = request.Lot.Trim();
                query = query.Where(p => p.lot.Lot.Equals(request.Lot, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.Loc.IsNull())
            {
                request.Loc = request.Loc.Trim();
                query = query.Where(p => p.lot.Loc.Equals(request.Loc, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(request.LotAttr01))
            {
                request.LotAttr01 = request.LotAttr01.Trim();
                query = query.Where(p => p.l.LotAttr01.Contains(request.LotAttr01));
            }

            var skuLots = query.Select(p => new InvSkuLotLocLpnGlobalDto()
            {
                SysId = p.lot.SysId,
                SkuSysId = p.lot.SkuSysId,
                SkuCode = p.SkuCode,
                SkuName = p.SkuName,
                UPC = p.UPC,
                Lot = p.lot.Lot,
                Loc = p.lot.Loc,
                Qty = p.lot.Qty,
                AllocatedQty = p.lot.AllocatedQty,
                PickedQty = p.lot.PickedQty,
                FrozenQty = p.lot.FrozenQty,
                UpdateDate = p.lot.UpdateDate,
                LotAttr01 = p.l.LotAttr01,
                ProduceDate = p.l.ProduceDate,
                ExpiryDate = p.l.ExpiryDate,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                    && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                    ? Math.Round(((p.FieldValue02.Value * p.lot.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.Qty,
                DisplayAllocatedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.AllocatedQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.AllocatedQty,
                DisplayPickedQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.PickedQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.PickedQty,
                DisplayFrozenQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                            && p.FieldValue01 > 0 && p.FieldValue02 > 0
                            ? Math.Round(((p.FieldValue02.Value * p.lot.FrozenQty * 1.00m) / p.FieldValue01.Value), 3) : p.lot.FrozenQty,
                WarehouseName = p.WarehouseName

            }).Distinct();
            request.iTotalDisplayRecords = skuLots.Count();
            skuLots =
                skuLots.OrderByDescending(p => p.UpdateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(skuLots, request);
        }

        /// <summary>
        /// 临期批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotGlobalDto> GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            var syscode = from sysCode in Context.syscodes
                          join detail in Context.syscodedetails on sysCode.SysId equals detail.SysCodeSysId
                          where sysCode.SysCodeType.Equals("ShelfLifeWarning", StringComparison.OrdinalIgnoreCase)
                                && detail.IsActive == true
                          select new
                          {
                              detail.Code
                          };
            Pages<InvSkuLotGlobalDto> response = new Pages<InvSkuLotGlobalDto>();
            if (syscode.FirstOrDefault() != null)
            {
                int days = int.Parse(syscode.FirstOrDefault().Code);

                DateTime comparaDate = DateTime.Now.AddDays(days);

                var query = from lot in Context.invlots
                            join w in Context.warehouses on lot.WareHouseSysId equals w.SysId
                            join lotloc in Context.invlotloclpns on lot.SkuSysId equals lotloc.SkuSysId
                            join sku in Context.skus on lot.SkuSysId equals sku.SysId
                            join pk in Context.packs on sku.PackSysId equals pk.SysId

                            where lot.Lot == lotloc.Lot
                                  && lot.ExpiryDate.HasValue
                                  && lot.ExpiryDate.Value <= comparaDate
                                  && lot.Qty > 0
                            select new
                            {
                                lot,
                                lotloc.UpdateDate,
                                lotloc.Qty,
                                lotloc.Loc,
                                sku.SkuCode,
                                sku.SkuName,
                                sku.SkuDescr,
                                sku.UPC,
                                pk.InLabelUnit01,
                                pk.FieldValue01,
                                pk.FieldValue02,
                                WarehouseName = w.Name
                            };

                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    query = query.Where(p => p.lot.WareHouseSysId == request.SearchWarehouseSysId);
                }
                if (!request.SkuName.IsNull())
                {
                    request.SkuName = request.SkuName.Trim();
                    query = query.Where(p => p.SkuName.Contains(request.SkuName));
                }
                if (!request.UPC.IsNull())
                {
                    request.UPC = request.UPC.Trim();
                    query = query.Where(p => p.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
                }
                if (request.IsExpiry != null)
                {
                    if (request.IsExpiry == 0)
                    {
                        query = query.Where(p => p.lot.ExpiryDate.Value > DateTime.Now);
                    }
                    else
                    {
                        query = query.Where(p => p.lot.ExpiryDate.Value <= DateTime.Now);
                    }
                }

                var skuLots = query.Select(p => new InvSkuLotGlobalDto()
                {
                    SysId = p.lot.SysId,
                    SkuSysId = p.lot.SkuSysId,
                    SkuCode = p.SkuCode,
                    SkuName = p.SkuName,
                    UPC = p.UPC,
                    Lot = p.lot.Lot,
                    Loc = p.Loc,
                    Qty = p.Qty,
                    ExpiryDate = p.lot.ExpiryDate,
                    ExpiryDescription = p.lot.ExpiryDate.Value > DateTime.Now ? "临期" : "已过期",
                    UpdateDate = p.UpdateDate,

                    DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                    && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                    ? Math.Round(((p.FieldValue02.Value * p.Qty * 1.00m) / p.FieldValue01.Value), 3) : p.Qty,
                    WarehouseName = p.WarehouseName

                }).Distinct();
                request.iTotalDisplayRecords = skuLots.Count();
                skuLots =
                    skuLots.OrderByDescending(p => p.UpdateDate)
                        .Skip(request.iDisplayStart)
                        .Take(request.iDisplayLength);
                response = ConvertPages(skuLots, request);
            }

            return response;
        }

        /// <summary>
        /// 收货明细查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceiptDetailGlobalDto> GetReceiptDetailReport(ReceiptDetailGlobalQuery request)
        {
            var query = from receiptdetail in Context.receiptdetails
                        join receipt in Context.receipts on receiptdetail.ReceiptSysId equals receipt.SysId
                        join purchase in Context.purchases on receipt.ExternalOrder equals purchase.PurchaseOrder
                        join w in Context.warehouses on receipt.WarehouseSysId equals w.SysId
                        join vendor in Context.vendors on receipt.VendorSysId equals vendor.SysId
                        join sku in Context.skus on receiptdetail.SkuSysId equals sku.SysId
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        select new
                        {
                            receiptdetail.SysId,
                            receiptdetail.ReceivedQty,
                            receiptdetail.ReceivedDate,
                            receiptdetail.ShelvesStatus,
                            receiptdetail.ShelvesQty,
                            receiptdetail.CreateDate,
                            receipt.ReceiptOrder,
                            receipt.ExternalOrder,
                            purchase.OutboundOrder,
                            receipt.Status,
                            receipt.WarehouseSysId,
                            receipt.ReceiptType,
                            vendor.VendorName,
                            sku.SkuCode,
                            sku.SkuName,
                            sku.SkuDescr,
                            sku.UPC,
                            sku.IsMaterial,
                            p1,
                            WarehouseName = w.Name
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(p => p.WarehouseSysId == request.SearchWarehouseSysId);
            }
            if (!request.ReceiptOrder.IsNull())
            {
                request.ReceiptOrder = request.ReceiptOrder.Trim();
                query = query.Where(p => p.ReceiptOrder == request.ReceiptOrder);
            }
            if (!request.ExternalOrder.IsNull())
            {
                request.ExternalOrder = request.ExternalOrder.Trim();
                query = query.Where(p => p.ExternalOrder == request.ExternalOrder);
            }
            if (!request.OutboundOrder.IsNull())
            {
                request.OutboundOrder = request.OutboundOrder.Trim();
                query = query.Where(p => p.OutboundOrder == request.OutboundOrder);
            }
            if (request.Status.HasValue)
            {
                query = query.Where(p => p.Status == request.Status);
            }
            if (!request.SkuName.IsNull())
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.SkuName.Contains(request.SkuName));
            }
            if (!request.SkuCode.IsNull())
            {
                request.SkuCode = request.SkuCode.Trim();
                query = query.Where(p => p.SkuCode == request.SkuCode);
            }
            if (!request.UPC.IsNull())
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
            }

            if (request.ReceivedDateFrom.HasValue)
            {
                query = query.Where(x => x.ReceivedDate.Value > request.ReceivedDateFrom.Value);
            }
            if (request.ReceivedDateTo.HasValue)
            {
                request.ReceivedDateTo = request.ReceivedDateTo.Value.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.ReceivedDate.Value < request.ReceivedDateTo.Value);
            }
            if (request.ShelvesStatus.HasValue)
            {
                query = query.Where(p => p.ShelvesStatus == request.ShelvesStatus);
            }
            if (!string.IsNullOrEmpty(request.VendorName))
            {
                request.VendorName = request.VendorName.Trim();
                query = query.Where(p => p.VendorName.Contains(request.VendorName));
            }
            if (request.ReceiptType.HasValue)
            {
                query = query.Where(p => p.ReceiptType == request.ReceiptType.Value);
            }
            if (request.IsMaterial.HasValue)
            {
                if (request.IsMaterial.Value)
                {
                    query = query.Where(p => p.IsMaterial == request.IsMaterial.Value);
                }
                else
                {
                    //不是原材料
                    query = query.Where(p => p.IsMaterial != (!request.IsMaterial.Value));
                }
            }
            var result = query.Select(p => new ReceiptDetailGlobalDto()
            {
                SysId = p.SysId,
                ReceivedQty = p.ReceivedQty ?? 0,
                ReceivedDate = p.ReceivedDate,
                ShelvesQty = p.ShelvesQty,
                CreateDate = p.CreateDate,
                ReceiptOrder = p.ReceiptOrder,
                ExternalOrder = p.ExternalOrder,
                OutboundOrder = p.OutboundOrder,
                VendorName = p.VendorName,
                Status = p.Status ?? 0,
                SkuCode = p.SkuCode,
                SkuName = p.SkuName,
                SkuDescr = p.SkuDescr,
                UPC = p.UPC,
                DisplayReceivedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.ReceivedQty.HasValue ? p.ReceivedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.ReceivedQty.HasValue ? p.ReceivedQty.Value : 0),
                DisplayShelvesQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.ShelvesQty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.ShelvesQty,
                WarehouseName = p.WarehouseName
            });
            request.iTotalDisplayRecords = result.Count();
            result = result.OrderByDescending(p => p.CreateDate)
                .Skip(request.iDisplayStart)
                .Take(request.iDisplayLength);
            return ConvertPages(result, request);
        }

        /// <summary>
        /// 出库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundDetailGlobalDto> GetOutboundDetailReport(OutboundDetailGlobalQuery request)
        {
            var sql = new StringBuilder();
            sql.Append(" SELECT  ot.SysId,  o.OutboundOrder,  IFNULL(o.Status, 10)  AS Status,");
            sql.Append(" IFNULL(o.OutboundType, 10)  AS OutboundType, o.OutboundChildType, s.SkuCode,s.SkuName,s.SkuDescr, ");
            sql.Append(" s.UPC, o.ConsigneeName,  o.ConsigneePhone,o.ConsigneeAddress,o.ServiceStationCode,");
            sql.Append(" o.ServiceStationName, o.SortNumber,o.TMSOrder,o.DepartureDate, ");
            sql.Append(" CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0 THEN ROUND(p.FieldValue02 * (IFNULL(ot.Qty,0)/p.FieldValue01),3) ELSE IFNULL(ot.Qty,0) END AS Qty, ");
            sql.Append(" CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0 THEN ROUND(p.FieldValue02 * (IFNULL(ot.ShippedQty,0)/p.FieldValue01),3) ELSE IFNULL(ot.ShippedQty,0) END AS ShippedQty, ");
            sql.Append(" o.OutboundDate, o.ActualShipDate,");
            sql.Append(" CONCAT(IFNULL(o.ConsigneeProvince, ''), IFNULL(o.ConsigneeCity, ''), ");
            sql.Append(" IFNULL(o.ConsigneeArea, ''), IFNULL(o.ConsigneeAddress, '')) as FullAddress ,w.Name as WarehouseName");
            sql.Append(" FROM outbound o ");
            sql.Append(" LEFT JOIN warehouse w ON o.WarehouseSysId =w.SysId ");
            sql.Append(" JOIN outbounddetail ot ON o.SysId =ot.OutboundSysId ");
            sql.Append(" LEFT JOIN sku s ON ot.SkuSysId = s.SysId ");
            sql.Append(" LEFT JOIN pack p ON s.PackSysId = p.SysId WHERE 1=1 @Where ORDER BY o.OutboundDate DESC");
            sql.AppendFormat(" LIMIT {0},{1} ", request.iDisplayStart, request.iDisplayLength);


            var countSql = new StringBuilder();
            countSql.Append(@" SELECT COUNT(*)  FROM  outbound o 
                            LEFT JOIN outbounddetail ot ON  o.SysId = ot.OutboundSysId 
                            WHERE  1 = 1 ");
            var mySqlParameter = new List<MySqlParameter>();
            var strWhere = new StringBuilder();
            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                strWhere.Append(" AND o.WareHouseSysId=@WareHouseSysId");
                countSql.Append(" AND o.WareHouseSysId=@WareHouseSysId");
                mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
            }
            if (!request.OutboundOrder.IsNull())
            {
                strWhere.Append(" AND o.OutboundOrder=@OutboundOrder");
                countSql.Append(" AND o.OutboundOrder=@OutboundOrder");
                mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
            }
            if (!request.ExternOrderId.IsNull())
            {
                strWhere.Append(" AND o.ExternOrderId=@ExternOrderId");
                countSql.Append(" AND o.ExternOrderId=@ExternOrderId");
                mySqlParameter.Add(new MySqlParameter("@ExternOrderId", request.ExternOrderId.Trim()));
            }
            if (request.Status.HasValue)
            {
                strWhere.Append(" AND o.Status=@Status");
                countSql.Append(" AND o.Status=@Status");
                mySqlParameter.Add(new MySqlParameter("@Status", request.Status.Value));
            }
            if (request.ActualShipDateFrom.HasValue)
            {
                strWhere.Append(" AND o.ActualShipDate > @ActualShipDateFrom");
                countSql.Append(" AND o.ActualShipDate > @ActualShipDateFrom");
                mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom.Value));
            }
            if (request.ActualShipDateTo.HasValue)
            {
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1).AddMilliseconds(-1);
                strWhere.Append(" AND o.ActualShipDate < @ActualShipDateTo");
                countSql.Append(" AND o.ActualShipDate < @ActualShipDateTo");
                mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo.Value));
            }

            if (request.DepartureDateFrom.HasValue)
            {
                strWhere.Append(" AND o.DepartureDate > @DepartureDateFrom");
                countSql.Append(" AND o.DepartureDate > @DepartureDateFrom");

                mySqlParameter.Add(new MySqlParameter("@DepartureDateFrom", request.DepartureDateFrom.Value));
            }
            if (request.DepartureDateTo.HasValue)
            {
                request.DepartureDateTo = request.DepartureDateTo.Value.AddDays(1).AddMilliseconds(-1);
                strWhere.Append(" AND o.DepartureDate < @DepartureDateTo");
                countSql.Append(" AND o.DepartureDate < @DepartureDateTo");
                mySqlParameter.Add(new MySqlParameter("@DepartureDateTo", request.DepartureDateTo.Value));
            }

            if (request.OutboundType.HasValue)
            {
                strWhere.Append(" AND o.OutboundType=@OutboundType");
                countSql.Append(" AND o.OutboundType=@OutboundType");
                mySqlParameter.Add(new MySqlParameter("@OutboundType", request.OutboundType.Value));

            }
            if (!request.ServiceStationCode.IsNull())
            {
                strWhere.AppendFormat(" AND o.ServiceStationCode = @ServiceStationCode");
                countSql.AppendFormat(" AND o.ServiceStationCode = @ServiceStationCode");
                mySqlParameter.Add(new MySqlParameter("@ServiceStationCode", request.ServiceStationCode.Trim()));
            }
            if (!request.ServiceStationName.IsNull())
            {
                strWhere.AppendFormat(" AND o.ServiceStationName LIKE CONCAT(@ServiceStationName,'%')");
                countSql.AppendFormat(" AND o.ServiceStationName LIKE CONCAT(@ServiceStationName,'%')");
                mySqlParameter.Add(new MySqlParameter("@ServiceStationName", request.ServiceStationName.Trim()));
            }
            if (!request.ConsigneeAddress.IsNull())
            {
                strWhere.AppendFormat(" AND o.ConsigneeAddress LIKE  CONCAT(@ConsigneeAddress,'%')");
                countSql.AppendFormat(" AND o.ConsigneeAddress LIKE  CONCAT(@ConsigneeAddress,'%')");
                mySqlParameter.Add(new MySqlParameter("@ConsigneeAddress", request.ConsigneeAddress.Trim()));
            }
            //业务类型
            if (!string.IsNullOrEmpty(request.OutboundChildType))
            {
                strWhere.AppendFormat(" AND o.OutboundChildType LIKE  CONCAT(@OutboundChildType,'%')");
                countSql.AppendFormat(" AND o.OutboundChildType LIKE  CONCAT(@OutboundChildType,'%')");
                mySqlParameter.Add(new MySqlParameter("@OutboundChildType", request.OutboundChildType.Trim()));
            }

            if (!request.SkuName.IsNull() || !request.SkuCode.IsNull() || !request.UPC.IsNull() || request.IsMaterial.HasValue)
            {
                countSql.Append(" AND ot.SkusysId in (select sysId from sku s where 1=1 ");
                var countWhere = new StringBuilder();
                if (!request.SkuName.IsNull())
                {
                    strWhere.AppendFormat(" AND s.SkuName LIKE CONCAT(@SkuName,'%')", request.SkuName.Trim());
                    countSql.AppendFormat(" AND s.SkuName  like CONCAT(@SkuName,'%')", request.SkuName.Trim());
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!request.SkuCode.IsNull())
                {
                    strWhere.AppendFormat(" AND s.SkuCode= @SkuCode", request.SkuCode.Trim());
                    countSql.AppendFormat(" AND s.SkuCode = @SkuCode", request.SkuCode.Trim());
                    mySqlParameter.Add(new MySqlParameter("@SkuCode", request.SkuCode.Trim()));
                }
                if (!request.UPC.IsNull())
                {
                    strWhere.AppendFormat(" AND s.UPC= @UPC", request.UPC.Trim());
                    countSql.AppendFormat(" AND s.UPC = @UPC", request.UPC.Trim());
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }

                if (request.IsMaterial.HasValue)
                {
                    if (request.IsMaterial.Value)
                    {
                        strWhere.Append(" AND s.IsMaterial = 1");
                        countSql.Append(" AND s.IsMaterial = 1");
                    }
                    else
                    {
                        strWhere.Append(" AND ifnull(s.IsMaterial,'') != '1'");
                        countSql.Append(" AND ifnull(s.IsMaterial,'') != '1'");
                    }
                }
                countSql.Append(" )");
            }

            var count = base.Context.Database.SqlQuery<int>(countSql.ToString(), mySqlParameter.ToArray()).AsQueryable().FirstOrDefault();
            if (request.IsExport == true && count > PublicConst.EachExportDataMaxCount)
            {
                throw new Exception("导出数据条数大于10W,请联系管理员!");
            }

            var queryList = base.Context.Database.SqlQuery<OutboundDetailGlobalDto>(sql.ToString().Replace("@Where", strWhere.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();

            var response = new Pages<OutboundDetailGlobalDto>();
            response.TableResuls = new TableResults<OutboundDetailGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;

        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuGlobalDto> GetInvSkuReport(InvSkuGlobalQuery request)
        {
            var strSql =
                new StringBuilder(
                    "SELECT s.SysId,s.otherId AS 'SkuOtherId',s.upc,s.SkuName,w.Name as 'WarehouseName',sum(inv.Qty) as Qty  FROM invLot inv  inner join sku s  on inv.SkuSysId = s.SysId left join warehouse w  on inv.warehouseSysId = w.SysId Where 1=1 ");
            var mySqlParameter = new List<MySqlParameter>();

            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    strSql.AppendFormat(" and inv.warehouseSysId = @WarehouseSysId ", request.SearchWarehouseSysId.ToString());
                    mySqlParameter.Add(new MySqlParameter("@WarehouseSysId", request.SearchWarehouseSysId.ToString()));
                }
                if (request.GreaterThanZero.HasValue && request.GreaterThanZero.Value)
                {
                    strSql.Append(" and inv.Qty > 0 ");
                }
                if (!string.IsNullOrEmpty(request.SkuCode))
                {
                    strSql.Append(" and s.SkuCode like CONCAT(@SkuCode,'%') ");
                    mySqlParameter.Add(new MySqlParameter("@SkuCode", request.SkuCode.Trim()));
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    strSql.Append(" and s.SkuName like CONCAT(@SkuName,'%')  ");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    strSql.Append("and s.UPC like  CONCAT(@UPC,'%')  ");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));

                }
                if (!string.IsNullOrEmpty(request.OtherId))
                {
                    strSql.AppendFormat("and s.OtherId like CONCAT(@OtherId,'%')");
                    mySqlParameter.Add(new MySqlParameter("@OtherId", request.OtherId.Trim()));
                }
                if (request.IsMaterial.HasValue)
                {
                    if (request.IsMaterial.Value)
                    {
                        strSql.Append(" AND s.IsMaterial = 1");
                    }
                    else
                    {
                        strSql.Append(" AND ifnull(s.IsMaterial,'') != '1'");
                    }
                }
            }
            strSql.Append(" GROUP BY s.SysId, s.otherId,s.upc,s.SkuName,w.Name ");

            var queryList = base.Context.Database.SqlQuery<InvSkuGlobalDto>(strSql.ToString(), mySqlParameter.ToArray()).ToList();
            request.iTotalDisplayRecords = queryList.Count();
            var pageList = queryList.Skip(request.iDisplayStart).Take(request.iDisplayLength);
            var result = ConvertPages(pageList.AsQueryable(), request);
            var skuSysIds = result.TableResuls.aaData.Select(x => x.SysId).Distinct();
            var skuList = from s in Context.skus
                          join p in Context.packs on s.PackSysId equals p.SysId
                          where skuSysIds.Contains(s.SysId)
                          select new { s, p };




            var list = from a in result.TableResuls.aaData
                       join b in skuList on a.SysId equals b.s.SysId

                       select new InvSkuGlobalDto
                       {
                           SysId = a.SysId,
                           SkuOtherId = a.SkuOtherId,
                           SkuName = a.SkuName,
                           UPC = a.UPC,
                           Qty = a.Qty,
                           WarehouseName = a.WarehouseName,
                           DisplayQty = b.p.InLabelUnit01.HasValue && b.p.InLabelUnit01.Value == true
                               && b.p.FieldValue01 > 0 && b.p.FieldValue02 > 0
                               ? Math.Round(((b.p.FieldValue02.Value * a.Qty * 1.00m) / b.p.FieldValue01.Value), 3) : a.Qty,
                       };
            result.TableResuls.aaData = list.ToList();

            return result;
        }

        /// <summary>
        /// 收发货明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceivedAndSendSkuGlobalDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(*) 
                              FROM
                              (
                                    SELECT  w.SysId,w.Name AS WarehouseName,w.WareHouseArea,w.WareHouseProperty,
                                    SUM(RSkuQty) AS ReceivedQty,SUM(RQty) AS ReceivedPieceQty,SUM(OSkuQty) AS DeliveryQty,
                                    SUM(OQty) AS DeliveryPieceQty,T.CreateDate,SUM(IFNULL(PurchaseCount,0)) AS PurchaseCount,SUM(IFNULL(OutboundCount,0)) AS OutboundCount
                                    FROM 
                                    (
                                            SELECT SUM(RSkuQty) AS RSkuQty,SUM(RQty) AS RQty,0 AS OSkuQty,0 AS OQty,B.WareHouseSysId,B.CreateDate,C.PurchaseCount,0 AS OutboundCount
                                            FROM 
                                            (
                                                    SELECT 1 AS RSkuQty,SUM(r1.ReceivedQty) AS RQty,r.WarehouseSysId,date_format( r.ReceiptDate,'%Y-%m-%d') AS CreateDate
                                                    FROM receipt r 
                                                    LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId
                                                    LEFT JOIN sku s ON r1.SkuSysId=s.SysId
                                                    WHERE r.Status=40  AND (s.IsMaterial =0 OR s.IsMaterial IS NULL)
                                                    @receiptWhere
                                                    GROUP BY r1.SkuSysId,CreateDate,r.WareHouseSysId
                                            ) B 
                                             LEFT JOIN 
                                            (  
                                                SELECT CC.WarehouseSysId,CC.CreateDate,SUM(CC.PurchaseCount) AS PurchaseCount 
                                                FROM 
                                                (
                                                    SELECT r.WarehouseSysId,COUNT(DISTINCT r.ExternalOrder) AS PurchaseCount,date_format(r.ReceiptDate, '%Y-%m-%d') AS CreateDate 
                                                    FROM receipt r WHERE r.Status = 40  
                                                    @purchaseCountWhere
                                                    GROUP BY r.WarehouseSysId,CreateDate
                                                )CC GROUP BY CC.WarehouseSysId,CC.CreateDate 
                                            ) C 
                                            ON B.WarehouseSysId = C.WarehouseSysId AND B.CreateDate = C.CreateDate
                                            GROUP BY B.CreateDate,B.WareHouseSysId
                                                    
                                            UNION ALL   
                    
                                            SELECT 0 AS RSkuQty,0 AS RQty,SUM(OSkuQty) AS OSkuQty,SUM(OQty) AS OQty,A.WareHouseSysId,A.CreateDate,0 AS PurchaseCount,B.OutboundCount 
                                            FROM 
                                            (
                                                    SELECT 1 AS OSkuQty,SUM(o1.ShippedQty) AS OQty,o.WareHouseSysId,date_format( o.ActualShipDate,'%Y-%m-%d') AS CreateDate 
                                                    FROM  outbounddetail o1 
                                                    LEFT JOIN outbound o ON o.SysId=o1.OutboundSysId
                                                    LEFT JOIN sku s ON o1.SkuSysId=s.SysId
                                                    WHERE o.Status=70  AND  (s.IsMaterial =0 OR s.IsMaterial IS NULL)
                                                    @outboundWhere
                                                    GROUP BY  o1.SkuSysId,CreateDate,o.WareHouseSysId
                                            ) A 
                                            LEFT JOIN 
                                            (
                                                 SELECT CC.WarehouseSysId,CC.CreateDate,SUM(CC.OutboundCount) AS OutboundCount
                                                 FROM   
                                                (
                                                    SELECT o.WarehouseSysId,COUNT(DISTINCT o.OutboundOrder) AS OutboundCount,date_format(o.ActualShipDate, '%Y-%m-%d') AS CreateDate 
                                                    FROM outbound o WHERE o.Status = 70
                                                    @outboundWhere
                                                    GROUP  BY o.WarehouseSysId,CreateDate
                                                )CC GROUP  BY CC.WarehouseSysId,CC.CreateDate 
                                            )B  ON B.WarehouseSysId = A.WarehouseSysId AND B.CreateDate = A.CreateDate 
                                            GROUP BY A.CreateDate,A.WareHouseSysId 
                                    ) T
                                    LEFT JOIN warehouse w ON T.WareHouseSysId=w.SysId
                                    WHERE T.CreateDate IS NOT NULL 
                                    @OutWhere
                                    GROUP BY T.CreateDate,T.WareHouseSysId
                              ) TT");

            var sql = new StringBuilder();
            sql.Append(@"   SELECT  w.Name AS WarehouseName, w.WareHouseArea, w.WareHouseProperty,
                            SUM(RSkuQty) AS ReceivedQty, SUM(RQty) AS ReceivedPieceQty, SUM(OSkuQty) AS DeliveryQty,
                            SUM(OQty) AS DeliveryPieceQty, T.CreateDate,SUM(IFNULL(PurchaseCount,0)) AS PurchaseCount,SUM(IFNULL(OutboundCount,0)) AS OutboundCount
                            FROM
                            (
                                SELECT SUM(RSkuQty) AS RSkuQty,SUM(RQty) AS RQty,0 AS OSkuQty,0 AS OQty, 
                                B.WareHouseSysId,B.CreateDate,C.PurchaseCount,0 AS OutboundCount 
                                FROM
                                ( 
                                    SELECT 1 AS RSkuQty, SUM(r1.ReceivedQty) AS RQty, r.WarehouseSysId,
                                    date_format(r.ReceiptDate, '%Y-%m-%d') AS CreateDate
                                    FROM receipt r 
                                    LEFT JOIN receiptdetail r1 ON r.SysId = r1.ReceiptSysId
                                    LEFT JOIN sku s ON r1.SkuSysId=s.SysId
                                    WHERE r.Status = 40 AND  (s.IsMaterial =0 OR s.IsMaterial IS NULL)
                                    @receiptWhere
                                    GROUP BY r1.SkuSysId, CreateDate, r.WareHouseSysId
                                ) B
                                LEFT JOIN 
                                (  
                                    SELECT CC.WarehouseSysId,CC.CreateDate,SUM(CC.PurchaseCount) AS PurchaseCount FROM (
                                    SELECT r.WarehouseSysId,COUNT(DISTINCT r.ExternalOrder) AS PurchaseCount,date_format(r.ReceiptDate, '%Y-%m-%d') AS CreateDate 
                                    FROM receipt r WHERE r.Status = 40  
                                    @purchaseCountWhere
                                    GROUP BY r.WarehouseSysId,CreateDate
                                    )CC GROUP BY CC.WarehouseSysId,CC.CreateDate 
                                ) C 
                                ON B.WarehouseSysId = C.WarehouseSysId AND B.CreateDate = C.CreateDate
                                GROUP BY B.CreateDate, B.WareHouseSysId
                    
                                UNION ALL
                            
                                SELECT 0 AS RSkuQty,0 AS RQty,SUM(OSkuQty) AS OSkuQty,SUM(OQty) AS OQty,A.WareHouseSysId, 
                                A.CreateDate,0 AS PurchaseCount,B.OutboundCount
                                FROM
                                (
                                    SELECT 1 AS OSkuQty, SUM(o1.ShippedQty) AS OQty, o.WareHouseSysId, 
                                    date_format(o.ActualShipDate, '%Y-%m-%d') AS CreateDate
                                    FROM  outbounddetail o1
                                    LEFT JOIN outbound o ON o.SysId = o1.OutboundSysId
                                    LEFT JOIN sku s ON o1.SkuSysId=s.SysId
                                    WHERE o.Status = 70 AND  (s.IsMaterial =0 OR s.IsMaterial IS NULL)
                                    @outboundWhere
                                    GROUP BY  o1.SkuSysId, CreateDate, o.WareHouseSysId
                                ) A
                                 LEFT JOIN 
                                (
                                    SELECT CC.WarehouseSysId,CC.CreateDate,SUM(CC.OutboundCount) AS OutboundCount
                                    FROM   
                                    (
                                        SELECT o.WarehouseSysId,COUNT(DISTINCT o.OutboundOrder) AS OutboundCount,date_format(o.ActualShipDate, '%Y-%m-%d') AS CreateDate 
                                        FROM outbound o WHERE o.Status = 70
                                        @outboundWhere
                                        GROUP  BY o.WarehouseSysId,CreateDate
                                    )CC GROUP  BY CC.WarehouseSysId,CC.CreateDate 
                                )B  ON B.WarehouseSysId = A.WarehouseSysId AND B.CreateDate = A.CreateDate  
                                GROUP BY A.CreateDate, A.WareHouseSysId
                            ) T
                            LEFT JOIN warehouse w ON T.WareHouseSysId = w.SysId
                            WHERE T.CreateDate IS NOT NULL
                            @OutWhere
                            GROUP BY T.CreateDate, T.WareHouseSysId
                            ORDER BY  T.CreateDate  ASC LIMIT @startIndex,@endIndex; ");
            var rWhere = new StringBuilder();
            var oWhere = new StringBuilder();
            var strWhere = new StringBuilder();
            var purchaseCountWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (request.StartTime.HasValue)
                {
                    rWhere.Append("  AND r.ReceiptDate >= @StartTime");
                    oWhere.Append("  AND o.ActualShipDate >= @StartTime");
                    purchaseCountWhere.Append("  AND r.ReceiptDate >= @StartTime");
                    mySqlParameter.Add(new MySqlParameter("@StartTime", Convert.ToDateTime(request.StartTime).ToString(PublicConst.StartDateFormat)));

                }
                if (request.EndTime.HasValue)
                {
                    rWhere.Append("  AND r.ReceiptDate <=@EndTime");
                    oWhere.Append("   AND o.ActualShipDate <=@EndTime");
                    purchaseCountWhere.Append("  AND r.ReceiptDate <= @EndTime");
                    mySqlParameter.Add(new MySqlParameter("@EndTime", Convert.ToDateTime(request.EndTime).ToString(PublicConst.EndDateFormat)));
                }
                #region 仓库过滤
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    strWhere.Append("AND T.WareHouseSysId = @WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }
                #endregion

                if (!string.IsNullOrEmpty(request.WareHouseArea))
                {
                    strWhere.Append(" AND  w.WareHouseArea = @WareHouseArea");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseArea", request.WareHouseArea));
                }
                if (!string.IsNullOrEmpty(request.WareHouseProperty))
                {
                    strWhere.Append(" AND  w.WareHouseProperty= @WareHouseProperty");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseProperty", request.WareHouseProperty.Trim()));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
                  .Replace("@receiptWhere", rWhere.ToString())
                  .Replace("@outboundWhere", oWhere.ToString())
                  .Replace("@OutWhere", strWhere.ToString())
                  .Replace("@purchaseCountWhere", purchaseCountWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<ReceivedAndSendSkuGlobalDto>(sql.ToString()
              .Replace("@receiptWhere", rWhere.ToString())
              .Replace("@outboundWhere", oWhere.ToString())
              .Replace("@OutWhere", strWhere.ToString())
              .Replace("@purchaseCountWhere", purchaseCountWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<ReceivedAndSendSkuGlobalDto>();
            response.TableResuls = new TableResults<ReceivedAndSendSkuGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }




        /// <summary>
        /// 进销存报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FinanceInvoicingGlobalDto> GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request)
        {
            //是否有商品查询条件
            bool searchSku = (!string.IsNullOrEmpty(request.SkuCode) || !string.IsNullOrEmpty(request.SkuName) || !string.IsNullOrEmpty(request.UPC) || !string.IsNullOrEmpty(request.OtherId)) ? true : false;

            var countSql = new StringBuilder();
            var sql = new StringBuilder();

            countSql.Append(@"  SELECT COUNT(1) FROM 
                                (
                                    SELECT i.SkuSysId,i.WareHouseSysId
                                    FROM invlotloclpn i  
                                    LEFT JOIN sku s ON i.SkuSysId = s.SysId 
                                    WHERE 1=1 @Where GROUP BY i.SkuSysId,i.WareHouseSysId
                                ) invsku;");

            sql.Append(@"SELECT i.SkuSysId, s.OtherId AS SkuOtherId,s.SkuName,s.UPC,pk.InLabelUnit01, pk.FieldValue01,pk.FieldValue02,w.Name AS WarehouseName,i.WareHouseSysId
                    FROM invlotloclpn i
                    LEFT JOIN warehouse w ON i.WareHouseSysId = w.SysId
                    LEFT JOIN sku s ON i.SkuSysId = s.SysId
                    LEFT JOIN pack AS pk  ON s.PackSysId = pk.SysId
                    WHERE 1=1 @Where GROUP BY i.SkuSysId,i.WareHouseSysId 
                    LIMIT @startIndex,@endIndex;");

            var strSqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (searchSku == true)
            {
                if (!string.IsNullOrEmpty(request.SkuCode))
                {
                    strSqlWhere.Append(" AND s.SkuCode like CONCAT(@SkuCode,'%') ");
                    mySqlParameter.Add(new MySqlParameter("@SkuCode", request.SkuCode.Trim()));
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    strSqlWhere.Append(" AND s.SkuName like CONCAT(@SkuName,'%') ");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    strSqlWhere.Append(" AND s.UPC like CONCAT(@UPC,'%')  ");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (!string.IsNullOrEmpty(request.OtherId))
                {
                    strSqlWhere.Append(" AND s.SkuCode like CONCAT(@SkuCode,'%') ");
                    strSqlWhere.Append(" AND s.OtherId like CONCAT(@OtherId,'%')  ");
                    mySqlParameter.Add(new MySqlParameter("@OtherId", request.OtherId.Trim()));
                }
            }
            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                strSqlWhere.Append(" AND i.WareHouseSysId=@WareHouseSysId ");
                mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
            }

            var count = base.Context.Database.SqlQuery<int>(countSql.ToString().Replace("@Where", strSqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var pageList = base.Context.Database.SqlQuery<FinanceInvoicingGlobalDto>(sql.ToString()
                .Replace("@Where", strSqlWhere.ToString())
                .Replace("@startIndex", request.iDisplayStart.ToString())
                .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).ToList();



            if (pageList != null && pageList.Count > 0)
            {
                var skuCondition = string.Empty;

                if (searchSku == true)
                {
                    skuCondition = "'" + string.Join("','", pageList.Select(x => x.SkuSysId).ToArray()) + "'";
                    skuCondition = string.Format(" AND i.SkuSysId  in ({0})", skuCondition);
                }

                var startTime = Convert.ToDateTime(request.StartTime.ToString(PublicConst.StartDateFormat));
                var endTime = Convert.ToDateTime(request.EndTime.ToString(PublicConst.EndDateFormat));

                var numberSql = new StringBuilder();
                var numberPara = new List<MySqlParameter>();
                string warehouseWhere = string.Empty;
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    warehouseWhere = " AND i.WareHouseSysId=@WareHouseSysId ";
                    numberPara.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }
                numberSql.AppendFormat(@"SELECT  i.SkuSysId,i.WareHouseSysId,
                                  IFNULL(SUM(CASE WHEN ((i.TransType = 'OUT' OR i.TransType = 'IN' OR
                                      i.TransType = 'ASSY' OR i.TransType = 'AJ') AND
                                      i.CreateDate < @StarTime) THEN i.Qty END),0) AS InitialQty,

                                  IFNULL( SUM(CASE WHEN (i.TransType = 'IN'
                                       AND i.CreateDate >= @StarTime) THEN i.Qty END),0) AS CurrentPeriodReceiptQty,

                                  IFNULL( SUM(CASE WHEN (i.TransType = 'OUT'
                                       AND i.CreateDate >= @StarTime) THEN i.Qty END),0) AS CurrentPeriodOutboundQty,

                                  IFNULL( SUM(CASE WHEN (i.TransType = 'ASSY'
                                      AND i.CreateDate >= @StarTime) THEN i.Qty END),0) AS AssemblyQty,

                                  IFNULL( SUM(CASE WHEN (i.TransType ='AJ'
                                      AND i.CreateDate >= @StarTime) THEN i.Qty END),0) AS AdjustmentQty

                                FROM invtrans i 
                                WHERE   i.Status <> 'CANCEL'  AND i.CreateDate <= @EndTime
                                {1}
                                {0}
                                GROUP BY i.SkuSysId,i.WareHouseSysId;", skuCondition, warehouseWhere);

                numberPara.Add(new MySqlParameter("@StarTime", startTime));
                numberPara.Add(new MySqlParameter("@EndTime", endTime));

                var allList = base.Context.Database.SqlQuery<FinanceInvoicingGlobalDto>(numberSql.ToString(), numberPara.ToArray()).ToList();
                if (allList != null && allList.Count > 0)
                {
                    foreach (var model in pageList)
                    {
                        var item = allList.First(a => a.SkuSysId == model.SkuSysId && a.WareHouseSysId == model.WareHouseSysId);

                        if (item != null)
                        {
                            model.InitialQty = item.InitialQty;
                            model.CurrentPeriodReceiptQty = item.CurrentPeriodReceiptQty;
                            model.CurrentPeriodOutboundQty = item.CurrentPeriodOutboundQty;
                            model.AssemblyQty = item.AssemblyQty;
                            model.AdjustmentQty = item.AdjustmentQty;

                            //数量计算
                            model.EndingInventoryQty = model.InitialQty + model.CurrentPeriodReceiptQty + model.CurrentPeriodOutboundQty + model.AssemblyQty + model.AdjustmentQty;

                            model.DisplayInitialQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.InitialQty * 1.00m) / model.FieldValue01.Value), 3) : model.InitialQty;

                            model.DisplayCurrentPeriodReceiptQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.CurrentPeriodReceiptQty * 1.00m) / model.FieldValue01.Value), 3) : model.CurrentPeriodReceiptQty;

                            model.DisplayCurrentPeriodOutboundQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.CurrentPeriodOutboundQty * 1.00m) / model.FieldValue01.Value), 3) : model.CurrentPeriodOutboundQty;

                            model.DisplayEndingInventoryQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.EndingInventoryQty * 1.00m) / model.FieldValue01.Value), 3) : model.EndingInventoryQty;

                            model.DisplayAssemblyQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.AssemblyQty * 1.00m) / model.FieldValue01.Value), 3) : model.AssemblyQty;


                            model.DisplayAdjustmentQty = model.InLabelUnit01.HasValue && model.InLabelUnit01.Value == true
                                        && model.FieldValue01 > 0 && model.FieldValue02 > 0
                                        ? Math.Round(((model.FieldValue02.Value * model.AdjustmentQty * 1.00m) / model.FieldValue01.Value), 3) : model.AdjustmentQty;

                        }
                    }
                }
            }

            var response = new Pages<FinanceInvoicingGlobalDto>();

            response.TableResuls = new TableResults<FinanceInvoicingGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 损益明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<AdjustmentDetailGlobalDto> GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request)
        {
            var sysCodeTable = from sysCode in Context.syscodes
                               join detail in Context.syscodedetails on sysCode.SysId equals detail.SysCodeSysId
                               where sysCode.SysCodeType.Equals(PublicConst.AdjustmentLevel, StringComparison.OrdinalIgnoreCase)
                                     && detail.IsActive == true
                               select new
                               {
                                   detail.Code,
                                   detail.Descr
                               };
            Pages<AdjustmentDetailGlobalDto> response = new Pages<AdjustmentDetailGlobalDto>();

            var query = from adjustmentdetail in Context.adjustmentdetails
                        join adjustment in Context.adjustments on adjustmentdetail.AdjustmentSysId equals adjustment.SysId
                        join wh in Context.warehouses on adjustment.WareHouseSysId equals wh.SysId
                        join it in Context.invtrans on new { AdjustmentSysId = adjustment.SysId, AdjustmentDetailSysId = adjustmentdetail.SysId } equals new { AdjustmentSysId = it.DocSysId, AdjustmentDetailSysId = it.DocDetailSysId }
                        join lot in Context.invlots on new { WarehouseSysId = it.WareHouseSysId, Lot = it.Lot } equals new { WarehouseSysId = lot.WareHouseSysId, Lot = lot.Lot }
                        join sku in Context.skus on adjustmentdetail.SkuSysId equals sku.SysId
                        join syscode in sysCodeTable on adjustmentdetail.AdjustlevelCode equals syscode.Code
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        where adjustment.Status == (int)AdjustmentStatus.Audit
                        select new
                        {
                            adjustmentdetail,
                            adjustment.WareHouseSysId,
                            it.Lot,
                            lot.LotAttr01,
                            sku.SkuCode,
                            sku.SkuName,
                            sku.SkuDescr,
                            sku.UPC,
                            sysCode = syscode.Code,
                            sysCodeDescription = syscode.Descr,
                            p1,
                            WarehouseName = wh.Name
                        };

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                query = query.Where(p => p.WareHouseSysId == request.SearchWarehouseSysId);
            }

            if (!request.AdjustmentLevelCode.IsNull())
            {
                query = query.Where(p => p.adjustmentdetail.AdjustlevelCode == request.AdjustmentLevelCode);
            }

            if (request.CreateDateFrom.HasValue)
            {
                query = query.Where(p => p.adjustmentdetail.CreateDate > request.CreateDateFrom.Value);
            }

            if (request.CreateDateTo.HasValue)
            {
                query = query.Where(p => p.adjustmentdetail.CreateDate <= request.CreateDateTo.Value);
            }

            var adjustmendDetails = query.Select(p => new AdjustmentDetailGlobalDto()
            {
                SysId = p.adjustmentdetail.SysId,
                SkuSysId = p.adjustmentdetail.SkuSysId.Value,
                SkuCode = p.SkuCode,
                SkuName = p.SkuName,
                SkuDescr = p.SkuDescr,
                UPC = p.UPC,
                CreateDate = p.adjustmentdetail.CreateDate,
                CreateUserName = p.adjustmentdetail.CreateUserName,
                AdjustmentLevelDescription = p.sysCodeDescription,
                Lot = p.Lot,
                LotAttr01 = p.LotAttr01,
                Loc = p.adjustmentdetail.Loc,
                Qty = p.adjustmentdetail.Qty,
                DisplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.adjustmentdetail.Qty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.adjustmentdetail.Qty,
                WarehouseName = p.WarehouseName
            }).Distinct();
            request.iTotalDisplayRecords = adjustmendDetails.Count();
            adjustmendDetails =
                adjustmendDetails.OrderByDescending(p => p.CreateDate)
                    .Skip(request.iDisplayStart)
                    .Take(request.iDisplayLength);
            response = ConvertPages(adjustmendDetails, request);

            return response;
        }

        /// <summary>
        /// 入库汇总查询
        /// </summary>
        /// <param name="inboundReportQuery"></param>
        /// <returns></returns>
        public Pages<InboundGlobalDto> GetInboundReport(InboundGlobalQuery inboundReportQuery)
        {

            var countSql = new StringBuilder();
            countSql.Append(@"SELECT  COUNT(*) FROM purchase p WHERE 1 = 1 @where");

            var listSql = new StringBuilder();
            listSql.Append(@"SELECT p.SysId,p.PurchaseOrder,p.Status,p.LastReceiptDate,p.Type AS PurchaseType,
                              CASE WHEN ((((p2.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p2.InLabelUnit01) AND
                                  (p2.InLabelUnit01 IS NOT NULL))) AND
                                  (p2.FieldValue01 > 0)) AND
                                  (p2.FieldValue01 > 0)) THEN (ROUND(((p2.FieldValue02 *  SUM(p1.ReceivedQty)) * 1.00) / (p2.FieldValue01), 3)) ELSE  SUM(p1.ReceivedQty) END AS DisplayReceivedQty,
  
                              CASE WHEN ((((p2.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p2.InLabelUnit01) AND
                                  (p2.InLabelUnit01 IS NOT NULL))) AND
                                  (p2.FieldValue01 > 0)) AND
                                  (p2.FieldValue01 > 0)) THEN (ROUND(((p2.FieldValue02 *  SUM(p1.RejectedQty)) * 1.00) / (p2.FieldValue01), 3)) ELSE  SUM(p1.RejectedQty) END AS DisplayRejectedQty,
                                wh.Name AS WarehouseName

                            FROM purchase p
                              LEFT JOIN purchasedetail p1    ON p.SysId = p1.PurchaseSysId
                              LEFT JOIN sku s    ON p1.SkuSysId = s.SysId
                              LEFT JOIN pack p2    ON s.PackSysId = p2.SysId
                              LEFT JOIN warehouse wh    ON p.WarehouseSysId = wh.SysId
                            WHERE 1 = 1 @where
                            GROUP BY p.SysId,p.PurchaseOrder
                            ORDER BY p.LastReceiptDate DESC   LIMIT @startIndex,@endIndex;");

            var strWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (inboundReportQuery != null)
            {
                if (inboundReportQuery.SearchWarehouseSysId != Guid.Empty)
                {
                    strWhere.Append(" AND p.WarehouseSysId=@WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", inboundReportQuery.SearchWarehouseSysId));
                }
                if (!inboundReportQuery.PurchaseOrderSearch.IsNull())
                {
                    strWhere.Append(" AND p.PurchaseOrder=@PurchaseOrder");
                    mySqlParameter.Add(new MySqlParameter("@PurchaseOrder", inboundReportQuery.PurchaseOrderSearch.Trim()));
                }
                if (inboundReportQuery.PurchaseTypeSearch.HasValue)
                {
                    strWhere.Append(" AND p.Type=@Type");
                    mySqlParameter.Add(new MySqlParameter("@Type", inboundReportQuery.PurchaseTypeSearch.Value));
                }
                if (inboundReportQuery.StartDateSearch.HasValue)
                {
                    strWhere.AppendFormat(" AND p.LastReceiptDate>=@StartDateSearch");
                    mySqlParameter.Add(new MySqlParameter("@StartDateSearch", inboundReportQuery.StartDateSearch));
                }
                if (inboundReportQuery.EndDateSearch.HasValue)
                {
                    inboundReportQuery.EndDateSearch = inboundReportQuery.EndDateSearch.Value.AddDays(1).AddMilliseconds(-1);
                    strWhere.AppendFormat(" AND p.LastReceiptDate<=@EndDateSearch", inboundReportQuery.EndDateSearch);
                    mySqlParameter.Add(new MySqlParameter("@EndDateSearch", inboundReportQuery.EndDateSearch));
                }
            }
            var count = base.Context.Database.SqlQuery<int>(countSql.ToString().Replace("@where", strWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<InboundGlobalDto>(listSql.ToString()
                .Replace("@where", strWhere.ToString())
                .Replace("@startIndex", inboundReportQuery.iDisplayStart.ToString())
                .Replace("@endIndex", inboundReportQuery.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<InboundGlobalDto>();
            response.TableResuls = new TableResults<InboundGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = inboundReportQuery.sEcho
            };
            return response;
        }

        /// <summary>
        /// 移仓单报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<TransferinventoryGlobalDto> GetTransferinventoryReport(TransferinventoryGlobalQuery request)
        {
            var sql = new StringBuilder();
            sql.Append(" SELECT ti.TransferInventoryOrder,w.Name AS FromWareHouseName,w1.Name as ToWareHouseName,ti.TransferPurchaseOrder,ti.TransferOutboundOrder,ti.Status,");
            sql.Append(" ti.ExternOrderId, p.UPdateUserName AS RUpdateUserName, o.UpdateUserName as OUpdateUserName");
            sql.Append(" FROM transferinventory ti");
            sql.Append(" LEFT JOIN warehouse w ON w.SysId = ti.FromWareHouseSysId");
            sql.Append(" LEFT JOIN warehouse w1 ON w1.SysId = ti.ToWareHouseSysId");
            sql.Append(" LEFT JOIN purchase p on p.sysid = ti.TransferPurchaseSysId");
            sql.Append(" LEFT JOIN outbound o ON o.sysid = ti.TransferOutboundSysId WHERE 1=1 @Where ORDER BY ti.TransferInventoryOrder DESC");
            sql.AppendFormat(" LIMIT {0},{1} ", request.iDisplayStart, request.iDisplayLength);


            var countSql = new StringBuilder();
            countSql.Append(@" SELECT COUNT(*)  FROM  transferinventory  ti 
                            WHERE  1 = 1 @Where");

            var strWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(request.TransferInventoryOrder))
            {
                strWhere.Append(" AND ti.TransferInventoryOrder=@TransferInventoryOrder");
                mySqlParameter.Add(new MySqlParameter("@TransferInventoryOrder", request.TransferInventoryOrder.Trim()));
            }
            if (request.FromWareHouseSysId != Guid.Empty)
            {
                strWhere.Append(" AND ti.FromWareHouseSysId=@FromWareHouseSysId");
                mySqlParameter.Add(new MySqlParameter("@FromWareHouseSysId", request.FromWareHouseSysId));
            }
            if (request.ToWareHouseSysId != Guid.Empty)
            {
                strWhere.Append(" AND ti.ToWareHouseSysId=@ToWareHouseSysId");
                mySqlParameter.Add(new MySqlParameter("@ToWareHouseSysId", request.ToWareHouseSysId));
            }
            if (!string.IsNullOrEmpty(request.TransferPurchaseOrder))
            {
                strWhere.Append(" AND ti.TransferPurchaseOrder=@TransferPurchaseOrder");
                mySqlParameter.Add(new MySqlParameter("@TransferPurchaseOrder", request.TransferPurchaseOrder.Trim()));
            }
            if (!string.IsNullOrEmpty(request.TransferOutboundOrder))
            {
                strWhere.Append(" AND ti.TransferOutboundOrder=@TransferOutboundOrder");
                mySqlParameter.Add(new MySqlParameter("@TransferOutboundOrder", request.TransferOutboundOrder.Trim()));
            }
            if (request.Status.HasValue)
            {
                strWhere.Append(" AND ti.Status=@Status");
                mySqlParameter.Add(new MySqlParameter("@Status", request.Status));
            }
            if (!string.IsNullOrEmpty(request.ExternOrderId))
            {
                strWhere.Append(" AND ti.ExternOrderId=@ExternOrderId");
                mySqlParameter.Add(new MySqlParameter("@ExternOrderId", request.ExternOrderId.Trim()));
            }

            var count = base.Context.Database.SqlQuery<int>(countSql.ToString().Replace("@Where", strWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().FirstOrDefault();

            var queryList = base.Context.Database.SqlQuery<TransferinventoryGlobalDto>(sql.ToString().Replace("@Where", strWhere.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();

            var response = new Pages<TransferinventoryGlobalDto>();
            response.TableResuls = new TableResults<TransferinventoryGlobalDto>()
            {
                aaData = queryList.ToList(),
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 冻结商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FrozenSkuGlobalDto> GetFrozenSkuReport(FrozenSkuGlobalQuery request)
        {

            string whWhere = request.SearchWarehouseSysId == Guid.Empty ? string.Empty : " AND sf.WarehouseSysId = '" + request.SearchWarehouseSysId + "'";
            string zoneSql = $@"
                                    SELECT 
                                      i.SkuSysId,
                                      s.UPC,
                                      s.SkuName,
                                      i.Loc,
                                      i.Lot,
                                      lot.LotAttr01,
                                      i.Qty,
                                      i.FrozenQty,
                                      wh.Name AS WarehouseName
                                    FROM stockfrozen sf
                                    JOIN warehouse wh ON sf.WarehouseSysId = wh.SysId
                                    JOIN location l ON sf.ZoneSysId = l.ZoneSysId AND sf.WarehouseSysId = l.WarehouseSysId
                                    JOIN invlotloclpn i ON l.Loc = i.Loc AND l.WarehouseSysId = i.WareHouseSysId
                                    JOIN invlot lot on i.Lot = lot.Lot and i.WarehouseSysId = lot.WarehouseSysId
                                    JOIN sku s ON i.SkuSysId = s.SysId
                                    WHERE sf.Type = {(int)FrozenType.Zone}
                                      AND sf.Status = {(int)FrozenStatus.Frozen}
                                      {whWhere}";

            string locationSql = $@"
                                        SELECT 
                                          i.SkuSysId,
                                          s.UPC,
                                          s.SkuName,
                                          i.Loc,
                                          i.Lot,
                                          lot.LotAttr01,
                                          i.Qty,
                                          i.FrozenQty,
                                          wh.Name AS WarehouseName
                                        FROM stockfrozen sf
                                        JOIN warehouse wh ON sf.WarehouseSysId = wh.SysId
                                        JOIN invlotloclpn i ON sf.Loc = i.Loc AND sf.WarehouseSysId = i.WareHouseSysId
                                        JOIN invlot lot on i.Lot = lot.Lot and i.WarehouseSysId = lot.WarehouseSysId
                                        JOIN sku s ON i.SkuSysId = s.SysId
                                        WHERE sf.Type = {(int)FrozenType.Location}
                                          AND sf.Status = {(int)FrozenStatus.Frozen}
                                          {whWhere}";

            string skuSql = $@"
                                    SELECT 
                                      i.SkuSysId,
                                      s.UPC,
                                      s.SkuName,
                                      i.Loc,
                                      i.Lot,
                                      lot.LotAttr01,
                                      i.Qty,
                                      i.FrozenQty,
                                      wh.Name AS WarehouseName 
                                    FROM stockfrozen sf
                                    JOIN warehouse wh ON sf.WarehouseSysId = wh.SysId
                                    JOIN invlotloclpn i ON sf.SkuSysId = i.SkuSysId AND sf.WarehouseSysId = i.WareHouseSysId
                                    JOIN invlot lot on i.Lot = lot.Lot and i.WarehouseSysId = lot.WarehouseSysId
                                    JOIN sku s ON i.SkuSysId = s.SysId
                                    WHERE sf.Type = {(int)FrozenType.Sku}
                                      AND sf.Status = {(int)FrozenStatus.Frozen}
                                      {whWhere}";

            string locskuSql = $@"
                                            SELECT 
                                              i.SkuSysId,
                                              s.UPC,
                                              s.SkuName,
                                              i.Loc,
                                              i.Lot,
                                              lot.LotAttr01,
                                              i.Qty,
                                              i.FrozenQty,
                                              wh.Name AS WarehouseName 
                                            FROM stockfrozen sf
                                            JOIN warehouse wh ON sf.WarehouseSysId = wh.SysId
                                            JOIN invlotloclpn i ON sf.SkuSysId = i.SkuSysId AND sf.Loc = i.Loc AND sf.WarehouseSysId = i.WareHouseSysId
                                            JOIN invlot lot on i.Lot = lot.Lot and i.WarehouseSysId = lot.WarehouseSysId
                                            JOIN sku s ON i.SkuSysId = s.SysId
                                            WHERE sf.Type = {(int)FrozenType.LocSku}
                                              AND sf.Status = {(int)FrozenStatus.Frozen}
                                              {whWhere}";

            StringBuilder conditionSb = new StringBuilder();

            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();

                conditionSb.Append($" AND s.UPC = '{request.UPC}'");
            }

            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();

                conditionSb.Append($" AND s.SkuName LIKE '{request.SkuName}%'");
            }

            if (request.IsStoreZero == true)
            {
                conditionSb.Append($" AND i.Qty > 0");
            }

            string conditionSql = conditionSb.ToString();
            if (!string.IsNullOrEmpty(conditionSql))
            {
                zoneSql = zoneSql + conditionSql;
                locationSql = locationSql + conditionSql;
                skuSql = skuSql + conditionSql;
                locskuSql = locskuSql + conditionSql;
            }

            StringBuilder searchCountSb = new StringBuilder();
            searchCountSb.AppendLine(@"
                SELECT COUNT(1) 
                FROM (
            ");
            searchCountSb.AppendLine(zoneSql);
            searchCountSb.AppendLine("UNION");
            searchCountSb.AppendLine(locationSql);
            searchCountSb.AppendLine("UNION");
            searchCountSb.AppendLine(skuSql);
            searchCountSb.AppendLine("UNION");
            searchCountSb.AppendLine(locskuSql);
            searchCountSb.AppendLine($@"
                ) result;
            ");

            StringBuilder searchSb = new StringBuilder();
            searchSb.AppendLine(@"
                SELECT 
                    SkuSysId,
                    UPC,
                    SkuName,
                    Loc,
                    Lot,
                    LotAttr01,
                    Qty,
                    FrozenQty,
                    WarehouseName 
                FROM (
            ");
            searchSb.AppendLine(zoneSql);
            searchSb.AppendLine("UNION");
            searchSb.AppendLine(locationSql);
            searchSb.AppendLine("UNION");
            searchSb.AppendLine(skuSql);
            searchSb.AppendLine("UNION");
            searchSb.AppendLine(locskuSql);
            searchSb.AppendLine($@"
                ) result
            ORDER BY result.UPC,result.Loc
            LIMIT {request.iDisplayStart},{request.iDisplayLength};
            ");

            var count = base.Context.Database.SqlQuery<int>(searchCountSb.ToString()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<FrozenSkuGlobalDto>(searchSb.ToString()).AsQueryable();

            var pageList = queryList.ToList();

            var skuSysIds = pageList.Select(x => x.SkuSysId).Distinct();
            var skuList = from s in Context.skus
                          join p in Context.packs on s.PackSysId equals p.SysId
                          where skuSysIds.Contains(s.SysId)
                          select new { s, p };

            var list = from a in pageList
                       join b in skuList on a.SkuSysId equals b.s.SysId

                       select new FrozenSkuGlobalDto
                       {
                           SkuSysId = a.SkuSysId,
                           UPC = a.UPC,
                           SkuName = a.SkuName,
                           Qty = a.Qty,
                           Loc = a.Loc,
                           Lot = a.Lot,
                           LotAttr01 = a.LotAttr01,
                           DisplayQty = b.p.InLabelUnit01.HasValue && b.p.InLabelUnit01.Value == true
                               && b.p.FieldValue01 > 0 && b.p.FieldValue02 > 0
                               ? Math.Round(((b.p.FieldValue02.Value * a.Qty * 1.00m) / b.p.FieldValue01.Value), 3) : a.Qty,
                           WarehouseName = a.WarehouseName
                       };

            var response = new Pages<FrozenSkuGlobalDto>();
            response.TableResuls = new TableResults<FrozenSkuGlobalDto>()
            {
                aaData = list.ToList(),
                iTotalDisplayRecords = count,
                iTotalRecords = list.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundHandleDateStatisticsGlobalDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(1)
                              FROM outbound o
                              WHERE o.Status IN (10,20,30,35,40,70) @CountWhere ");

            var sql = new StringBuilder();
            sql.Append(@"SELECT o.OutboundOrder,o.OutboundType,o.CreateDate,  
                                (SELECT p.PickDate FROM pickdetail p WHERE p.Status != -999 and p.OutboundSysId = 
                                o.SysId ORDER by p.PickDate DESC LIMIT 1) AS PickDate,  
                                (SELECT vd.CreateDate FROM vanningdetail vd
                                INNER JOIN vanning v ON v.sysid = vd.VanningSysId WHERE v.Status != -999 
                                and v.OutboundSysId = o.SysId
                                ORDER by vd.CreateDate desc limit 1) AS VanningDate,
                                o.ActualShipDate,o.Status,wh.Name AS WarehouseName
                                FROM outbound o
                                LEFT JOIN warehouse wh ON o.WarehouseSysId = wh.SysId
                                WHERE o.Status IN (10,20,30,35,40,70)
                                @SQLWhere
                                ORDER BY o.OutboundOrder DESC LIMIT  @startIndex,@endIndex;");
            var countWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    countWhere.AppendFormat(" AND o.OutboundOrder = @OutboundOrder");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
                }
                if (request.Status.HasValue)
                {
                    countWhere.Append(" AND o.Status=@Status");
                    mySqlParameter.Add(new MySqlParameter("@Status", request.Status));
                }
                if (request.OutboundType.HasValue)
                {
                    countWhere.Append(" AND o.OutboundType=@OutboundType");
                    mySqlParameter.Add(new MySqlParameter("@OutboundType", request.OutboundType));
                }
                if (request.CreateDateFrom.HasValue)
                {
                    countWhere.Append(" AND o.CreateDate>=@CreateDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@CreateDateFrom", request.CreateDateFrom));
                }
                if (request.CreateDateTo.HasValue)
                {
                    countWhere.Append(" AND o.CreateDate<=@CreateDateTo");
                    mySqlParameter.Add(new MySqlParameter("@CreateDateTo", request.CreateDateTo));
                }
                if (request.ActualShipDateFrom.HasValue)
                {
                    countWhere.Append(" AND o.ActualShipDate>=@ActualShipDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom));
                }
                if (request.ActualShipDateTo.HasValue)
                {
                    countWhere.Append(" AND o.ActualShipDate<=@ActualShipDateTo");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo));
                }
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    countWhere.Append(" AND o.WareHouseSysId=@WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }
            }
            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
              .Replace("@CountWhere", countWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundHandleDateStatisticsGlobalDto>(sql.ToString()
              .Replace("@SQLWhere", countWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundHandleDateStatisticsGlobalDto>();
            response.TableResuls = new TableResults<OutboundHandleDateStatisticsGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceiptAndDeliveryDateGlobalDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(1) FROM ( SELECT
                              p.PurchaseOrder 
                              FROM purchase p
                              LEFT JOIN receipt r  ON r.ExternalOrder = p.PurchaseOrder
                              LEFT JOIN receiptdetail rd  ON rd.ReceiptSysId = r.SysId
                              WHERE p.Status IN (-10,10, 20, 30)
                              AND r.Status IN (10, 20, 30, 40)
                              @CountWhere
                              GROUP BY p.PurchaseOrder,p.AuditingDate,
                                     r.ReceiptOrder,r.CreateDate,r.ReceiptDate
                              ORDER BY p.PurchaseOrder DESC) A");

            var sql = new StringBuilder();
            sql.Append(@"SELECT p.PurchaseOrder, p.Type,p.AuditingDate,r.ReceiptOrder,r.CreateDate,
                               IFNULL(SUM(
                               CASE when p1.InLabelUnit01 IS NOT NULL AND p1.InLabelUnit01 = 1
                               AND p1.FieldValue01 > 0 AND p1.FieldValue02 > 0
                               THEN ROUND(p1.FieldValue02 * (IFNULL(rd.ReceivedQty,0)/p1.FieldValue01),3)
                               ELSE IFNULL(rd.ReceivedQty,0) END),0) TotalReceivedQty ,
                                r.ReceiptDate,
                               IFNULL(SUM(
                               CASE when p1.InLabelUnit01 IS NOT NULL AND p1.InLabelUnit01 = 1
                               AND p1.FieldValue01 > 0 AND p1.FieldValue02 > 0
                               THEN ROUND(p1.FieldValue02 * (IFNULL(rd.ShelvesQty,0)/p1.FieldValue01),3)
                               ELSE IFNULL(rd.ShelvesQty,0) END),0) TotalShelvesQty ,
                                (SELECT i.CreateDate FROM invtrans  i WHERE i.DocSysId = r.SysId AND i.Status = 'OK' 
                                order by i.CreateDate DESC LIMIT 1) AS ShelvesDate,wh.Name AS WarehouseName
                                FROM purchase p
                                LEFT JOIN warehouse wh ON p.WarehouseSysId = wh.SysId
                                LEFT JOIN receipt r ON r.ExternalOrder = p.PurchaseOrder
                                LEFT JOIN receiptdetail rd   ON rd.ReceiptSysId = r.SysId
                                LEFT JOIN sku s ON rd.SkuSysId= s.SysId
                                LEFT JOIN pack p1 ON s.PackSysId=p1.SysId
                                WHERE p.Status IN (-10,10, 20, 30)
                                AND r.Status IN (10, 20, 30, 40)
                                @SqlWhere
                                GROUP BY p.PurchaseOrder,p.AuditingDate,r.ReceiptOrder,r.CreateDate,r.ReceiptDate
                                ORDER BY p.PurchaseOrder DESC LIMIT  @startIndex,@endIndex;");
            var countWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.PurchaseOrder))
                {
                    countWhere.AppendFormat(" AND p.PurchaseOrder = @PurchaseOrder");
                    mySqlParameter.Add(new MySqlParameter("@PurchaseOrder", request.PurchaseOrder.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ReceiptOrder))
                {
                    countWhere.AppendFormat(" AND r.ReceiptOrder= @ReceiptOrder");
                    mySqlParameter.Add(new MySqlParameter("@ReceiptOrder", request.ReceiptOrder.Trim()));
                }
                if (request.CreateDateFrom.HasValue)
                {
                    countWhere.AppendFormat(" AND r.CreateDate >=@CreateDate", request.CreateDateFrom);
                    mySqlParameter.Add(new MySqlParameter("@CreateDate", request.CreateDateFrom));
                }
                if (request.CreateDateTo.HasValue)
                {
                    countWhere.AppendFormat(" AND r.CreateDate <= @CreateDateTo");
                    mySqlParameter.Add(new MySqlParameter("@CreateDateTo", request.CreateDateTo));
                }
                if (request.AuditingDateFrom.HasValue)
                {
                    countWhere.AppendFormat(" AND p.AuditingDate >=@AuditingDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@AuditingDateFrom", request.AuditingDateFrom));
                }
                if (request.AuditingDateTo.HasValue)
                {
                    countWhere.AppendFormat(" AND p.AuditingDate <= @AuditingDateTo");
                    mySqlParameter.Add(new MySqlParameter("@AuditingDateTo", request.AuditingDateTo));
                }
                if (request.Type.HasValue)
                {
                    countWhere.AppendFormat(" AND p.Type=@Type", request.Type);
                    mySqlParameter.Add(new MySqlParameter("@Type", request.Type));
                }
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    countWhere.AppendFormat(" AND p.WarehouseSysId = @WarehouseSysId");
                    countWhere.AppendFormat(" AND r.WarehouseSysId = @WarehouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WarehouseSysId", request.SearchWarehouseSysId));
                }
            }
            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
             .Replace("@CountWhere", countWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<ReceiptAndDeliveryDateGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", countWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<ReceiptAndDeliveryDateGlobalDto>();
            response.TableResuls = new TableResults<ReceiptAndDeliveryDateGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;

        }


        /// <summary>
        /// SN管理报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<SNManageGlobalDto> GetSNManageReport(SNManageGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append($@"
                    SELECT COUNT(1)
                    FROM receiptsn rs
                    INNER JOIN purchase p
                      ON rs.purchaseSysId = p.SysId
                    INNER JOIN receipt r
                      ON rs.ReceiptSysId = r.SysId
                    LEFT JOIN outbound o
                      ON rs.OutboundSysId = o.SysId
                    WHERE rs.WarehouseSysId = '{request.WarehouseSysId}'
                    @CountWhere;");

            var sql = new StringBuilder();
            sql.Append($@"
                            SELECT 
                              rs.SysId,
                              rs.SN,
                              rs.status,
                              p.PurchaseOrder,
                              r.ReceiptDate as PurchaseDate,
                              o.OutboundOrder,
                              o.ActualShipDate as OutboundDate,
                              o.ConsigneeName,
                              o.ConsigneePhone,
                              o.ConsigneeAddress,
                              wh.Name AS WarehouseName
                            FROM receiptsn rs
                            INNER JOIN purchase p
                              ON rs.purchaseSysId = p.SysId
                            LEFT JOIN warehouse wh ON p.WarehouseSysId = wh.SysId
                            INNER JOIN receipt r
                              ON rs.ReceiptSysId = r.SysId
                            LEFT JOIN outbound o
                              ON rs.OutboundSysId = o.SysId
                            WHERE rs.WarehouseSysId = '{request.WarehouseSysId}'
                            @CountWhere
                            ORDER BY r.ReceiptDate DESC
                            LIMIT  @startIndex,@endIndex;
                            ");

            var countWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.SN))
                {
                    countWhere.AppendFormat(" AND rs.SN = @SN");
                    mySqlParameter.Add(new MySqlParameter("@SN", request.SN.Trim()));
                }

                if (!string.IsNullOrEmpty(request.PurchaseOrder))
                {
                    countWhere.AppendFormat(" AND p.PurchaseOrder = @PurchaseOrder");
                    mySqlParameter.Add(new MySqlParameter("@PurchaseOrder", request.PurchaseOrder.Trim()));
                }

                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    countWhere.AppendFormat(" AND o.OutboundOrder = @OutboundOrder");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
                }

                if (request.Status > 0)
                {
                    countWhere.AppendFormat(" AND rs.status = @Status");
                    mySqlParameter.Add(new MySqlParameter("@Status", request.Status));
                }

                if (request.PurchaseDateFrom.HasValue)
                {
                    countWhere.AppendFormat(" AND r.ReceiptDate > @PurchaseDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@PurchaseDateFrom", request.PurchaseDateFrom.Value));
                }

                if (request.PurchaseDateTo.HasValue)
                {
                    countWhere.AppendFormat(" AND r.ReceiptDate < @PurchaseDateTo");
                    mySqlParameter.Add(new MySqlParameter("@PurchaseDateTo", request.PurchaseDateTo.Value));
                }

                if (request.OutboundDateFrom.HasValue)
                {
                    countWhere.AppendFormat(" AND o.ActualShipDate >@OutboundDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@OutboundDateFrom", request.OutboundDateFrom.Value));
                }

                if (request.OutboundDateTo.HasValue)
                {
                    countWhere.AppendFormat(" AND o.ActualShipDate <@OutboundDateTo");
                    mySqlParameter.Add(new MySqlParameter("@OutboundDateTo", request.OutboundDateTo.Value));
                }

                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    countWhere.AppendFormat(" AND p.WarehouseSysId = @WarehouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WarehouseSysId", request.SearchWarehouseSysId));
                }

            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
             .Replace("@CountWhere", countWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<SNManageGlobalDto>(sql.ToString()
              .Replace("@CountWhere", countWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<SNManageGlobalDto>();
            response.TableResuls = new TableResults<SNManageGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;

        }

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundExceptionGlobalDto> GetOutboundExceptionReport(OutboundExceptionGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"  SELECT COUNT(1) FROM outboundexception o
                                LEFT JOIN outbounddetail o1 ON o.OutboundDetailSysId=o1.SysId
                                LEFT JOIN outbound o2 ON o1.OutboundSysId=o2.SysId
                                LEFT JOIN sku s ON o1.SkuSysId=s.SysId
                                WHERE 1=1  @SqlWhere;");


            var sql = new StringBuilder();
            sql.Append(@" SELECT o2.ActualShipDate,o2.ConsigneeCity,o2.ConsigneeArea,o2.ConsigneeTown,
                                 o2.ServiceStationCode,o2.ServiceStationName,
                                 s.SkuName,s.UPC,u.UOMCode,o.ExceptionReason,
                                 IFNULL(o.ExceptionQty,0) AS ExceptionQty,o.ExceptionDesc,
                                 o.Result,o.Department,o.Responsibility,o.Remark,o.IsSettlement,wh.Name as WarehouseName
                          FROM outboundexception o
                          LEFT JOIN outbounddetail o1 ON o.OutboundDetailSysId=o1.SysId
                          LEFT JOIN outbound o2 ON o1.OutboundSysId=o2.SysId
                          left join warehouse wh on o2.WarehouseSysId = wh.SysId
                          LEFT JOIN sku s ON o1.SkuSysId=s.SysId
                          LEFT JOIN uom u ON o1.UOMSysId =u.SysId
                          WHERE 1=1 @SqlWhere
                          ORDER BY o2.ActualShipDate DESC
                          LIMIT  @startIndex,@endIndex;");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.AppendFormat(" AND o2.WareHouseSysId =@WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }

                if (!string.IsNullOrEmpty(request.ConsigneeCity))
                {
                    sqlWhere.AppendFormat(" AND o2.ConsigneeCity LIKE CONCAT(@ConsigneeCity,'%')  ");
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeCity", request.ConsigneeCity.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ConsigneeArea))
                {
                    sqlWhere.AppendFormat(" AND o2.ConsigneeArea LIKE CONCAT(@ConsigneeArea,'%')", request.ConsigneeArea.Trim());
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeArea", request.ConsigneeArea.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ConsigneeTown))
                {
                    sqlWhere.AppendFormat(" AND o2.ConsigneeTown LIKE CONCAT(@ConsigneeTown,'%')");
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeTown", request.ConsigneeTown.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ServiceStationName))
                {
                    sqlWhere.AppendFormat(" AND o2.ServiceStationName LIKE CONCAT(@ServiceStationName,'%')", request.ServiceStationName.Trim());
                    mySqlParameter.Add(new MySqlParameter("@ServiceStationName", request.ServiceStationName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ServiceStationCode))
                {
                    sqlWhere.AppendFormat(" AND o2.ServiceStationCode LIKE CONCAT(@ServiceStationCode,'%')");
                    mySqlParameter.Add(new MySqlParameter("@ServiceStationCode", request.ServiceStationCode.Trim()));

                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhere.AppendFormat(" AND s.SkuName LIKE  CONCAT(@SkuName,'%')", request.SkuName.Trim());
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhere.AppendFormat(" AND s.UPC = @UPC");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (request.IsSettlement.HasValue)
                {
                    if (request.IsSettlement.Value)
                    {
                        sqlWhere.Append(" AND o.IsSettlement = 1");
                    }
                    else
                    {
                        sqlWhere.Append(" AND ifnull(o.IsSettlement,'') != '1'");
                    }
                }
                if (request.StartTime.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o2.ActualShipDate > @StartTime");
                    mySqlParameter.Add(new MySqlParameter("@StartTime", request.StartTime.Value));
                }
                if (request.EndTime.HasValue)
                {
                    request.EndTime = request.EndTime.Value.AddDays(1).AddMilliseconds(-1);
                    sqlWhere.AppendFormat(" AND o2.ActualShipDate < @EndTime");
                    mySqlParameter.Add(new MySqlParameter("@EndTime", request.EndTime.Value));
                }
            }
            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
            .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundExceptionGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundExceptionGlobalDto>();
            response.TableResuls = new TableResults<OutboundExceptionGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundBoxGlobalDto> GetOutboundBoxReport(OutboundBoxGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append($@"SELECT COUNT(1) FROM outbound o
                              WHERE 1=1 @SqlWhere");


            var sql = new StringBuilder();
            sql.Append($@"select w.Name as WarehouseName,o.SysId as OutboundSysId,o.OutboundOrder,o.Status,o.CreateDate,o.ActualShipDate,
                        ifnull(ot.WholeCaseQty,0) as WholeCaseQty,ifnull(ot.ScatteredCaseQty,0) as ScatteredCaseQty
                        from outbound o 
                        left join
                        (select sum(case when TransferType = '{(int)OutboundTransferOrderType.Whole}' then 1 end) as WholeCaseQty, 
                         sum(case when TransferType = '{(int)OutboundTransferOrderType.Scattered}' then 1 end) as ScatteredCaseQty,
                        OutboundSysId from Outboundtransferorder
                        where status = '{(int)OutboundTransferOrderStatus.Finish}'
                        group by OutboundSysId) ot on ot.OutboundSysId = o.sysid
                        left join warehouse w on w.sysid = o.WareHouseSysId
                        WHERE 1=1 @SqlWhere
                        ORDER BY o.CreateDate DESC
                        LIMIT {request.iDisplayStart}, { request.iDisplayLength}");

            var sqlWhere = new StringBuilder();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat($" AND o.OutboundOrder = '{request.OutboundOrder.Trim()}'");
                }
                if (request.Status.HasValue)
                {
                    sqlWhere.AppendFormat($" AND o.Status = {request.Status.Value}");
                }
                if (request.CreateDateStart.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.CreateDateStart.Value}' <= o.CreateDate ");
                }
                if (request.CreateDateEnd.HasValue)
                {
                    sqlWhere.AppendFormat($" AND o.CreateDate <= '{request.CreateDateEnd.Value}' ");
                }

                if (request.ActualShipDateStart.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.ActualShipDateStart.Value}' <= o.ActualShipDate ");
                }

                if (request.ActualShipDateEnd.HasValue)
                {
                    sqlWhere.AppendFormat($" AND o.ActualShipDate <= '{request.ActualShipDateEnd.Value}' ");
                }
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.Append($" AND o.WareHouseSysId = '{request.SearchWarehouseSysId}'");
                }
            }
            sqlWhere.AppendFormat($" AND o.OutboundType != {(int)OutboundType.B2C} ");
            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString().Replace("@SqlWhere", sqlWhere.ToString())).AsQueryable().First();
            var queryList = base.Context.Database.SqlQuery<OutboundBoxGlobalDto>(sql.ToString().Replace("@SqlWhere", sqlWhere.ToString())).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundBoxGlobalDto>();
            response.TableResuls = new TableResults<OutboundBoxGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<OutboundBoxDto> GetOutboundBox(List<Guid> outboundSysIds, Guid wareHouseSysId)
        {
            var strSql = $@"SELECT DISTINCT od.OutboundSysId,od.SkuSysId,od.Qty,s.SkuName,pbp.SysId as BoxSysId,pbp.TransferOrder as BoxName,IFNULL(pbp.Qty,0) AS CaseQty, p.SysId,p.FieldValue02,p.FieldValue03 FROM outbounddetail od
                                LEFT join
                                (select pbpd.SkuSysId,pbp.OutboundSysId,pbpd.Qty,pbp.SysId,pbp.TransferOrder from outboundtransferorderdetail pbpd
                                 left join outboundtransferorder pbp on pbp.SysId = pbpd.OutboundTransferOrderSysId
                                 where pbp.OutboundSysId IN (@OutboundSysIds) AND pbp.Status != @Status AND pbp.TransferType = {(int)OutboundTransferOrderType.Scattered}) pbp 
                                  on od.OutboundSysId = pbp.OutboundSysId AND od.SkuSysId = pbp.SkuSysId
                                LEFT JOIN sku s ON s.SysId = od.SkuSysId
                                LEFT JOIN pack p on p.SysId = s.PackSysId
                                WHERE od.OutboundSysId IN (@OutboundSysIds)
                                Order By od.SkuSysId";
            var sysIds = StringHelper.GetConvertSqlIn(outboundSysIds);
            strSql = strSql.Replace("@OutboundSysIds", sysIds);
            var outboundBoxList = base.Context.Database.SqlQuery<OutboundBoxDto>(strSql, new MySqlParameter("@Status", -999)).ToList();
            return outboundBoxList;
        }

        /// <summary>
        /// B2C结算报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<BalanceGlobalDto> GetBalanceReport(BalanceGlobalQuery request)
        {
            string whWhere = request.SearchWarehouseSysId == Guid.Empty ? "" : $" and o.WareHouseSysId = '{request.SearchWarehouseSysId}'";

            var sqlCount = new StringBuilder();
            sqlCount.Append($@"  SELECT COUNT(1)
                                FROM vanningdetail v
                                JOIN vanning v1 ON v.VanningSysId = v1.SysId
                                JOIN outbound o ON v1.OutboundSysId = o.SysId
                                left JOIN carrier c ON v.CarrierSysId = c.SysId
                                JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                WHERE o.OutboundType = {(int)OutboundType.B2C} {whWhere} @SqlWhere;");


            var sql = new StringBuilder();
            sql.Append($@"   SELECT c.CarrierName,o.ActualShipDate AS OutboundDate,o.OutboundOrder,o.Remark,w.Name AS WarehouseName,
                            o.ConsigneeProvince,o.ConsigneeCity,o.ConsigneeArea,o.ConsigneeAddress,v.CarrierNumber,IFNULL(v.Weight,0) AS Weight
                            FROM vanningdetail v
                            JOIN vanning v1 ON v.VanningSysId = v1.SysId
                            JOIN outbound o ON v1.OutboundSysId = o.SysId
                            left JOIN carrier c ON v.CarrierSysId = c.SysId
                            JOIN warehouse w ON o.WareHouseSysId = w.SysId
                            WHERE o.OutboundType = {(int)OutboundType.B2C} {whWhere} @SqlWhere
                            ORDER BY o.OutboundDate DESC
                            LIMIT  @startIndex,@endIndex;");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.CarrierName))
                {
                    sqlWhere.AppendFormat(" AND c.CarrierName LIKE  CONCAT(@CarrierName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@CarrierName", request.CarrierName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND o.OutboundOrder =@OutboundOrder ");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
                }
                if (request.StartTime.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate > @StartTime");
                    mySqlParameter.Add(new MySqlParameter("@StartTime", request.StartTime.Value));
                }
                if (request.EndTime.HasValue)
                {
                    request.EndTime = request.EndTime.Value.AddDays(1).AddMilliseconds(-1);
                    sqlWhere.AppendFormat(" AND o.ActualShipDate < @EndTime");
                    mySqlParameter.Add(new MySqlParameter("@EndTime", request.EndTime.Value));
                }
                if (!string.IsNullOrEmpty(request.WarehouseName))
                {
                    sqlWhere.AppendFormat(" AND w.Name LIKE CONCAT(@WarehouseName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@WarehouseName", request.WarehouseName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ConsigneeProvince))
                {
                    sqlWhere.AppendFormat(" AND o.ConsigneeProvince LIKE CONCAT(@ConsigneeProvince, '%')");
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeProvince", request.ConsigneeProvince.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ConsigneeCity))
                {
                    sqlWhere.AppendFormat(" AND o.ConsigneeCity LIKE CONCAT(@ConsigneeCity, '%')", request.ConsigneeCity.Trim());
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeCity", request.ConsigneeCity.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ConsigneeArea))
                {
                    sqlWhere.AppendFormat(" AND o.ConsigneeArea LIKE CONCAT(@ConsigneeArea, '%')");
                    mySqlParameter.Add(new MySqlParameter("@ConsigneeArea", request.ConsigneeArea.Trim()));
                }
                if (!string.IsNullOrEmpty(request.CarrierNumber))
                {
                    sqlWhere.AppendFormat(" AND v.CarrierNumber =@CarrierNumber");
                    mySqlParameter.Add(new MySqlParameter("@CarrierNumber", request.CarrierNumber.Trim()));
                }
            }
            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
            .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<BalanceGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<BalanceGlobalDto>();
            response.TableResuls = new TableResults<BalanceGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 整散箱装箱明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundPackGlobalDto> GetOutboundPackReport(OutboundPackGlobalQuery request)
        {
            string whWhere = request.SearchWarehouseSysId == Guid.Empty ? "" : $" and o.WareHouseSysId = '{request.SearchWarehouseSysId}'";


            var sqlCount = new StringBuilder();
            sqlCount.Append($@"  
                                SELECT COUNT(1)
                                FROM outbound o
                                JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId 
                                WHERE o.Status = {(int)OutboundStatus.Delivery} 
                                    AND o1.Status = {(int)OutboundTransferOrderStatus.Finish}
                                   { whWhere }
                                    @SqlWhere;");

            var sql = new StringBuilder();
            sql.Append($@"   
                                SELECT 
                                  w.Name AS WarehouseName,
                                  o.Channel,
                                  o.OutboundOrder,
                                  o.ServiceStationName,
                                  1 AS packCount,
                                  o1.TransferType,
                                  IFNULL((SELECT SUM(o2.Qty) FROM outboundtransferorderdetail o2 WHERE o2.OutboundTransferOrderSysId = o1.SysId),0) AS OutboundQty,
                                  o.ActualShipDate ,
                                  o1.ReviewUserName AS CreateUserName,
                                  o1.TransferOrder 
                                FROM outbound o
                                JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId 
                                WHERE o.Status = {(int)OutboundStatus.Delivery} 
                                    AND o1.Status = {(int)OutboundTransferOrderStatus.Finish}
                                    { whWhere }
                                @SqlWhere
                                ORDER BY o.ActualShipDate DESC
                                LIMIT  @startIndex,@endIndex;");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND o.OutboundOrder = @OutboundOrder");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
                }
                if (request.ActualShipDateFrom.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate > @ActualShipDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom));
                }
                if (request.ActualShipDateTo.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate <= @ActualShipDateTo");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
            .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundPackGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundPackGlobalDto>();
            response.TableResuls = new TableResults<OutboundPackGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 整散箱商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundPackSkuGlobalDto> GetOutboundPackSkuReport(OutboundPackSkuGlobalQuery request)
        {

            string whWhere = request.SearchWarehouseSysId == Guid.Empty ? "" : $" and o.WareHouseSysId = '{request.SearchWarehouseSysId}'";


            var sqlCount = new StringBuilder();
            sqlCount.Append($@"  
                            SELECT COUNT(1)
                            FROM outbound o
                            JOIN warehouse w ON o.WareHouseSysId = w.SysId
                            JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId 
                            JOIN outboundtransferorderdetail O2 ON o1.SysId = O2.OutboundTransferOrderSysId
                            JOIN sku s ON O2.SkuSysId = s.SysId
                            WHERE o.Status = {(int)OutboundStatus.Delivery} 
                                AND o1.Status = {(int)OutboundTransferOrderStatus.Finish}
                                            {whWhere}
                                            @SqlWhere;");

            var sql = new StringBuilder();
            sql.Append($@"   
                            SELECT 
                                w.Name AS WarehouseName,
                                o.Channel,
                                o.OutboundOrder,
                                o.ServiceStationName,
                                o1.TransferType,
                                1 AS packCount,
                                o2.Qty,
                                s.SkuName,
                                s.UPC,
                                o.ActualShipDate ,
                                o1.ReviewUserName AS CreateUserName,
                                o1.TransferOrder 
                            FROM outbound o
                            JOIN warehouse w ON o.WareHouseSysId = w.SysId
                            JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId 
                            JOIN outboundtransferorderdetail O2 ON o1.SysId = O2.OutboundTransferOrderSysId
                            JOIN sku s ON O2.SkuSysId = s.SysId
                            WHERE o.Status = {(int)OutboundStatus.Delivery} 
                                AND o1.Status = {(int)OutboundTransferOrderStatus.Finish}
                                {whWhere}
                            @SqlWhere
                            ORDER BY o.ActualShipDate DESC,o.OutboundOrder,o1.TransferOrder ASC
                            LIMIT  @startIndex,@endIndex;");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND o.OutboundOrder = @OutboundOrder");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder));
                }
                if (!string.IsNullOrEmpty(request.TransferOrder))
                {
                    sqlWhere.AppendFormat(" AND o1.TransferOrder =@TransferOrder");
                    mySqlParameter.Add(new MySqlParameter("@TransferOrder", request.TransferOrder.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhere.AppendFormat(" AND s.UPC = @UPC");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhere.AppendFormat(" AND s.SkuName LIKE CONCAT(@SkuName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (request.ActualShipDateFrom.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate > @ActualShipDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom));
                }
                if (request.ActualShipDateTo.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate <= @ActualShipDateTo");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
            .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundPackSkuGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundPackSkuGlobalDto>();
            response.TableResuls = new TableResults<OutboundPackSkuGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<SkuPackGlobalListDto> GetSkuPackReport(SkuPackGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(1) FROM sku s
                                    LEFT JOIN pack p ON s.PackSysId = p.SysId
                                    WHERE 1 = 1  @SqlWhere");

            var sql = new StringBuilder();
            sql.AppendFormat(@"  SELECT s.SkuCode,s.SkuName,s.UPC,s.OtherId,
                                    u.UOMCode AS UOMCode03,p.FieldValue03,p.UPC03,
                                    u1.UOMCode AS UOMCode02,p.FieldValue02,p.UPC02,
                                    u2.UOMCode AS UOMCode04,p.FieldValue04,p.UPC04,
                                    u3.UOMCode AS UOMCode05,p.FieldValue05,p.UPC05
  
                                  FROM sku s
                                  LEFT JOIN pack p ON s.PackSysId=p.SysId
                                  LEFT JOIN uom u ON p.FieldUom03=u.SysId
                                  LEFT JOIN uom u1 ON p.FieldUom02=u1.SysId
                                  LEFT JOIN uom u2 ON p.FieldUom04=u2.SysId
                                  LEFT JOIN uom u3 ON p.FieldUom05=u3.SysId
                                  WHERE 1=1  @SqlWhere
                                  ORDER BY s.OtherId desc
                                  LIMIT  @startIndex,@endIndex; ");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhere.AppendFormat(" AND s.SkuName like  CONCAT(@SkuName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.SkuCode))
                {
                    sqlWhere.AppendFormat(" AND s.SkuCode=@SkuCode");
                    mySqlParameter.Add(new MySqlParameter("@SkuCode", request.SkuCode.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhere.AppendFormat(" AND s.UPC=@UPC");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (!string.IsNullOrEmpty(request.OtherId))
                {
                    sqlWhere.AppendFormat(" AND s.OtherId=@OtherId");
                    mySqlParameter.Add(new MySqlParameter("@OtherId", request.OtherId.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC03))
                {
                    sqlWhere.AppendFormat(" AND(p.UPC02='{0}' || p.UPC03=@UPC03 || p.UPC04=@UPC03 || p.UPC05=@UPC03 )");
                    mySqlParameter.Add(new MySqlParameter("@UPC03", request.UPC03.Trim()));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<SkuPackGlobalListDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<SkuPackGlobalListDto>();
            response.TableResuls = new TableResults<SkuPackGlobalListDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 出库单商品汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundSkuGlobalDto> GetOutboundSkuReport(OutboundSkuGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(o.SysId) FROM outbound o WHERE o.Status = 70 and o.OutboundType IN (30,80) @SqlWhere;");

            var sql = new StringBuilder();
            sql.AppendFormat(@"  SELECT a.*
                                  ,(SELECT COUNT(1) FROM outbounddetail od WHERE a.sysid = od.OutboundSysId) AS SkuType
                                  ,(SELECT SUM(od.ShippedQty) FROM outbounddetail od WHERE a.sysid = od.OutboundSysId) AS SkuCount
                                  ,(SELECT SUM(ifnull(od.ReturnQty,0)) FROM outbounddetail od WHERE a.sysid = od.OutboundSysId) AS ReturnQty
                                FROM (
                                select o.ServiceStationCode,o.ConsigneeCity,o.ConsigneeArea,
                                o.OutboundOrder,o.ActualShipDate,w.Name as WarehouseName,
                                o.ServiceStationName,o.SysId 
                                from outbound o 
                                join warehouse w on w.sysid = o.WareHouseSysId
                                WHERE o.Status = 70 and o.OutboundType IN(30,80)  @SqlWhere 
                                order by ActualShipDate desc
                                LIMIT  @startIndex,@endIndex) a;");

            var sqlWhere = new StringBuilder();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.AppendFormat(" AND o.WareHouseSysId = '{0}'", request.SearchWarehouseSysId);
                }
                if (!string.IsNullOrEmpty(request.ServiceStationCode))
                {
                    sqlWhere.AppendFormat(" AND o.ServiceStationCode='{0}'", request.ServiceStationCode.Trim());
                }
                if (!string.IsNullOrEmpty(request.ConsigneeCity))
                {
                    sqlWhere.AppendFormat(" AND o.ConsigneeCity like '{0}%'", request.ConsigneeCity.Trim());
                }
                if (!string.IsNullOrEmpty(request.ConsigneeArea))
                {
                    sqlWhere.AppendFormat(" AND o.ConsigneeArea like '{0}%'", request.ConsigneeArea.Trim());
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND o.OutboundOrder = '{0}'", request.OutboundOrder.Trim());
                }
                if (request.ActualShipDateFrom.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.ActualShipDateFrom.Value}' <= o.ActualShipDate ");
                }
                if (request.ActualShipDateTo.HasValue)
                {
                    sqlWhere.AppendFormat($" AND o.ActualShipDate <= '{request.ActualShipDateTo.Value}' ");
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString())).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundSkuGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString())).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<OutboundSkuGlobalDto>();
            response.TableResuls = new TableResults<OutboundSkuGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 出库捡货工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<PickingTimeSpanGlobalDto> GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"  SELECT COUNT(1)
                                FROM outbound o 
                                JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                JOIN pickdetail p ON o.SysId = p.OutboundSysId
                                WHERE o.Status IN (40,70) AND p.Status NOT IN (-1,-999) @SqlWhere");

            var sql = new StringBuilder();
            sql.AppendFormat(@"   SELECT w.SysId AS WarehouseSysId,w.Name AS WarehouseName,o.OutboundOrder,COUNT(DISTINCT p.SkuSysId) AS SkuTypeNumber,
                                  SUM(p.Qty) AS SkuTotalCount,p.UpdateUserName AS Operator,MIN(p.CreateDate) AS StartTime,MAX(p.UpdateDate) AS EndTime
                                  FROM outbound o 
                                  JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                  JOIN pickdetail p ON o.SysId = p.OutboundSysId
                                  WHERE o.Status IN (40,70) AND p.Status NOT IN (-1,-999) @SqlWhere
                                  GROUP BY o.SysId  
                                  LIMIT  @startIndex,@endIndex; ");

            var sqlWhere = new StringBuilder();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.AppendFormat(" AND o.WareHouseSysId = '{0}'", request.SearchWarehouseSysId);
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND o.OutboundOrder='{0}'", request.OutboundOrder.Trim());
                }
                if (!string.IsNullOrEmpty(request.Operator))
                {
                    sqlWhere.AppendFormat(" AND p.UpdateUserName like '%{0}%'", request.Operator.Trim());
                }
                if (request.StartTimeFrom.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.StartTimeFrom.Value}' <= p.CreateDate ");
                }
                if (request.StartTimeTo.HasValue)
                {
                    sqlWhere.AppendFormat($" AND p.CreateDate <= '{request.StartTimeTo.Value}' ");
                }
                if (request.EndTimeFrom.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.EndTimeFrom.Value}' <= p.UpdateDate ");
                }
                if (request.EndTimeTo.HasValue)
                {
                    sqlWhere.AppendFormat($" AND p.UpdateDate <= '{request.EndTimeTo.Value}' ");
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString())).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<PickingTimeSpanGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString())).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<PickingTimeSpanGlobalDto>();
            response.TableResuls = new TableResults<PickingTimeSpanGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }


        /// <summary>
        /// 出库复核工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundTransferOrderGlobalDto> GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"SELECT COUNT(1) FROM 
                            (
                                SELECT w.SysId AS WarehouseSysId,w.Name AS WarehouseName,o.OutboundOrder,COUNT(DISTINCT o2.SkuSysId) AS SkuTypeNumber,
                                SUM(o2.Qty) AS SkuTotalCount,o1.CreateUserName AS Operator,MIN(o2.CreateDate) AS StartTime,MAX(o2.CreateDate) AS EndTime
                                FROM outbound o
                                JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId
                                JOIN outboundtransferorderdetail o2 ON o1.SysId = o2.OutboundTransferOrderSysId
                                WHERE o.Status IN (40,70) AND o1.Status <> -999
                                GROUP BY o.OutboundOrder,w.Name
                            )aa WHERE 1 = 1  @SqlWhere");

            var sql = new StringBuilder();
            sql.AppendFormat(@"  SELECT * FROM 
                                (
                                    SELECT w.SysId AS WarehouseSysId,w.Name AS WarehouseName,o.OutboundOrder,COUNT(DISTINCT o2.SkuSysId) AS SkuTypeNumber,
                                    SUM(o2.Qty) AS SkuTotalCount,o1.CreateUserName AS Operator,MIN(o2.CreateDate) AS StartTime,MAX(o2.CreateDate) AS EndTime
                                    FROM outbound o
                                    JOIN warehouse w ON o.WareHouseSysId = w.SysId
                                    JOIN outboundtransferorder o1 ON o.SysId = o1.OutboundSysId
                                    JOIN outboundtransferorderdetail o2 ON o1.SysId = o2.OutboundTransferOrderSysId
                                    WHERE o.Status IN (40,70) AND o1.Status <> -999
                                    GROUP BY o.OutboundOrder,w.Name
                                )aa WHERE 1 = 1 @SqlWhere
                                ORDER BY StartTime DESC
                                LIMIT  @startIndex,@endIndex; ");

            var sqlWhere = new StringBuilder();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.AppendFormat(" AND WarehouseSysId = '{0}'", request.SearchWarehouseSysId);
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat(" AND OutboundOrder='{0}'", request.OutboundOrder.Trim());
                }
                if (!string.IsNullOrEmpty(request.Operator))
                {
                    sqlWhere.AppendFormat(" AND Operator='{0}'", request.Operator.Trim());
                }
                if (request.StartTimeFrom.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.StartTimeFrom.Value}' <= StartTime ");
                }
                if (request.StartTimeTo.HasValue)
                {
                    sqlWhere.AppendFormat($" AND StartTime <= '{request.StartTimeTo.Value}' ");
                }
                if (request.EndTimeFrom.HasValue)
                {
                    sqlWhere.AppendFormat($" AND '{request.EndTimeFrom.Value}' <= EndTime ");
                }
                if (request.EndTimeTo.HasValue)
                {
                    sqlWhere.AppendFormat($" AND EndTime <= '{request.EndTimeTo.Value}' ");
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString())).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<OutboundTransferOrderGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString())).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<OutboundTransferOrderGlobalDto>();
            response.TableResuls = new TableResults<OutboundTransferOrderGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        public List<AccessBizMappingDto> GetAccessBizMappingList()
        {
            string query = @"
                SELECT 
                  SysId,
                  app_controller,
                  app_service,
                  BizName,
                  SecondBizName,
                  FirstBizName 
                FROM AccessBizMapping;
                ";
            var bizList = base.Context.Database.SqlQuery<AccessBizMappingDto>(query).ToList();

            //List<AccessBizGlobalDto> globalList = queryList.GroupBy(p => new { p.FirstBizName})
            //                                    .Select(p => new AccessBizGlobalDto {
            //                                        FirstBizName = p.Key.FirstBizName
            //                                    }).ToList();

            string sql = $@"
                SELECT app_controller,app_service,COUNT(1) AS 'Count'
                FROM access_log
                GROUP BY app_controller,app_service";

            var dataList = base.Context.Database.SqlQuery<AccessBizMappingDto>(sql).ToList();

            foreach (var item in bizList)
            {
                var data = dataList.FirstOrDefault(p => p.app_controller.Equals(item.app_controller) && p.app_service.Equals(item.app_service));

                if (data != null)
                {
                    item.Count = data.Count;
                }

                //List<AccessBizMappingDto> bizList = queryList.Where(p => p.FirstBizName.Equals(item.FirstBizName, StringComparison.OrdinalIgnoreCase)).ToList();

                //List<MySqlParameter> parameters = new List<MySqlParameter>();

                //string sql = $@"
                //    SELECT COUNT(1)
                //    FROM access_log al
                //    WHERE al.app_controller = @app_controller
                //      AND al.app_service = @app_service;";

                //parameters.Add(new MySqlParameter($"@app_controller", item.app_controller));
                //parameters.Add(new MySqlParameter($"@app_service", item.app_service));

                //var count = base.Context.Database.SqlQuery<int>(sql, parameters.ToArray()).AsQueryable().First();
                //item.Count = count;
            }

            return bizList;
        }

        public Pages<ReturnOrderGlobalDto> GetReturnOrderGlobalReport(ReturnOrderGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append($@"SELECT COUNT(1) 
                                FROM purchase p
                                LEFT JOIN outbound o ON p.OutboundSysId = o.SysId
                                JOIN warehouse w ON p.WarehouseSysId = w.SysId
                                LEFT JOIN warehouse w1 ON o.WareHouseSysId = w1.SysId
                                WHERE p.Type = {(int)PurchaseType.Return} 
                                @SqlWhere ;");

            var sql = new StringBuilder();
            sql.AppendFormat($@"SELECT 
                                  p.PurchaseOrder,
                                  p.CreateDate,
                                  p.CreateUserName,
                                  p.OutboundOrder,
                                  (SELECT COUNT(1) FROM purchasedetail p1 WHERE p.SysId = p1.PurchaseSysId) AS skuGroupCount,
                                  (SELECT SUM(p1.Qty) FROM purchasedetail p1 WHERE p.SysId = p1.PurchaseSysId) AS skuTotalCount,
                                  o.OutboundType,
                                  w.Name AS PurchaseWarehouse,
                                  w1.Name AS OutboundWarehouse
                                FROM purchase p
                                LEFT JOIN outbound o ON p.OutboundSysId = o.SysId
                                JOIN warehouse w ON p.WarehouseSysId = w.SysId
                                LEFT JOIN warehouse w1 ON o.WareHouseSysId = w1.SysId
                                WHERE p.Type = {(int)PurchaseType.Return} 
                                 @SqlWhere   
                                ORDER BY p.CreateDate DESC
                                LIMIT  @startIndex,@endIndex; ");

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            var sqlWhere = new StringBuilder();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.AppendFormat($" AND p.WarehouseSysId = @WarehouseSysId");
                    parameters.Add(new MySqlParameter("@WarehouseSysId", request.SearchWarehouseSysId));
                }

                if (request.OutboundType.HasValue)
                {
                    sqlWhere.AppendFormat($" AND o.OutboundType = @OutboundType");
                    parameters.Add(new MySqlParameter("@OutboundType", request.OutboundType.Value));
                }

                if (!string.IsNullOrEmpty(request.PurchaseOrder))
                {
                    sqlWhere.AppendFormat($" AND p.PurchaseOrder = @PurchaseOrder");
                    parameters.Add(new MySqlParameter("@PurchaseOrder", request.PurchaseOrder));
                }

                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.AppendFormat($" AND p.OutboundOrder = @OutboundOrder");
                    parameters.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString()), parameters.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<ReturnOrderGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), parameters.ToArray());

            var pageList = queryList.ToList();
            var response = new Pages<ReturnOrderGlobalDto>();
            response.TableResuls = new TableResults<ReturnOrderGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        public List<FertilizerRORadarGlobalDto> GetFertilizerRORadarList(FertilizerRORadarGlobalQuery request)
        {
            var sql = @"SELECT A.SkuName, SUM(A.RQty) AS RQty, SUM(A.OQty) AS OQty FROM
                        (
                          SELECT s.SkuName, SUM(r1.ReceivedQty) AS RQty, 0 AS OQty FROM receipt r 
                          LEFT JOIN receiptdetail r1 ON r.SysId = r1.ReceiptSysId
                          LEFT JOIN sku s ON r1.SkuSysId = s.SysId
                          WHERE r1.SkuSysId IN ({0}) AND r.Status = 40
                          GROUP BY r1.SkuSysId
                            UNION ALL
                          SELECT s.SkuName, 0 AS RQty, SUM(o1.ShippedQty) AS OQty FROM outbound o
                          LEFT JOIN outbounddetail o1 ON o.SysId = o1.OutboundSysId
                          LEFT JOIN sku s ON o1.SkuSysId = s.SysId
                          WHERE o1.SkuSysId IN ({0}) AND o.Status = 70
                          GROUP BY o1.SkuSysId  
                        ) A GROUP BY A.SkuName ORDER BY A.SkuName";
            sql = string.Format(sql, request.SkuSysIds.GuidListToIds());
            return base.Context.Database.SqlQuery<FertilizerRORadarGlobalDto>(sql).ToList();
        }

        public List<FertilizerInvRadarGlobalDto> GetFertilizerInvRadarList(FertilizerInvRadarGlobalQuery request)
        {
            var sql = @"SELECT s.SkuName, SUM(i.Qty) AS Qty FROM invlot i
                        LEFT JOIN sku s ON i.SkuSysId = s.SysId
                        WHERE i.SkuSysId IN
                        ({0}) GROUP BY i.SkuSysId ORDER BY s.SkuName;";
            sql = string.Format(sql, request.SkuSysIds.GuidListToIds());
            return base.Context.Database.SqlQuery<FertilizerInvRadarGlobalDto>(sql).ToList();
        }

        public List<FertilizerInvPieGlobalDto> GetFertilizerInvPieList(FertilizerInvPieGlobalQuery request)
        {
            var sql = @"SELECT w.Name AS WarehouseName, SUM(i.Qty) AS Qty FROM invlot i
                        LEFT JOIN warehouse w ON i.WareHouseSysId = w.SysId
                        WHERE i.SkuSysId = @SkuSysId
                        GROUP BY i.WareHouseSysId HAVING SUM(i.Qty) != 0 ORDER BY w.CreateDate;";
            return base.Context.Database.SqlQuery<FertilizerInvPieGlobalDto>(sql, new MySqlParameter("@SkuSysId", request.SkuSysId)).ToList();
        }

        /// <summary>
        /// 出库汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundListGlobalDto> GetOutboundSummaryReport(OutboundGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append($@"SELECT COUNT(1) 
                               FROM outbound o
                               WHERE 1=1 @SqlWhere");


            var sql = new StringBuilder();
            sql.Append($@"  SELECT o.SysId,o.OutboundOrder,o.Status,o.OutboundType,o.OutboundChildType,
                            o.CreateDate,o.ExternOrderId,o.ExternOrderDate,o.AuditingDate,o.ActualShipDate,
                            o.ConsigneeName,o.ConsigneeArea,o.ConsigneeProvince,o.ConsigneeCity,o.ConsigneeAddress,
                            o.TotalQty,o.Remark,o.ServiceStationName,o.ServiceStationCode,o.ConsigneePhone,
                            o.ConsigneeTown,o.ConsigneeVillage,o.PlatformOrder,o.PurchaseOrder,o.IsReturn,o.SortNumber,
                            o.TMSOrder,o.DepartureDate,o.AppointUserNames,o.Exception,o.IsInvoice,o.Channel,o.WareHouseSysId AS WarehouseSysId,
                            (SELECT  p.CreateDate FROM pickdetail p WHERE p.OutboundSysId = o.SysId LIMIT 0,1) AS PickdetailCreateDate,
                            (SELECT w1.Name FROM warehouse w1 WHERE w1.SysId = o.WareHouseSysId LIMIT 0,1) AS WarehouseName 
                             ,(
                                 SELECT IFNULL(SUM(
                                     CASE when p1.InLabelUnit01 IS NOT NULL AND p1.InLabelUnit01 = 1
                                     AND p1.FieldValue01 > 0 AND p1.FieldValue02 > 0
                                     THEN ROUND(p1.FieldValue02 * (IFNULL(o1.Qty,0)/p1.FieldValue01),3)
                                     ELSE IFNULL(o1.Qty,0) END),0)  
                                     FROM outbounddetail o1 
                                     JOIN sku s ON o1.SkuSysId = s.SysId
                                     JOIN pack p1 ON s.PackSysId = p1.SysId 
                                     WHERE o1.OutboundSysId = o.SysId  @SqlDetailWhere
   
                              ) AS DisplayTotalQty
                               ,(
                                 SELECT   
                                     IFNULL(SUM(
                                     CASE when p1.InLabelUnit01 IS NOT NULL AND p1.InLabelUnit01 = 1
                                     AND p1.FieldValue01 > 0 AND p1.FieldValue02 > 0
                                     THEN ROUND(p1.FieldValue02 * (IFNULL(o1.ShippedQty,0)/p1.FieldValue01),3)
                                     ELSE IFNULL(o1.ShippedQty,0) END),0)  
                                     FROM outbounddetail o1 
                                     JOIN sku s ON o1.SkuSysId = s.SysId
                                     JOIN pack p1 ON s.PackSysId = p1.SysId 
                                     WHERE o1.OutboundSysId = o.SysId  @SqlDetailWhere
    
                              ) AS DisplayTotalShippedQty
                            FROM outbound o  
                            WHERE 1=1 @SqlWhere
                            ORDER BY o.CreateDate DESC 
                        LIMIT {request.iDisplayStart}, { request.iDisplayLength}");

            var sqlWhere = new StringBuilder();
            var sqlWhereDetail = new StringBuilder();

            #region 查询条件

            var mySqlParameter = new List<MySqlParameter>();

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                sqlWhere.Append(" AND o.WarehouseSysId = @WarehouseSysId");
                mySqlParameter.Add(new MySqlParameter("@WarehouseSysId", request.SearchWarehouseSysId));
            }

            if (!string.IsNullOrEmpty(request.ExternOrderId))
            {
                sqlWhere.Append(" AND o.ExternOrderId LIKE CONCAT(@ExternOrderId,'%')");
                mySqlParameter.Add(new MySqlParameter("@ExternOrderId", request.ExternOrderId.Trim()));
            }
            if (!string.IsNullOrEmpty(request.OutboundOrder))
            {
                sqlWhere.Append(" AND o.OutboundOrder = @OutboundOrder");
                mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
            }
            if (!string.IsNullOrEmpty(request.ConsigneeName))
            {
                sqlWhere.Append(" AND o.ConsigneeName LIKE CONCAT(@ConsigneeName,'%')");
                mySqlParameter.Add(new MySqlParameter("@ConsigneeName", request.ConsigneeName.Trim()));
            }
            if (!string.IsNullOrEmpty(request.ConsigneePhone))
            {
                sqlWhere.Append(" AND o.ConsigneePhone LIKE CONCAT(@ConsigneePhone,'%')");
                mySqlParameter.Add(new MySqlParameter("@ConsigneePhone", request.ConsigneePhone.Trim()));
            }
            if (!string.IsNullOrEmpty(request.ConsigneeAddress))
            {
                sqlWhere.Append(" AND CONCAT(IFNULL(o.ConsigneeProvince,''),IFNULL(o.ConsigneeCity,''),IFNULL(o.ConsigneeArea,''),IFNULL(o.ConsigneeAddress,'')) LIKE CONCAT(@ConsigneeAddress,'%')");
                mySqlParameter.Add(new MySqlParameter("@ConsigneeAddress", request.ConsigneeAddress.Trim()));
            }
            if (request.CreateDateFrom.HasValue)
            {
                sqlWhere.Append(" AND o.CreateDate > @CreateDateFrom");
                mySqlParameter.Add(new MySqlParameter("@CreateDateFrom", request.CreateDateFrom.Value));
            }
            if (request.CreateDateTo.HasValue)
            {
                sqlWhere.Append(" AND o.CreateDate < @CreateDateTo");
                mySqlParameter.Add(new MySqlParameter("@CreateDateTo", request.CreateDateTo.Value));
            }
            if (request.ActualShipDateFrom.HasValue)
            {
                sqlWhere.Append(" AND o.ActualShipDate > @ActualShipDateFrom");
                mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom.Value));
            }
            if (request.ActualShipDateTo.HasValue)
            {
                sqlWhere.Append(" AND o.ActualShipDate < @ActualShipDateTo");
                mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo.Value));
            }
            if (request.Status.HasValue)
            {
                sqlWhere.Append(" AND o.Status = @Status");
                mySqlParameter.Add(new MySqlParameter("@Status", request.Status.Value));
            }
            if (request.AuditingDateFrom.HasValue)
            {
                sqlWhere.Append(" AND o.AuditingDate > @AuditingDateFrom");
                mySqlParameter.Add(new MySqlParameter("@AuditingDateFrom", request.AuditingDateFrom.Value));
            }
            if (request.AuditingDateTo.HasValue)
            {
                sqlWhere.Append(" AND o.AuditingDate < @AuditingDateTo");
                mySqlParameter.Add(new MySqlParameter("@AuditingDateTo", request.AuditingDateTo.Value));
            }
            if (request.OutboundType.HasValue)
            {
                sqlWhere.Append(" AND o.OutboundType = @OutboundType");
                mySqlParameter.Add(new MySqlParameter("@OutboundType", request.OutboundType.Value));
            }
            if (request.IsReturn.HasValue)
            {
                if (request.IsReturn == true)
                {
                    sqlWhere.Append(" AND o.IsReturn > @IsReturn");
                    mySqlParameter.Add(new MySqlParameter("@IsReturn", 0));
                }
                else
                {
                    sqlWhere.Append(" AND o.IsReturn = @IsReturn");
                    mySqlParameter.Add(new MySqlParameter("@IsReturn", DBNull.Value));
                }
            }
            if (!string.IsNullOrEmpty(request.SkuName) || !string.IsNullOrEmpty(request.UPC) || !string.IsNullOrEmpty(request.SkuCode) || request.IsMaterial.HasValue)
            {
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhereDetail.Append(" AND s.SkuName LIKE CONCAT(@SkuName,'%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhereDetail.Append(" AND s.UPC LIKE CONCAT(@UPC,'%')");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (!string.IsNullOrEmpty(request.SkuCode))
                {
                    sqlWhereDetail.Append(" AND s.SkuCode LIKE CONCAT(@SkuCode,'%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuCode", request.SkuCode.Trim()));
                }

                if (request.IsMaterial.HasValue)
                {
                    if (request.IsMaterial.Value)
                    {
                        sqlWhereDetail.Append(" AND s.IsMaterial = '@IsMaterial%'");
                        mySqlParameter.Add(new MySqlParameter("@IsMaterial", request.IsMaterial.Value));
                    }
                    else
                    {
                        sqlWhereDetail.Append(" AND s.IsMaterial <> '@IsMaterial%'");
                        mySqlParameter.Add(new MySqlParameter("@IsMaterial", !request.IsMaterial.Value));
                    }
                }
            }

            if (!string.IsNullOrEmpty(request.ToWareHouseSysId))
            {
                var toWareHouseSysId = Guid.Parse(request.ToWareHouseSysId);
                sqlWhere.Append(" AND o.SysId IN (SELECT TransferOutboundSysId FROM transferinventory WHERE ToWareHouseSysId = @ToWareHouseSysId)");
                mySqlParameter.Add(new MySqlParameter("@ToWareHouseSysId", toWareHouseSysId));
            }
            if (!string.IsNullOrEmpty(request.ServiceStationCode))
            {
                sqlWhere.Append(" AND o.ServiceStationCode LIKE CONCAT(@ServiceStationCode,'%')");
                mySqlParameter.Add(new MySqlParameter("@ServiceStationCode", request.ServiceStationCode.Trim()));
            }
            if (!string.IsNullOrEmpty(request.ServiceStationName))
            {
                sqlWhere.Append(" AND o.ServiceStationName LIKE CONCAT(@ServiceStationName,'%')");
                mySqlParameter.Add(new MySqlParameter("@ServiceStationName", request.ServiceStationName.Trim()));
            }
            if (!string.IsNullOrEmpty(request.OutboundChildType))
            {
                sqlWhere.Append(" AND o.OutboundChildType LIKE CONCAT(@OutboundChildType,'%')");
                mySqlParameter.Add(new MySqlParameter("@OutboundChildType", request.OutboundChildType.Trim()));
            }
            if (!string.IsNullOrEmpty(request.PurchaseOrder))
            {
                sqlWhere.Append(" AND o.PurchaseOrder = @PurchaseOrder");
                mySqlParameter.Add(new MySqlParameter("@PurchaseOrder", request.PurchaseOrder.Trim()));
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                var regionList = request.Region.Split(',');
                for (int i = 0; i < regionList.Length; i++)
                {
                    var region = regionList[i];
                    if (i == 0)
                    {
                        sqlWhere.Append(" AND o.ConsigneeProvince LIKE CONCAT(@Region0,'%')");
                        mySqlParameter.Add(new MySqlParameter("@Region0", region));
                    }

                    if (i == 1)
                    {
                        sqlWhere.Append(" AND o.ConsigneeCity LIKE CONCAT(@Region1,'%')");
                        mySqlParameter.Add(new MySqlParameter("@Region1", region));
                    }
                    if (i == 2)
                    {
                        sqlWhere.Append(" AND o.ConsigneeArea LIKE CONCAT(@Region2,'%')");
                        mySqlParameter.Add(new MySqlParameter("@Region2", region));
                    }
                    if (i == 3)
                    {
                        sqlWhere.Append(" AND o.ConsigneeTown LIKE CONCAT(@Region3,'%')");
                        mySqlParameter.Add(new MySqlParameter("@Region3", region));
                    }
                    if (i == 4)
                    {
                        sqlWhere.Append(" AND o.ConsigneeVillage LIKE CONCAT(@Region4,'%')");
                        mySqlParameter.Add(new MySqlParameter("@Region4", region));
                    }
                }
            }

            if (!string.IsNullOrEmpty(request.PlatformOrder))
            {
                sqlWhere.Append(" AND o.PlatformOrder = @PlatformOrder");
                mySqlParameter.Add(new MySqlParameter("@PlatformOrder", request.PlatformOrder.Trim()));
            }
            if (!string.IsNullOrEmpty(request.TMSOrder))
            {
                sqlWhere.Append(" AND o.TMSOrder = @TMSOrder");
                mySqlParameter.Add(new MySqlParameter("@TMSOrder", request.TMSOrder.Trim()));
            }

            if (request.DepartureDateFrom.HasValue)
            {
                sqlWhere.Append(" AND o.DepartureDate > @DepartureDateFrom");
                mySqlParameter.Add(new MySqlParameter("@DepartureDateFrom", request.DepartureDateFrom.Value));
            }
            if (request.DepartureDateTo.HasValue)
            {
                sqlWhere.Append(" AND o.DepartureDate < @DepartureDateTo");
                mySqlParameter.Add(new MySqlParameter("@DepartureDateTo", request.DepartureDateTo.Value));
            }

            if (!string.IsNullOrEmpty(request.Channel))
            {
                sqlWhere.Append(" AND o.Channel LIKE CONCAT(@Channel,'%')");
                mySqlParameter.Add(new MySqlParameter("@Channel", request.Channel.Trim()));
            }

            #endregion 查询条件 

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString().Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            if (request.IsExport == true && count > PublicConst.EachExportDataMaxCount)
            {
                throw new Exception("导出数据条数大于10W,请联系管理员!");
            }

            var queryList = base.Context.Database.SqlQuery<OutboundListGlobalDto>(sql.ToString().Replace("@SqlWhere", sqlWhere.ToString()).Replace("@SqlDetailWhere", sqlWhereDetail.ToString()), mySqlParameter.ToArray()).AsQueryable();
            var pageList = queryList.ToList();
            var response = new Pages<OutboundListGlobalDto>();
            response.TableResuls = new TableResults<OutboundListGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 农资出库商品报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FertilizerOutboundSkuGlobalDto> GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@"  SELECT COUNT(1)
                                FROM outbound o 
                                LEFT JOIN outbounddetail od ON od.OutboundSysId = o.SysId
                                LEFT JOIN sku s ON s.SysId = od.SkuSysId
                                LEFT JOIN warehouse w ON w.SysId = o.WareHouseSysId
                                WHERE o.Status = 70 AND o.OutboundType IN (80) @SqlWhere");

            var sql = new StringBuilder();
            sql.AppendFormat(@"   SELECT
                                  o.ServiceStationCode,
                                  o.ServiceStationName,
                                  o.OutboundOrder,
                                  o.ActualShipDate,
                                  s.SkuName,
                                  s.UPC,
                                  od.ShippedQty,
                                  ifnull(od.ReturnQty,0) as ReturnQty,
                                  w.Name AS WarehouseName
                                  FROM outbound o 
                                  LEFT JOIN outbounddetail od ON od.OutboundSysId = o.SysId
                                  LEFT JOIN sku s ON s.SysId = od.SkuSysId
                                  LEFT JOIN warehouse w ON w.SysId = o.WareHouseSysId
                                  WHERE o.Status = 70 AND OutboundType IN (80) @SqlWhere
                                  ORDER BY o.OutboundOrder DESC
                                  LIMIT  @startIndex,@endIndex; ");
            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != Guid.Empty)
                {
                    sqlWhere.Append(" AND o.WareHouseSysId = @WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }
                if (!string.IsNullOrEmpty(request.ServiceStationCode))
                {
                    sqlWhere.Append(" AND o.ServiceStationCode = @ServiceStationCode");
                    mySqlParameter.Add(new MySqlParameter("@ServiceStationCode", request.ServiceStationCode.Trim()));
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    sqlWhere.Append(" AND o.OutboundOrder = @OutboundOrder");
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", request.OutboundOrder.Trim()));
                }
                if (!string.IsNullOrEmpty(request.ServiceStationName))
                {
                    sqlWhere.Append(" AND o.ServiceStationName = @ServiceStationName");
                    mySqlParameter.Add(new MySqlParameter("@ServiceStationName", request.ServiceStationName.Trim()));
                }
                if (request.ActualShipDateFrom.HasValue)
                {
                    sqlWhere.AppendFormat(" AND o.ActualShipDate >= @ActualShipDateFrom");
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateFrom", request.ActualShipDateFrom));
                }
                if (request.ActualShipDateTo.HasValue)
                {
                    request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1).AddMilliseconds(-1);
                    sqlWhere.AppendFormat(" AND o.ActualShipDate <= @ActualShipDateTo", request.ActualShipDateTo);
                    mySqlParameter.Add(new MySqlParameter("@ActualShipDateTo", request.ActualShipDateTo));
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhere.AppendFormat(" AND s.SkuName LIKE CONCAT(@SkuName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhere.Append(" AND s.UPC = @UPC");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
           .Replace("@SqlWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<FertilizerOutboundSkuGlobalDto>(sql.ToString()
              .Replace("@SqlWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<FertilizerOutboundSkuGlobalDto>();
            response.TableResuls = new TableResults<FertilizerOutboundSkuGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }

        /// <summary>
        /// 分页获取渠道库存数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ChannelInventoryGlobalDto> GetChannelInventoryByPage(ChannelInventoryGlobalQuery request)
        {
            var sqlCount = new StringBuilder();
            sqlCount.Append(@" SELECT COUNT(*) FROM ( SELECT COUNT(1) FROM invlot i
                                LEFT JOIN sku s ON i.SkuSysId=s.SysId
                                WHERE 1=1 @SQLWhere
                                GROUP BY i.SkuSysId,i.LotAttr01) A");

            var sql = new StringBuilder(); sql.Append(@" SELECT w.Name AS WareHouseName,s.SkuName,s.UPC,s.OtherId,i.LotAttr01 AS Channel,
                             IFNULL(CASE WHEN ((((p.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p.InLabelUnit01) AND
                                  (p.InLabelUnit01 IS NOT NULL))) AND
                                  (p.FieldValue01 > 0)) AND
                                  (p.FieldValue01 > 0)) THEN (ROUND(((p.FieldValue02 *  SUM(i.Qty)) * 1.00) /             (p.FieldValue01), 3)) 
                              ELSE SUM(i.Qty)  END, 0) AS Qty,
                              IFNULL(CASE WHEN ((((p.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p.InLabelUnit01) AND
                                  (p.InLabelUnit01 IS NOT NULL))) AND
                                  (p.FieldValue01 > 0)) AND
                                  (p.FieldValue01 > 0)) THEN (ROUND(((p.FieldValue02 *  SUM(i.FrozenQty)) * 1.00) /        (p.FieldValue01), 3)) 
                              ELSE SUM(i.FrozenQty)  END, 0) AS FrozenQty,
                              IFNULL(CASE WHEN ((((p.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p.InLabelUnit01) AND
                                  (p.InLabelUnit01 IS NOT NULL))) AND
                                  (p.FieldValue01 > 0)) AND
                                  (p.FieldValue01 > 0)) THEN (ROUND(((p.FieldValue02 *  SUM(i.AllocatedQty)) * 1.00) /    (p.FieldValue01), 3)) 
                              ELSE SUM(i.AllocatedQty)  END, 0) AS AllocatedQty,
                              IFNULL(CASE WHEN ((((p.InLabelUnit01 IS NOT NULL) AND
                                  ((1 = p.InLabelUnit01) AND
                                  (p.InLabelUnit01 IS NOT NULL))) AND
                                  (p.FieldValue01 > 0)) AND
                                  (p.FieldValue01 > 0)) THEN (ROUND(((p.FieldValue02 *  SUM(i.PickedQty)) * 1.00) /         (p.FieldValue01), 3)) 
                                ELSE SUM(i.PickedQty)  END, 0) AS PickedQty
                            FROM invlot i
                            LEFT JOIN sku s ON i.SkuSysId=s.SysId
                            LEFT JOIN pack p ON s.PackSysId = p.SysId
                            LEFT JOIN warehouse w ON i.WareHouseSysId=w.SysId
                            WHERE 1=1 @SQLWhere
                            GROUP BY i.SkuSysId,i.LotAttr01,i.WareHouseSysId
                            LIMIT  @startIndex,@endIndex;");

            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            if (request != null)
            {
                if (request.SearchWarehouseSysId != new Guid())
                {
                    sqlWhere.AppendFormat(" AND i.WareHouseSysId =@WareHouseSysId");
                    mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", request.SearchWarehouseSysId));
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    sqlWhere.AppendFormat(" AND s.SkuName like  CONCAT(@SkuName, '%')");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", request.SkuName.Trim()));
                }
                if (!string.IsNullOrEmpty(request.Channel))
                {
                    sqlWhere.AppendFormat(" AND i.LotAttr01 LIKE  CONCAT(@Channel, '%')");
                    mySqlParameter.Add(new MySqlParameter("@Channel", request.Channel.Trim()));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    sqlWhere.AppendFormat(" AND s.UPC=@UPC");
                    mySqlParameter.Add(new MySqlParameter("@UPC", request.UPC.Trim()));
                }
                if (!string.IsNullOrEmpty(request.OtherId))
                {
                    sqlWhere.AppendFormat(" AND s.OtherId=@OtherId");
                    mySqlParameter.Add(new MySqlParameter("@OtherId", request.OtherId.Trim()));
                }
                if (request.IsStoreZero.HasValue && request.IsStoreZero.Value)
                {
                    sqlWhere.AppendFormat(" AND i.Qty>0");
                }
            }

            var count = base.Context.Database.SqlQuery<int>(sqlCount.ToString()
                             .Replace("@SQLWhere", sqlWhere.ToString()), mySqlParameter.ToArray()).AsQueryable().First();

            var queryList = base.Context.Database.SqlQuery<ChannelInventoryGlobalDto>(sql.ToString()
              .Replace("@SQLWhere", sqlWhere.ToString())
              .Replace("@startIndex", request.iDisplayStart.ToString())
              .Replace("@endIndex", request.iDisplayLength.ToString()), mySqlParameter.ToArray()).AsQueryable();

            var pageList = queryList.ToList();
            var response = new Pages<ChannelInventoryGlobalDto>();
            response.TableResuls = new TableResults<ChannelInventoryGlobalDto>()
            {
                aaData = pageList,
                iTotalDisplayRecords = count,
                iTotalRecords = pageList.Count(),
                sEcho = request.sEcho
            };
            return response;
        }
    }
}
