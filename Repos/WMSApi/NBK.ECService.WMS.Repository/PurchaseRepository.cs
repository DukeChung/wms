using System;
using System.Collections.Generic;
using Abp.EntityFramework;
using System.Linq;
using Abp.EntityFramework.SimpleRepositories;
using FortuneLab.Models;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.Repository
{
    public class PurchaseRepository : CrudRepository, IPurchaseRepository
    {
        /// <param name="dbContextProvider"></param>
        public PurchaseRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 获取采购订单信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PurchaseViewDto GetPurchaseViewDtoBySysId(Guid sysId)
        {
            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId
                        join tf in Context.transferinventorys on po.SysId equals tf.TransferPurchaseSysId into t0
                        from tfInfo in t0.DefaultIfEmpty()
                        where po.SysId == sysId
                        select new PurchaseViewDto()
                        {
                            SysId = po.SysId,
                            PurchaseOrder = po.PurchaseOrder,
                            DeliveryDate = po.DeliveryDate,
                            ExternalOrder = po.ExternalOrder,
                            VendorName = v.VendorName,
                            PurchaseDate = po.PurchaseDate,
                            Status = po.Status,
                            LastReceiptDate = po.LastReceiptDate,
                            Type = po.Type,
                            FromWareHouseName = tfInfo.FromWareHouseName,
                            ToWareHouseName = tfInfo.ToWareHouseName,
                            TransferInventoryOrder = tfInfo.TransferInventoryOrder,
                            OutboundSysId = po.OutboundSysId,
                            OutboundOrder = po.OutboundOrder
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery)
        {
            var query = from po in Context.purchases
                        join poDetail in Context.purchasedetails on po.SysId equals poDetail.PurchaseSysId
                        join v in Context.vendors on po.VendorSysId equals v.SysId
                        join s in Context.skus on poDetail.SkuSysId equals s.SysId
                        join r in Context.receipts on po.PurchaseOrder equals r.ExternalOrder
                        into x
                        from y in x.DefaultIfEmpty()
                        join t in Context.transferinventorys on po.SysId equals t.TransferPurchaseSysId
                        into xx
                        from w in xx.DefaultIfEmpty()
                        select new { po, poDetail, v, s, y, w };

            #region 查询条件 

            query = query.Where(x => x.po.WarehouseSysId == purchaseQuery.WarehouseSysId);

            if (!string.IsNullOrEmpty(purchaseQuery.PurchaseOrderSearch))
            {
                purchaseQuery.PurchaseOrderSearch = purchaseQuery.PurchaseOrderSearch.Trim();
                query = query.Where(x => x.po.PurchaseOrder.Contains(purchaseQuery.PurchaseOrderSearch));
            }
            if (!string.IsNullOrEmpty(purchaseQuery.VendorNameSearch))
            {
                purchaseQuery.VendorNameSearch = purchaseQuery.VendorNameSearch.Trim();
                query = query.Where(x => x.v.VendorName.Contains(purchaseQuery.VendorNameSearch));
            }
            if (!string.IsNullOrEmpty(purchaseQuery.SkuCodeSearch))
            {
                purchaseQuery.SkuCodeSearch = purchaseQuery.SkuCodeSearch.Trim();
                query = query.Where(x => x.s.SkuCode.Contains(purchaseQuery.SkuCodeSearch));
            }
            if (!string.IsNullOrEmpty(purchaseQuery.SkuNameSearch))
            {
                purchaseQuery.SkuNameSearch = purchaseQuery.SkuNameSearch.Trim();
                query = query.Where(x => x.s.SkuName.Contains(purchaseQuery.SkuNameSearch));
            }
            if (!string.IsNullOrEmpty(purchaseQuery.ExternalOrderSearch))
            {
                purchaseQuery.ExternalOrderSearch = purchaseQuery.ExternalOrderSearch.Trim();
                query = query.Where(x => x.po.ExternalOrder.Contains(purchaseQuery.ExternalOrderSearch));
            }
            if (purchaseQuery.ReceiptStartDateSearch.HasValue)
            {
                query = query.Where(x => x.po.LastReceiptDate >= purchaseQuery.ReceiptStartDateSearch.Value);
            }
            if (purchaseQuery.ReceiptEndDateSearch.HasValue)
            {
                purchaseQuery.ReceiptEndDateSearch = Convert.ToDateTime(purchaseQuery.ReceiptEndDateSearch).AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.po.LastReceiptDate <= purchaseQuery.ReceiptEndDateSearch.Value);
            }
            if (purchaseQuery.StatusSearch.HasValue)
            {
                query = query.Where(x => x.po.Status == purchaseQuery.StatusSearch.Value);
            }
            if (purchaseQuery.TypeSearch.HasValue)
            {
                query = query.Where(x => x.po.Type == purchaseQuery.TypeSearch.Value);
            }
            if (!string.IsNullOrEmpty(purchaseQuery.UpcCodeSearch))
            {
                purchaseQuery.UpcCodeSearch = purchaseQuery.UpcCodeSearch.Trim();
                query = query.Where(x => purchaseQuery.UpcCodeSearch.Contains(x.s.UPC));
            }
            if (!string.IsNullOrEmpty(purchaseQuery.ReceiptOrderSearch))
            {
                purchaseQuery.ReceiptOrderSearch = purchaseQuery.ReceiptOrderSearch.Trim();
                query = query.Where(x => x.y.ReceiptOrder.Contains(purchaseQuery.ReceiptOrderSearch));
            }

            if (!string.IsNullOrEmpty(purchaseQuery.TransferInventoryOrderSearch))
            {
                purchaseQuery.TransferInventoryOrderSearch = purchaseQuery.TransferInventoryOrderSearch.Trim();
                query = query.Where(x => x.w.TransferInventoryOrder.Contains(purchaseQuery.TransferInventoryOrderSearch));
            }

            if (purchaseQuery.ToWareHouseSysId.HasValue)
            {
                query = query.Where(x => x.w.ToWareHouseSysId == purchaseQuery.ToWareHouseSysId);
            }

            if (purchaseQuery.AuditingDateFrom.HasValue)
            {
                query = query.Where(x => x.po.AuditingDate >= purchaseQuery.AuditingDateFrom.Value);
            }
            if (purchaseQuery.AuditingDateTo.HasValue)
            {
                query = query.Where(x => x.po.AuditingDate <= purchaseQuery.AuditingDateTo.Value);
            }
            if (!string.IsNullOrEmpty(purchaseQuery.OutboundOrderSearch))
            {
                purchaseQuery.OutboundOrderSearch = purchaseQuery.OutboundOrderSearch.Trim();
                query = query.Where(x => x.po.OutboundOrder == purchaseQuery.OutboundOrderSearch);
            }
            if (!string.IsNullOrEmpty(purchaseQuery.BatchNumber))
            {
                purchaseQuery.BatchNumber = purchaseQuery.BatchNumber.Trim();
                query = query.Where(x => x.po.BatchNumber == purchaseQuery.BatchNumber);
            }

            #endregion

            var purchase = query.Select(x => new PurchaseListDto()
            {
                SysId = x.po.SysId,
                PurchaseOrder = x.po.PurchaseOrder,
                VendorName = x.v.VendorName,
                Type = x.po.Type,
                Status = x.po.Status,
                LastReceiptDate = x.po.LastReceiptDate,
                AuditingDate = x.po.AuditingDate,
                AuditingName = x.po.AuditingName,
                ExternalOrder = x.po.ExternalOrder,
                BatchNumber = x.po.BatchNumber
            }).Distinct();

            purchaseQuery.iTotalDisplayRecords = purchase.Count();
            purchase = purchase.OrderByDescending(p => p.AuditingDate).Skip(purchaseQuery.iDisplayStart).Take(purchaseQuery.iDisplayLength);
            return ConvertPages<PurchaseListDto>(purchase, purchaseQuery);


        }

        /// <summary>
        /// 获取PurchaseDetailViewDto 根据主表的SysId
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailViewDto> GetPurchaseDetailViewBySysId(Guid sysId)
        {
            var query = from poDetail in Context.purchasedetails
                        join sku in Context.skus on poDetail.SkuSysId equals sku.SysId
                        join temp in Context.lottemplates on sku.LotTemplateSysId equals temp.SysId
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on poDetail.UOMSysId equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        join u2 in Context.uoms on p1.FieldUom02 equals u2.SysId into t4
                        from ti4 in t4.DefaultIfEmpty()

                            //join p0 in Context.purchases on poDetail.PurchaseSysId equals p0.SysId
                            //join rc in Context.receipts on p0.PurchaseOrder equals rc.ExternalOrder into rt
                            //from receipt in rt.DefaultIfEmpty()
                            //join rcd in Context.receiptdatarecord on receipt.SysId equals rcd.ReceiptSysId into rcdt
                            //from rcdc in rcdt.DefaultIfEmpty()

                        where poDetail.PurchaseSysId == sysId
                        select new PurchaseDetailViewDto
                        {
                            SysId = poDetail.SysId,
                            SkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            IsMaterial = sku.IsMaterial,
                            SkuSpecialTypes = sku.SpecialTypes,
                            GiftQty = poDetail.GiftQty,
                            ReceivedGiftQty = poDetail.ReceivedGiftQty,
                            SkuUPC = sku.UPC,
                            SkuDescr = sku.SkuDescr,
                            PackSysId = poDetail.PackSysId,
                            UomSysId = poDetail.UOMSysId,
                            PackCode = poDetail.PackCode,
                            UomCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti4.UOMCode : ti3.UOMCode,
                            Qty = poDetail.Qty,
                            PurchaseGiftQty = poDetail.GiftQty,
                            ReceivedQty = poDetail.ReceivedQty,
                            PackFactor = poDetail.PackFactor,
                            UPC01 = p1.UPC01,
                            UPC02 = p1.UPC02,
                            UPC03 = p1.UPC03,
                            UPC04 = p1.UPC04,
                            UPC05 = p1.UPC05,
                            FieldValue01 = p1.FieldValue01,
                            FieldValue02 = p1.FieldValue02,
                            FieldValue03 = p1.FieldValue03,
                            FieldValue04 = p1.FieldValue04,
                            FieldValue05 = p1.FieldValue05,
                            DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * poDetail.Qty * 1.00m) / p1.FieldValue01.Value), 3) : poDetail.Qty,

                            DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * poDetail.ReceivedQty * 1.00m) / p1.FieldValue01.Value), 3) : poDetail.ReceivedQty,
                            DisplayRejectedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * poDetail.RejectedQty * 1.00m) / p1.FieldValue01.Value), 3) : poDetail.RejectedQty,
                            RejectedQty = poDetail.RejectedQty,
                            CurrentQty = poDetail.Qty - poDetail.ReceivedQty - poDetail.RejectedQty,
                            CurrentGiftQty = poDetail.GiftQty - poDetail.ReceivedGiftQty - poDetail.RejectedGiftQty,
                            Remark = poDetail.Remark,
                            LotTemplateDto = new LotTemplateDto()
                            {
                                Lot01 = temp.Lot01,
                                LotVisible01 = temp.LotVisible01,
                                LotMandatory01 = temp.LotMandatory01,
                                LotType01 = temp.LotType01,
                                LotValue01 = temp.LotValue01,
                                DefaultIN01 = temp.DefaultIN01,
                                Lot02 = temp.Lot02,
                                LotVisible02 = temp.LotVisible02,
                                LotMandatory02 = temp.LotMandatory02,
                                LotType02 = temp.LotType02,
                                LotValue02 = temp.LotValue02,
                                DefaultIN02 = temp.DefaultIN02,
                                Lot03 = temp.Lot03,
                                LotVisible03 = temp.LotVisible03,
                                LotMandatory03 = temp.LotMandatory03,
                                LotType03 = temp.LotType03,
                                LotValue03 = temp.LotValue03,
                                DefaultIN03 = temp.DefaultIN03,
                                Lot04 = temp.Lot04,
                                LotVisible04 = temp.LotVisible04,
                                LotMandatory04 = temp.LotMandatory04,
                                LotType04 = temp.LotType04,
                                LotValue04 = temp.LotValue04,
                                DefaultIN04 = temp.DefaultIN04,
                                Lot05 = temp.Lot05,
                                LotVisible05 = temp.LotVisible05,
                                LotMandatory05 = temp.LotMandatory05,
                                LotType05 = temp.LotType05,
                                LotValue05 = temp.LotValue05,
                                DefaultIN05 = temp.DefaultIN05,
                                Lot06 = temp.Lot06,
                                LotVisible06 = temp.LotVisible06,
                                LotMandatory06 = temp.LotMandatory06,
                                LotType06 = temp.LotType06,
                                LotValue06 = temp.LotValue06,
                                DefaultIN06 = temp.DefaultIN06,
                                Lot07 = temp.Lot07,
                                LotVisible07 = temp.LotVisible07,
                                LotMandatory07 = temp.LotMandatory07,
                                LotType07 = temp.LotType07,
                                LotValue07 = temp.LotValue07,
                                DefaultIN07 = temp.DefaultIN07,
                                Lot08 = temp.Lot08,
                                LotVisible08 = temp.LotVisible08,
                                LotMandatory08 = temp.LotMandatory08,
                                LotType08 = temp.LotType08,
                                LotValue08 = temp.LotValue08,
                                DefaultIN08 = temp.DefaultIN08,
                                Lot09 = temp.Lot09,
                                LotVisible09 = temp.LotVisible09,
                                LotMandatory09 = temp.LotMandatory09,
                                LotType09 = temp.LotType09,
                                LotValue09 = temp.LotValue09,
                                DefaultIN09 = temp.DefaultIN09,
                                Lot10 = temp.Lot10,
                                LotVisible10 = temp.LotVisible10,
                                LotMandatory10 = temp.LotMandatory10,
                                Lot11 = temp.Lot11,
                                LotVisible11 = temp.LotVisible11,
                                LotMandatory11 = temp.LotMandatory11,
                                Lot12 = temp.Lot12,
                                LotVisible12 = temp.LotVisible12,
                                LotMandatory12 = temp.LotMandatory12
                            }
                        };
            var list = query.ToList();
            if (list != null && list.Count > 0)
            {
                list.ForEach(p =>
                {
                    p.DisplayCurrentQty = p.DisplayQty.GetValueOrDefault() - p.DisplayReceivedQty.GetValueOrDefault() - p.DisplayRejectedQty.GetValueOrDefault();
                });
            }

            return list;
        }

        /// <summary>
        /// 获取ReceiptDetail Sku 是空的对象
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailSkuDto> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId)
        {
            var query = from rd in Context.purchasedetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        where rd.PurchaseSysId == purchaseSysId && (s.UPC == null || s.UPC == "")
                        select new PurchaseDetailSkuDto
                        {
                            PurchaseSysId = rd.PurchaseSysId,
                            PurchaseDetailSysId = rd.SysId,
                            SkuSysId = s.SysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            SkuUPC = s.UPC,
                            NetWeight = s.NetWeight,
                            Length = s.Length,
                            Width = s.Width,
                            Height = s.Width,
                            DaysToExpire = s.DaysToExpire,

                        };
            return query.ToList();
        }

        public List<PurchaseDetailSkuDto> GetPurchaseDetailSku(Guid purchaseSysId)
        {
            var query = from rd in Context.purchasedetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        where rd.PurchaseSysId == purchaseSysId
                        select new PurchaseDetailSkuDto
                        {
                            PurchaseSysId = rd.PurchaseSysId,
                            PurchaseDetailSysId = rd.SysId,
                            SkuSysId = s.SysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            RecommendLoc = s.RecommendLoc,
                            SkuUPC = s.UPC,
                            NetWeight = s.NetWeight,
                            Length = s.Length,
                            Width = s.Width,
                            Height = s.Width,
                            DaysToExpire = s.DaysToExpire,
                            UOMSysId = rd.UOMSysId,
                            PackSysId = rd.PackSysId,
                            PurchasePrice = rd.PurchasePrice,
                            ReceivedQty = rd.ReceivedQty,
                            Qty = rd.Qty
                        };
            return query.ToList();
        }

        /// <summary>
        /// 根据源出库拣货信息生成 退货入库 收货信息
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<ReceiptdetailAutoShelvesDto> GetReceiptdetailForAutoShelves(Guid outboundSysId, Guid receiptSysId, int userId, string userName)
        {
            var query = from pickdetails in Context.pickdetails
                        join invlot in Context.invlots
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Lot } equals new { invlot.WareHouseSysId, invlot.SkuSysId, invlot.Lot }
                        where pickdetails.OutboundSysId == outboundSysId 
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                            && pickdetails.Qty > 0
                        select new ReceiptdetailAutoShelvesDto
                        {
                            ReceiptSysId = receiptSysId,
                            SkuSysId = pickdetails.SkuSysId,
                            Status = (int)ReceiptDetailStatus.Received,
                            ExpectedQty = pickdetails.Qty,
                            ReceivedQty = pickdetails.Qty,
                            RejectedQty = 0,
                            CreateBy = userId,
                            CreateUserName = userName,
                            CreateDate = DateTime.Now,
                            UpdateBy = userId,
                            UpdateUserName = userName,
                            UpdateDate = DateTime.Now,
                            UOMSysId = pickdetails.UOMSysId,
                            PackSysId = pickdetails.PackSysId,
                            ToLoc = pickdetails.Loc,
                            ToLot = pickdetails.Lot,
                            ToLpn = pickdetails.Lpn,
                            LotAttr01 = invlot.LotAttr01,
                            LotAttr02 = invlot.LotAttr02,
                            LotAttr03 = invlot.LotAttr03,
                            LotAttr04 = invlot.LotAttr04,
                            LotAttr05 = invlot.LotAttr05,
                            LotAttr06 = invlot.LotAttr06,
                            LotAttr07 = invlot.LotAttr07,
                            LotAttr08 = invlot.LotAttr08,
                            LotAttr09 = invlot.LotAttr09,
                            ExternalLot = invlot.ExternalLot,
                            ProduceDate = invlot.ProduceDate,
                            ExpiryDate = invlot.ExpiryDate,
                            ReceivedDate = DateTime.Now,
                            Price = invlot.Price,
                            ShelvesStatus = (int)ShelvesStatus.Finish,
                            ShelvesQty = pickdetails.Qty.Value,
                            PickDetailSysId = pickdetails.SysId
                        };

            return query.ToList();
        }


        /// <summary>
        /// 根据源出库拣货信息生成 退货入库 收货信息
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<ReceiptdetailAutoShelvesDto> GetReceiptdetailForAutoShelves(Guid outboundSysId, Guid receiptSysId, int userId, string userName, List<Guid> skuSysIds)
        {
            var query = from pickdetails in Context.pickdetails
                        join invlot in Context.invlots
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Lot } equals new { invlot.WareHouseSysId, invlot.SkuSysId, invlot.Lot }
                        where pickdetails.OutboundSysId == outboundSysId && pickdetails.Status == (int)PickDetailStatus.Finish && skuSysIds.Contains(pickdetails.SkuSysId)
                        select new ReceiptdetailAutoShelvesDto
                        {
                            ReceiptSysId = receiptSysId,
                            SkuSysId = pickdetails.SkuSysId,
                            Status = (int)ReceiptDetailStatus.Received,
                            ExpectedQty = pickdetails.Qty,
                            ReceivedQty = pickdetails.Qty,
                            RejectedQty = 0,
                            CreateBy = userId,
                            CreateUserName = userName,
                            CreateDate = DateTime.Now,
                            UpdateBy = userId,
                            UpdateUserName = userName,
                            UpdateDate = DateTime.Now,
                            UOMSysId = pickdetails.UOMSysId,
                            PackSysId = pickdetails.PackSysId,
                            ToLoc = pickdetails.Loc,
                            ToLot = pickdetails.Lot,
                            ToLpn = pickdetails.Lpn,
                            LotAttr01 = invlot.LotAttr01,
                            LotAttr02 = invlot.LotAttr02,
                            LotAttr03 = invlot.LotAttr03,
                            LotAttr04 = invlot.LotAttr04,
                            LotAttr05 = invlot.LotAttr05,
                            LotAttr06 = invlot.LotAttr06,
                            LotAttr07 = invlot.LotAttr07,
                            LotAttr08 = invlot.LotAttr08,
                            LotAttr09 = invlot.LotAttr09,
                            ExternalLot = invlot.ExternalLot,
                            ProduceDate = invlot.ProduceDate,
                            ExpiryDate = invlot.ExpiryDate,
                            ReceivedDate = DateTime.Now,
                            Price = invlot.Price,
                            ShelvesStatus = (int)ShelvesStatus.Finish,
                            ShelvesQty = pickdetails.Qty.Value,
                            PickDetailSysId = pickdetails.SysId
                        };

            return query.ToList();
        }

        public List<invlot> GetInvlotForAutoShelves(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join invlot in Context.invlots
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Lot } equals new { invlot.WareHouseSysId, invlot.SkuSysId, invlot.Lot }
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select invlot;

            return query.ToList();
        }

        public List<invskuloc> GetInvskulocForAutoShelves(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join invskuloc in Context.invskulocs
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Loc } equals new { invskuloc.WareHouseSysId, invskuloc.SkuSysId, invskuloc.Loc }
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select invskuloc;

            return query.ToList();
        }

        public List<invlotloclpn> GetInvlotloclpnForAutoShelves(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join invlotloclpn in Context.invlotloclpns
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Loc, pickdetails.Lot, pickdetails.Lpn }
                            equals new { invlotloclpn.WareHouseSysId, invlotloclpn.SkuSysId, invlotloclpn.Loc, invlotloclpn.Lot, invlotloclpn.Lpn }
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select invlotloclpn;

            return query.ToList();
        }

        public List<PurchaseAutoShelvesSkuInfo> GetSkuForAutoShelves(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join sku in Context.skus on pickdetails.SkuSysId equals sku.SysId
                        join pack in Context.packs on pickdetails.PackSysId equals pack.SysId
                        join uom in Context.uoms on pickdetails.UOMSysId equals uom.SysId
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select new PurchaseAutoShelvesSkuInfo
                        {
                            PickDetailSysId = pickdetails.SysId,
                            OtherSkuId = sku.OtherId,
                            SkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            PackSysId = pack.SysId,
                            PackCode = pack.PackCode,
                            UOMSysId = uom.SysId,
                            UOMCode = uom.UOMCode
                        };
            return query.ToList();
        }

        /// <summary>
        /// 指定入库批号
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public void UpdatePurchaseBatchNumberBySysId(List<Guid> sysIdList, string batchNumber)
        {
            string UpdatePurchase =
                $"Update Purchase set BatchNumber=@BatchNumber where SysId in(@SysId) and Status={(int)PurchaseStatus.New}";

            var sysIds = StringHelper.GetConvertSqlIn<Guid>(sysIdList);
            UpdatePurchase = UpdatePurchase.Replace("@SysId", sysIds);
            var result = base.Context.Database.ExecuteSqlCommand(UpdatePurchase.ToString(),
                new MySqlParameter("@BatchNumber", batchNumber));
            if (sysIdList.Count() != result)
            {
                throw new Exception("入库单状态发生变化存在不等于待入库单据,请刷新页面后从新操作!");
            }
        }

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysIdList"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        public bool UpdatePurchaseBusinessTypeBySysId(List<Guid> sysIdList, string businessType)
        {
            try
            {
                string strSql = $"Update Purchase set BusinessType=@BusinessType where SysId in(@SysId) and Status={(int)PurchaseStatus.New}";

                var sysIds = StringHelper.GetConvertSqlIn<Guid>(sysIdList);
                strSql = strSql.Replace("@SysId", sysIds);
                var result = base.Context.Database.ExecuteSqlCommand(strSql.ToString(),
                    new MySqlParameter("@BusinessType", businessType));
                if (sysIdList.Count() != result)
                {
                    throw new Exception("入库单状态发生变化，存在不等于待入库的单据，请刷新页面后重新操作!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
    }
}