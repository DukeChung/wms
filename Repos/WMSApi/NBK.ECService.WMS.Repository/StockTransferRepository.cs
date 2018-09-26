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
    public class StockTransferRepository : CrudRepository, IStockTransferRepository
    {
        public StockTransferRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            var query = from invlot in Context.invlots
                        join invlotloclpn in Context.invlotloclpns on invlot.SkuSysId equals invlotloclpn.SkuSysId
                        join sku in Context.skus on invlot.SkuSysId equals sku.SysId
                        join warehouse in Context.warehouses on invlot.WareHouseSysId equals warehouse.SysId
                        join pk in Context.packs on sku.PackSysId equals pk.SysId
                        where invlot.WareHouseSysId == invlotloclpn.WareHouseSysId
                            && invlot.Lot == invlotloclpn.Lot
                            && invlot.WareHouseSysId == request.WarehouseSysId
                        select new
                        {
                            invlot,
                            invlotloclpnSysId = invlotloclpn.SysId,
                            invlotloclpnQty = invlotloclpn.Qty,
                            invlotloclpn.Loc,
                            sku.SkuName,
                            sku.SkuCode,
                            sku.SkuDescr,
                            sku.UPC,
                            WarehouseName = warehouse.Name,
                            pk.InLabelUnit01,
                            pk.FieldValue01,
                            pk.FieldValue02
                        };
            #region 拼条件
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

            if (!request.Lot.IsNull())
            {
                request.Lot = request.Lot.Trim();
                query = query.Where(p => p.invlot.Lot.Equals(request.Lot, StringComparison.OrdinalIgnoreCase));
            }

            if (!request.ExternalLot.IsNull())
            {
                request.ExternalLot = request.ExternalLot.Trim();
                query = query.Where(p => p.invlot.ExternalLot.Equals(request.ExternalLot, StringComparison.OrdinalIgnoreCase));
            }
            if (request.ProduceDateFrom.HasValue)
            {
                query = query.Where(x => x.invlot.ProduceDate.Value >= request.ProduceDateFrom.Value);
            }
            if (request.ProduceDateTo.HasValue)
            {
                query = query.Where(x => x.invlot.ProduceDate.Value < request.ProduceDateTo.Value);
            }
            if (request.ExpiryDateFrom.HasValue)
            {
                query = query.Where(x => x.invlot.ExpiryDate.Value >= request.ExpiryDateFrom.Value);
            }
            if (request.ExpiryDateTo.HasValue)
            {
                query = query.Where(x => x.invlot.ExpiryDate.Value < request.ExpiryDateTo.Value);
            }
            if (!request.LotAttr01.IsNull())
            {
                request.LotAttr01 = request.LotAttr01.Trim();
                query = query.Where(p => p.invlot.LotAttr01.Equals(request.LotAttr01, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr02.IsNull())
            {
                request.LotAttr02 = request.LotAttr02.Trim();
                query = query.Where(p => p.invlot.LotAttr02.Equals(request.LotAttr02, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr03.IsNull())
            {
                request.LotAttr03 = request.LotAttr03.Trim();
                query = query.Where(p => p.invlot.LotAttr03.Equals(request.LotAttr03, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr04.IsNull())
            {
                request.LotAttr04 = request.LotAttr04.Trim();
                query = query.Where(p => p.invlot.LotAttr04.Equals(request.LotAttr04, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr05.IsNull())
            {
                request.LotAttr05 = request.LotAttr05.Trim();
                query = query.Where(p => p.invlot.LotAttr05.Equals(request.LotAttr05, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr06.IsNull())
            {
                request.LotAttr06 = request.LotAttr06.Trim();
                query = query.Where(p => p.invlot.LotAttr06.Equals(request.LotAttr06, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr07.IsNull())
            {
                request.LotAttr07 = request.LotAttr07.Trim();
                query = query.Where(p => p.invlot.LotAttr07.Equals(request.LotAttr07, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr08.IsNull())
            {
                request.LotAttr08 = request.LotAttr08.Trim();
                query = query.Where(p => p.invlot.LotAttr08.Equals(request.LotAttr08, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.LotAttr09.IsNull())
            {
                request.LotAttr09 = request.LotAttr09.Trim();
                query = query.Where(p => p.invlot.LotAttr09.Equals(request.LotAttr09, StringComparison.OrdinalIgnoreCase));
            }
            if (request.GreaterThanZero == true)
            {
                query = query.Where(p => p.invlotloclpnQty > 0);
            }

            #endregion

            var inventorys = query.Select(p => new StockTransferLotListDto()
            {
                SysId = p.invlotloclpnSysId,
                SkuSysId = p.invlot.SkuSysId,
                SkuName = p.SkuName,
                SkuCode = p.SkuCode,
                SkuDescr = p.SkuDescr,
                UPC = p.UPC,
                WarehouseName = p.WarehouseName,
                Loc = p.Loc,
                Lot = p.invlot.Lot,
                Qty = p.invlotloclpnQty,
                ProduceDate = p.invlot.ProduceDate,
                ExpiryDate = p.invlot.ExpiryDate,
                LotAttr01 = p.invlot.LotAttr01,
                LotAttr02 = p.invlot.LotAttr02,
                LotAttr03 = p.invlot.LotAttr03,
                LotAttr04 = p.invlot.LotAttr04,
                LotAttr05 = p.invlot.LotAttr05,
                LotAttr06 = p.invlot.LotAttr06,
                LotAttr07 = p.invlot.LotAttr07,
                LotAttr08 = p.invlot.LotAttr08,
                LotAttr09 = p.invlot.LotAttr09,
                DisplayQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.invlotloclpnQty * 1.00m) / p.FieldValue01.Value), 3) : p.invlotloclpnQty
            }).Distinct();

            request.iTotalDisplayRecords = inventorys.Count();
            inventorys = inventorys.OrderBy(p => p.SkuName).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(inventorys, request);
        }

        public StockTransferDto GetStockTransferBySysId(Guid sysid)
        {
            var query = from invlot in Context.invlots
                        join invlotloclpn in Context.invlotloclpns on invlot.SkuSysId equals invlotloclpn.SkuSysId
                        join sku in Context.skus on invlot.SkuSysId equals sku.SysId
                        join p in Context.packs on sku.PackSysId equals p.SysId
                        join lottemplate in Context.lottemplates on sku.LotTemplateSysId equals lottemplate.SysId
                        join warehouse in Context.warehouses on invlot.WareHouseSysId equals warehouse.SysId
                        where invlotloclpn.SysId == sysid
                            && invlot.WareHouseSysId == invlotloclpn.WareHouseSysId
                            && invlot.Lot == invlotloclpn.Lot
                        //select new { invlot, invlotloclpn.Loc, sku.SkuName, sku.SkuCode, sku.SkuDescr, sku.UPC, WarehouseName = warehouse.Name };
                        select new StockTransferDto()
                        {
                            FromSkuSysId = invlot.SkuSysId,
                            WarehouseSysId = invlotloclpn.WareHouseSysId,
                            SkuName = sku.SkuName,
                            SkuCode = sku.SkuCode,
                            SkuDescr = sku.SkuDescr,
                            UPC = sku.UPC,
                            WarehouseName = warehouse.Name,
                            FromLoc = invlotloclpn.Loc,
                            FromExternalLot = invlot.ExternalLot,
                            FromLot = invlotloclpn.Lot,
                            CurrentQty = (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty),
                            DisplayCurrentQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                ? Math.Round(((p.FieldValue02.Value * (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty) * 1.00m) / p.FieldValue01.Value), 3) : (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty),
                            FromProduceDate = invlot.ProduceDate,
                            FromExpiryDate = invlot.ExpiryDate,
                            FromLotAttr01 = invlot.LotAttr01,
                            FromLotAttr02 = invlot.LotAttr02,
                            FromLotAttr03 = invlot.LotAttr03,
                            FromLotAttr04 = invlot.LotAttr04,
                            FromLotAttr05 = invlot.LotAttr05,
                            FromLotAttr06 = invlot.LotAttr06,
                            FromLotAttr07 = invlot.LotAttr07,
                            FromLotAttr08 = invlot.LotAttr08,
                            FromLotAttr09 = invlot.LotAttr09,

                            #region 此处让UI上目标栏位的值默认等于源值
                            ToExternalLot = invlot.ExternalLot,
                            ToLotAttr01 = invlot.LotAttr01,
                            ToLotAttr02 = invlot.LotAttr02,
                            ToLotAttr03 = invlot.LotAttr03,
                            ToLotAttr04 = invlot.LotAttr04,
                            ToLotAttr05 = invlot.LotAttr05,
                            ToLotAttr06 = invlot.LotAttr06,
                            ToLotAttr07 = invlot.LotAttr07,
                            ToLotAttr08 = invlot.LotAttr08,
                            ToLotAttr09 = invlot.LotAttr09,
                            #endregion

                            LotAttrName01 = lottemplate.Lot01,
                            LotAttrName02 = lottemplate.Lot02,
                            LotAttrName03 = lottemplate.Lot03,
                            LotAttrName04 = lottemplate.Lot04,
                            LotAttrName05 = lottemplate.Lot05,
                            LotAttrName06 = lottemplate.Lot06,
                            LotAttrName07 = lottemplate.Lot07,
                            LotAttrName08 = lottemplate.Lot08,
                            LotAttrName09 = lottemplate.Lot09,
                            LotVisible01 = lottemplate.LotVisible01,
                            LotVisible02 = lottemplate.LotVisible02,
                            LotVisible03 = lottemplate.LotVisible03,
                            LotVisible04 = lottemplate.LotVisible04,
                            LotVisible05 = lottemplate.LotVisible05,
                            LotVisible06 = lottemplate.LotVisible06,
                            LotVisible07 = lottemplate.LotVisible07,
                            LotVisible08 = lottemplate.LotVisible08,
                            LotVisible09 = lottemplate.LotVisible09
                        };

            return query.FirstOrDefault();
        }

        public Pages<StockTransferDto> GetStockTransferOrderByPage(StockTransferQuery request)
        {
            var query = from stocktransfer in Context.stocktransfers
                        join sku in Context.skus on stocktransfer.FromSkuSysId equals sku.SysId
                        join invlotloclpn in Context.invlotloclpns
                            on new { SkuSysId = stocktransfer.FromSkuSysId, stocktransfer.WareHouseSysId, Lot = stocktransfer.FromLot, Loc = stocktransfer.FromLoc }
                            equals new { invlotloclpn.SkuSysId, invlotloclpn.WareHouseSysId, invlotloclpn.Lot, invlotloclpn.Loc } into tempinvlotloclpn
                        from tw in tempinvlotloclpn.DefaultIfEmpty()
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        select new StockTransferDto()
                        {
                            SysId = stocktransfer.SysId,
                            WarehouseSysId = stocktransfer.WareHouseSysId,
                            StockTransferOrder = stocktransfer.StockTransferOrder,
                            FromSkuSysId = stocktransfer.FromSkuSysId,
                            SkuName = sku.SkuName,
                            SkuDescr = sku.SkuDescr,
                            UPC = sku.UPC,
                            CurrentQty = tw.Qty,
                            Status = stocktransfer.Status,
                            FromQty = stocktransfer.FromQty,
                            ToQty = stocktransfer.ToQty,
                            FromLoc = stocktransfer.FromLoc,
                            FromLot = stocktransfer.FromLot,
                            ToLoc = stocktransfer.ToLoc,
                            ToLot = stocktransfer.ToLot,
                            ToLotAttr01 = stocktransfer.ToLotAttr01,
                            DisplayCurrentQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * tw.Qty * 1.00m) / p1.FieldValue01.Value), 3) : tw.Qty,
                            DisplayToQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * stocktransfer.ToQty * 1.00m) / p1.FieldValue01.Value), 3) : stocktransfer.ToQty,
                            CreateDate = stocktransfer.CreateDate
                        };

            query = query.Where(p => p.WarehouseSysId == request.WarehouseSysId);
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

            if (request.Status.HasValue)
            {
                query = query.Where(p => p.Status == request.Status);
            }

            request.iTotalDisplayRecords = query.Count();
            query = query.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(query, request);
        }

        public StockTransferDto GetStockTransferOrderBySysId(Guid sysId)
        {
            var query = from stocktransfer in Context.stocktransfers
                        join sku in Context.skus on stocktransfer.FromSkuSysId equals sku.SysId
                        join lottemplate in Context.lottemplates on sku.LotTemplateSysId equals lottemplate.SysId
                        join warehouse in Context.warehouses on stocktransfer.WareHouseSysId equals warehouse.SysId
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        where stocktransfer.SysId == sysId
                        select new StockTransferDto()
                        {
                            FromSkuSysId = stocktransfer.FromSkuSysId,
                            ToSkuSysId = stocktransfer.ToSkuSysId,
                            WarehouseSysId = stocktransfer.WareHouseSysId,
                            SkuName = sku.SkuName,
                            SkuCode = sku.SkuCode,
                            SkuDescr = sku.SkuDescr,
                            UPC = sku.UPC,
                            Status = stocktransfer.Status,
                            WarehouseName = warehouse.Name,
                            FromLoc = stocktransfer.FromLoc,
                            FromExternalLot = stocktransfer.FromExternalLot,
                            FromLot = stocktransfer.FromLot,
                            FromQty = stocktransfer.FromQty,
                            FromProduceDate = stocktransfer.FromProduceDate,
                            FromExpiryDate = stocktransfer.FromExpiryDate,
                            FromLotAttr01 = stocktransfer.FromLotAttr01,
                            FromLotAttr02 = stocktransfer.FromLotAttr02,
                            FromLotAttr03 = stocktransfer.FromLotAttr03,
                            FromLotAttr04 = stocktransfer.FromLotAttr04,
                            FromLotAttr05 = stocktransfer.FromLotAttr05,
                            FromLotAttr06 = stocktransfer.FromLotAttr06,
                            FromLotAttr07 = stocktransfer.FromLotAttr07,
                            FromLotAttr08 = stocktransfer.FromLotAttr08,
                            FromLotAttr09 = stocktransfer.FromLotAttr09,

                            ToLoc = stocktransfer.ToLoc,
                            ToExternalLot = stocktransfer.ToExternalLot,
                            ToLot = stocktransfer.ToLot,
                            ToQty = stocktransfer.ToQty,
                            DisplayToQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * stocktransfer.ToQty * 1.00m) / p1.FieldValue01.Value), 3) : stocktransfer.ToQty,
                            ToProduceDate = stocktransfer.ToProduceDate,
                            ToExpiryDate = stocktransfer.ToExpiryDate,
                            ToLotAttr01 = stocktransfer.ToLotAttr01,
                            ToLotAttr02 = stocktransfer.ToLotAttr02,
                            ToLotAttr03 = stocktransfer.ToLotAttr03,
                            ToLotAttr04 = stocktransfer.ToLotAttr04,
                            ToLotAttr05 = stocktransfer.ToLotAttr05,
                            ToLotAttr06 = stocktransfer.ToLotAttr06,
                            ToLotAttr07 = stocktransfer.ToLotAttr07,
                            ToLotAttr08 = stocktransfer.ToLotAttr08,
                            ToLotAttr09 = stocktransfer.ToLotAttr09,

                            LotAttrName01 = lottemplate.Lot01,
                            LotAttrName02 = lottemplate.Lot02,
                            LotAttrName03 = lottemplate.Lot03,
                            LotAttrName04 = lottemplate.Lot04,
                            LotAttrName05 = lottemplate.Lot05,
                            LotAttrName06 = lottemplate.Lot06,
                            LotAttrName07 = lottemplate.Lot07,
                            LotAttrName08 = lottemplate.Lot08,
                            LotAttrName09 = lottemplate.Lot09,
                            LotVisible01 = lottemplate.LotVisible01,
                            LotVisible02 = lottemplate.LotVisible02,
                            LotVisible03 = lottemplate.LotVisible03,
                            LotVisible04 = lottemplate.LotVisible04,
                            LotVisible05 = lottemplate.LotVisible05,
                            LotVisible06 = lottemplate.LotVisible06,
                            LotVisible07 = lottemplate.LotVisible07,
                            LotVisible08 = lottemplate.LotVisible08,
                            LotVisible09 = lottemplate.LotVisible09

                        };

            return query.FirstOrDefault();
        }
    }
}
