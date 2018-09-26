using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System.Text;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.Repository
{
    public class PrintRepository : CrudRepository, IPrintRepository
    {
        public object g { get; private set; }

        /// <param name="dbContextProvider"></param>
        public PrintRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
        /// <summary>
        /// 按订单打印
        /// </summary>
        /// <param name="pickDetailOrder">拣货单ID</param>
        /// <returns></returns>
        public List<PrintPickDetailDto> GetPrintPickDetailByOrderDto(string pickDetailOrder)
        {
            var query = (from pd in Context.pickdetails
                         join od in Context.outbounddetails on pd.OutboundDetailSysId equals od.SysId
                         join o in Context.outbounds on od.OutboundSysId equals o.SysId into t0
                         from outbound in t0.DefaultIfEmpty()
                         join s in Context.skus on pd.SkuSysId equals s.SysId
                         join u in Context.uoms on pd.UOMSysId equals u.SysId
                         join inv in Context.invlots on pd.SkuSysId equals inv.SkuSysId
                         where pd.PickDetailOrder == pickDetailOrder && inv.Lot == pd.Lot && inv.WareHouseSysId == pd.WareHouseSysId
                         group new { pd, s, u, inv, outbound.Channel, outbound.OutboundChildType } by
                             new { pd.SkuSysId, s.SysId, s.UPC, s.SkuName, s.SkuDescr, pd.Loc, pd.Lot, u.UOMCode, od.PackFactor, outbound.Channel }
                into g
                         select new PrintPickDetailDto
                         {
                             UPC = g.Key.UPC,
                             SkuSysId = g.Key.SysId,
                             SkuName = g.Key.SkuName,
                             SkuDescr = g.Key.SkuDescr,
                             Loc = g.Key.Loc,
                             Lot = g.Key.Lot,
                             Qty = g.Sum(x => x.pd.Qty),
                             UomName = g.Key.UOMCode,
                             Channel = g.Key.Channel,
                             PackFactor = g.Key.PackFactor
                         }).Distinct().OrderBy(p => new { p.Loc, p.UPC, p.Lot }).ToList();


            return query;
        }

        public PrintReceiptDto GetPrintReceiptDto(string receiptOrder)
        {
            var query = from p in Context.purchases
                        join r in Context.receipts on p.PurchaseOrder equals r.ExternalOrder
                        join v in Context.vendors on p.VendorSysId equals v.SysId
                        join tf in Context.transferinventorys on p.SysId equals tf.TransferPurchaseSysId into t0
                        from tfinfo in t0.DefaultIfEmpty()
                        where r.ReceiptOrder == receiptOrder
                        select new PrintReceiptDto
                        {
                            SysId = r.SysId,
                            ReceiptOrder = r.ReceiptOrder,
                            ExternalOrder = r.ExternalOrder,
                            PurchaseDate = p.PurchaseDate,
                            ReceipDate = r.ReceiptDate,
                            VendorName = v.VendorName,
                            VendorPhone = v.VendorPhone,
                            VendorContacts = v.VendorContacts,
                            PurchaseDescr = p.Descr,
                            TransferInventoryOrder = tfinfo.TransferInventoryOrder,
                            FromWareHouseName = tfinfo.FromWareHouseName,
                            ToWareHouseName = tfinfo.ToWareHouseName,
                            ReceiptType = r.ReceiptType,
                            AppointUserNames = r.AppointUserNames
                        };

            return query.FirstOrDefault();
        }

        public List<PrintReceiptDetailDto> GetPrintReceiptDetailDto(string receiptOrder)
        {
            var query = from r in Context.receipts
                        join p in Context.purchases on r.ExternalOrder equals p.PurchaseOrder
                        join pd in Context.purchasedetails on p.SysId equals pd.PurchaseSysId
                        join rdc in Context.receiptdatarecord on r.ReceiptOrder equals rdc.ReceiptOrder into t0
                        from rdcp in t0.DefaultIfEmpty()
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join p1 in Context.packs on s.PackSysId equals p1.SysId
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t3
                        from u1 in t3.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom02 equals u.SysId into t4
                        from u2 in t4.DefaultIfEmpty()
                        where r.ReceiptOrder == receiptOrder
                        select new PrintReceiptDetailDto
                        {
                            SkuSysId = s.SysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? u2.UOMCode : u1.UOMCode,
                            PurchaseQty = pd.Qty,
                            DisplayPurchaseQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * pd.Qty * 1.00m) / p1.FieldValue01.Value), 3) : pd.Qty,
                            ReceivedQty = pd.ReceivedQty,
                            DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * pd.ReceivedQty * 1.00m) / p1.FieldValue01.Value), 3) : pd.ReceivedQty,
                            Remark = pd.Remark,
                            PackFactor = pd.PackFactor,
                            OtherId = s.OtherId,
                            GiftQty = rdcp.GiftQty,
                            AdjustmentQty = rdcp.AdjustmentQty
                        };

            return query.ToList();
        }

        /// <summary>
        /// 嗨曲男简直了，方法名和上面的方法写反了，大家开发的时候多注意别进坑！
        /// </summary>
        /// <param name="receiptOrder"></param>
        /// <returns></returns>
        public List<PrintReceiptDetailDto> GetPrintReceiptDetailDtoByPurchaseDetail(string receiptOrder)
        {
            var query = from r in Context.receipts
                        join rd in Context.receiptdetails on r.SysId equals rd.ReceiptSysId
                        join pu in Context.purchases on r.ExternalOrder equals pu.PurchaseOrder
                        join pd in Context.purchasedetails on pu.SysId equals pd.PurchaseSysId
                        join rdc in Context.receiptdatarecord on new { rd.ReceiptSysId, rd.SkuSysId } equals new { rdc.ReceiptSysId, rdc.SkuSysId } into t0
                        from rdcp in t0.DefaultIfEmpty()
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        //join u in Context.uoms on pd.UOMSysId equals u.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t3
                        from u1 in t3.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom02 equals u.SysId into t4
                        from u2 in t4.DefaultIfEmpty()
                        where r.ReceiptOrder == receiptOrder && rd.SkuSysId == pd.SkuSysId
                        select new PrintReceiptDetailDto
                        {
                            SkuSysId = s.SysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? u2.UOMCode : u1.UOMCode,//u.UOMCode,
                            PurchaseQty = pd.Qty,
                            DisplayPurchaseQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * pd.Qty * 1.00m) / p1.FieldValue01.Value), 3) : pd.Qty,
                            ReceivedQty = pd.ReceivedQty,
                            DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * pd.ReceivedQty * 1.00m) / p1.FieldValue01.Value), 3) : pd.ReceivedQty,
                            CurrentReceivedQty = rd.ReceivedQty,
                            DisplayCurrentReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (rd.ReceivedQty.HasValue ? rd.ReceivedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (rd.ReceivedQty.HasValue ? rd.ReceivedQty.Value : 0),
                            Remark = pd.Remark,
                            PackFactor = pd.PackFactor,
                            OtherId = s.OtherId,
                            GiftQty = rdcp.GiftQty,
                            AdjustmentQty = rdcp.AdjustmentQty
                        };

            return query.ToList();
        }

        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrintPurchaseViewDto GetPrintPurchaseViewDtoBySysId(Guid sysId)
        {
            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId
                        //join tf in Context.transferinventorys on po.SysId equals tf.TransferPurchaseSysId into t0
                        //from tfInfo in t0.DefaultIfEmpty()
                        join w in Context.warehouses on po.FromWareHouseSysId equals w.SysId into t0
                        from w1 in t0.DefaultIfEmpty()
                        where po.SysId == sysId
                        select new PrintPurchaseViewDto()
                        {
                            PurchaseOrder = po.PurchaseOrder,
                            DeliveryDate = po.DeliveryDate,
                            VendorName = v.VendorName,
                            VendorContacts = v.VendorContacts,
                            VendorPhone = v.VendorPhone,
                            PurchaseDate = po.PurchaseDate,
                            Status = po.Status,
                            Type = po.Type,
                            LastReceiptDate = po.LastReceiptDate,
                            FromWareHouseName = w1.Name,
                            TransferInventoryOrder = po.ExternalOrder,
                            Descr = po.Descr
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 打印采购单明细
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        public List<PrintPurchaseDetailViewDto> GetPrintPurchaseDetailViewBySysId(Guid sysId)
        {
            var query = from poDetail in Context.purchasedetails
                        join sku in Context.skus on poDetail.SkuSysId equals sku.SysId
                        join temp in Context.lottemplates on sku.LotTemplateSysId equals temp.SysId
                        join p in Context.packs on sku.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t3
                        from u1 in t3.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom02 equals u.SysId into t4
                        from u2 in t4.DefaultIfEmpty()
                        where poDetail.PurchaseSysId == sysId
                        select new PrintPurchaseDetailViewDto
                        {
                            SkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            SkuUPC = sku.UPC,
                            SkuDescr = sku.SkuDescr,
                            PackCode = poDetail.PackCode,
                            UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? u2.UOMCode : u1.UOMCode,//poDetail.UomCode,
                            Qty = poDetail.Qty,
                            DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * poDetail.Qty * 1.00m) / p1.FieldValue01.Value), 3) : poDetail.Qty,
                            ReceivedQty = poDetail.ReceivedQty,
                            DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * poDetail.ReceivedQty * 1.00m) / p1.FieldValue01.Value), 3) : poDetail.ReceivedQty,
                            Remark = poDetail.Remark,
                            GiftQty = poDetail.GiftQty,
                            PackFactor = poDetail.PackFactor
                        };
            return query.ToList();
        }

        public PrintVanningDetailDto GetPrintVanningDetailDto(Guid vanningDetailSysId)
        {
            var query = from vd in Context.vanningdetails
                        join van in Context.vannings on vd.VanningSysId equals van.SysId
                        join ob in Context.outbounds on van.OutboundSysId equals ob.SysId
                        where vd.SysId == vanningDetailSysId
                        select new PrintVanningDetailDto()
                        {
                            OutboundSysId = ob.SysId,
                            OutboundOrder = ob.OutboundOrder,
                            VanningOrder = van.VanningOrder,
                            Weight = vd.Weight ?? 0,
                            ContainerNumber = vd.ContainerNumber,
                            ConsigneeName = ob.ConsigneeName,
                            ConsigneeAddress = ob.ConsigneeAddress,
                            ConsigneePhone = ob.ConsigneePhone,
                            DetailAddress = (ob.ConsigneeProvince ?? string.Empty) + (ob.ConsigneeCity ?? string.Empty) + (ob.ConsigneeArea ?? string.Empty) + (ob.ConsigneeAddress ?? string.Empty),
                            UpdateUserName = ob.UpdateUserName,
                            UpdateDate = vd.UpdateDate,
                            Freight = ob.Freight ?? 0.00m,
                            PlatformOrder = ob.PlatformOrder,
                            DiscountPrice = ob.DiscountPrice ?? 0.00m,
                            CouponPrice = ob.CouponPrice
                        };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取装箱单合计
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<PrintVanningDetailSkuDto> GetPrintVanningSkuList(Guid outboundSysId)
        {
            var query = from od in Context.outbounddetails
                        join o in Context.outbounds on od.OutboundSysId equals o.SysId
                        //join vd in Context.vanningdetails on vpd.VanningDetailSysId equals vd.SysId
                        where o.SysId == outboundSysId
                        //&& pd.Status != (int)PickDetailStatus.Cancel && vd.Status != (int)VanningStatus.Cancel
                        select new PrintVanningDetailSkuDto()
                        {
                            Qty = od.Qty.Value,
                            TotalPrice = ((od.Qty ?? 0) - od.GiftQty) * (od.Price ?? 0),
                        };

            return query.ToList();
        }

        public List<PrintVanningDetailSkuDto> GetPrintVanningDetailSkuDtoList(Guid vanningDetailSysId)
        {
            var query = from vd in Context.vanningdetails
                        join vpd in Context.vanningpickdetails on vd.SysId equals vpd.VanningDetailSysId
                        join pd in Context.pickdetails on vpd.PickDetailSysId equals pd.SysId
                        join od in Context.outbounddetails on pd.OutboundDetailSysId equals od.SysId
                        join sku in Context.skus on vpd.SkuSysId equals sku.SysId
                        join u in Context.uoms on vpd.UOMSysId equals u.SysId
                        where vd.SysId == vanningDetailSysId && vd.Status != (int)VanningStatus.Cancel
                        select new PrintVanningDetailSkuDto()
                        {
                            OtherId = sku.OtherId,
                            UPC = sku.UPC,
                            SkuName = sku.SkuName,
                            SkuDescr = sku.SkuDescr,
                            UOMCode = u.UOMCode,
                            Qty = vpd.Qty.Value,
                            Price = od.Price ?? 0,
                            IsGift = od.IsGift,
                            TotalPrice = ((vpd.Qty ?? 0) - od.GiftQty) * (od.Price ?? 0),
                            GiftQty = od.GiftQty
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取装箱明细数据
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        public List<PrintVanningDetailSkuDto> GetPrintVanningDetailSkuDtoListNew(Guid outboundSysId)
        {
            var query = from vd in Context.vanningdetails
                        join vpd in Context.vanningpickdetails on vd.SysId equals vpd.VanningDetailSysId
                        join pd in Context.pickdetails on vpd.PickDetailSysId equals pd.SysId
                        join od in Context.outbounddetails on pd.OutboundDetailSysId equals od.SysId
                        join sku in Context.skus on vpd.SkuSysId equals sku.SysId
                        join u in Context.uoms on vpd.UOMSysId equals u.SysId
                        where od.OutboundSysId == outboundSysId && vd.Status != (int)VanningStatus.Cancel
                        select new PrintVanningDetailSkuDto()
                        {
                            SysId = vd.SysId,
                            OtherId = sku.OtherId,
                            UPC = sku.UPC,
                            SkuName = sku.SkuName,
                            SkuDescr = sku.SkuDescr,
                            UOMCode = u.UOMCode,
                            Qty = vpd.Qty.Value,
                            Price = od.Price ?? 0,
                            IsGift = od.IsGift,
                            GiftQty = od.GiftQty,
                            ContainerNumber = vd.ContainerNumber
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrintVanningDetailStickDto GetPrintVanningDetailStick(Guid sysId)
        {
            var query = from vd in Context.vanningdetails
                        join v in Context.vannings on vd.VanningSysId equals v.SysId
                        join o in Context.outbounds on v.OutboundSysId equals o.SysId
                        join w in Context.warehouses on o.WareHouseSysId equals w.SysId
                        where vd.SysId == sysId
                        select new PrintVanningDetailStickDto()
                        {
                            VanningSysId = vd.VanningSysId,
                            ConsigneeName = o.ConsigneeName,
                            ConsigneePhone = o.ConsigneePhone,
                            ConsigneeAddress = (o.ConsigneeProvince ?? string.Empty) + (o.ConsigneeCity ?? string.Empty) + (o.ConsigneeArea ?? string.Empty) + (o.ConsigneeAddress ?? string.Empty),
                            ExternOrderId = o.ExternOrderId,
                            VanningOrderNumber = v.VanningOrder + "-" + vd.ContainerNumber,
                            Weight = vd.Weight,
                            Contacts = w.Contacts,
                            Telephone = w.Telephone,
                            Address = w.Address,
                            ContainerNumber = vd.ContainerNumber,
                            CarrierNumber = vd.CarrierNumber,
                            Marke = vd.Marke,
                            OutboundOrder = o.OutboundOrder
                        };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取箱贴数据ToB
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrintVanningDetailStickDto GetPrintVanningDetailStickToB(Guid sysId)
        {
            var query = from vd in Context.vanningdetails
                        join v in Context.vannings on vd.VanningSysId equals v.SysId
                        join o in Context.outbounds on v.OutboundSysId equals o.SysId
                        where vd.SysId == sysId
                        select new PrintVanningDetailStickDto()
                        {
                            VanningSysId = vd.VanningSysId,
                            ConsigneeName = o.ConsigneeName,
                            ConsigneePhone = o.ConsigneePhone,
                            ConsigneeAddress = (o.ConsigneeProvince ?? string.Empty) + (o.ConsigneeCity ?? string.Empty) + (o.ConsigneeArea ?? string.Empty) + (o.ConsigneeAddress ?? string.Empty),
                            ExternOrderId = o.ExternOrderId,
                            VanningOrderNumber = v.VanningOrder + "-" + vd.ContainerNumber,
                            Weight = vd.Weight,
                            ContainerNumber = vd.ContainerNumber
                        };

            var skuQuery = from vdp in Context.vanningpickdetails
                           join s in Context.skus on vdp.SkuSysId equals s.SysId
                           join u in Context.uoms on vdp.UOMSysId equals u.SysId
                           where vdp.VanningDetailSysId == sysId
                           group new { vdp, s, u } by new { vdp.SkuSysId, s.UPC, s.SkuName, u.UOMCode } into g
                           select new PrintVanningDetailStickSkuDto()
                           {
                               SkuSysId = g.Key.SkuSysId,
                               UPC = g.Key.UPC,
                               SkuName = g.Key.SkuName,
                               UOMCode = g.Key.UOMCode,
                               Qty = g.Sum(x => x.vdp.Qty)
                           };

            var printVanningDetailStick = query.FirstOrDefault();
            printVanningDetailStick.PrintVanningDetailStickSkuDto = skuQuery.ToList();
            printVanningDetailStick.SkuCount = printVanningDetailStick.PrintVanningDetailStickSkuDto.GroupBy(x => x.SkuSysId).Count();
            printVanningDetailStick.SkuQty = printVanningDetailStick.PrintVanningDetailStickSkuDto.Sum(x => x.Qty.Value);

            return printVanningDetailStick;
        }

        /// <summary>
        /// 批量打印拣货单
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        public List<PrintPickDetailDto> GetPrintPickDetailByBatchDtoList(string pickDetailOrder)
        {
            var query = (from pd in Context.pickdetails
                         join od in Context.outbounddetails on pd.OutboundDetailSysId equals od.SysId
                         join o in Context.outbounds on od.OutboundSysId equals o.SysId into t0
                         from outbound in t0.DefaultIfEmpty()
                         join s in Context.skus on pd.SkuSysId equals s.SysId
                         join u in Context.uoms on pd.UOMSysId equals u.SysId
                         where pd.PickDetailOrder == pickDetailOrder
                         select new
                         {
                             OutboundSysId = pd.OutboundSysId,
                             WarehouseSysId = pd.WareHouseSysId,
                             SkuSysId = pd.SkuSysId,
                             Qty = pd.Qty,
                             UPC = s.UPC,
                             SkuName = s.SkuName,
                             SkuDescr = s.SkuDescr,
                             Loc = pd.Loc,
                             Lot = pd.Lot,
                             Lpn = pd.Lpn,
                             UomName = u.UOMCode,
                             PackFactor = od.PackFactor,
                             Channel = outbound.Channel,
                             OutboundChildType = outbound.OutboundChildType
                         }).Distinct().OrderBy(p => new { p.Loc, p.UPC, p.Lot });


            return (from q in query
                    join inv in Context.invlots on q.SkuSysId equals inv.SkuSysId
                    where q.Lot == inv.Lot
                        && q.WarehouseSysId == inv.WareHouseSysId
                    select new PrintPickDetailDto
                    {
                        OutboundSysId = q.OutboundSysId,
                        SkuSysId = q.SkuSysId,
                        UPC = q.UPC,
                        SkuName = q.SkuName,
                        SkuDescr = q.SkuDescr,
                        Loc = q.Loc,
                        Lot = q.Lot,
                        Qty = q.Qty,
                        UomName = q.UomName,
                        Channel = q.Channel,
                        OutboundChildType = q.OutboundChildType,
                        PackFactor = q.PackFactor,
                        LotAttr01 = inv.LotAttr01,
                        LotAttr02 = inv.LotAttr02,
                        LotAttr03 = inv.LotAttr03,
                        LotAttr04 = inv.LotAttr04,
                        LotAttr05 = inv.LotAttr05,
                        LotAttr06 = inv.LotAttr06,
                        LotAttr07 = inv.LotAttr07,
                        LotAttr08 = inv.LotAttr08,
                        LotAttr09 = inv.LotAttr09
                    }).ToList();
        }

        public List<PrintPickDetailDto> GetPrintRecommendPickDetail(Guid outboundDetailSysId, Guid wareHouseSysId)
        {
            var query = (from outboundDetail in Context.outbounddetails
                         join invlotloclpn in Context.invlotloclpns on outboundDetail.SkuSysId equals invlotloclpn.SkuSysId
                         join sku in Context.skus on outboundDetail.SkuSysId equals sku.SysId
                         join u in Context.uoms on outboundDetail.UOMSysId equals u.SysId
                         where outboundDetail.SysId == outboundDetailSysId && invlotloclpn.WareHouseSysId == wareHouseSysId
                         select new PrintPickDetailDto()
                         {
                             OutboundSysId = outboundDetail.OutboundSysId,
                             SkuSysId = outboundDetail.SkuSysId,
                             Qty = (invlotloclpn.Qty - invlotloclpn.AllocatedQty - invlotloclpn.PickedQty - invlotloclpn.FrozenQty),
                             UPC = sku.UPC,
                             SkuName = sku.SkuName,
                             SkuDescr = sku.SkuDescr,
                             Loc = invlotloclpn.Loc,
                             Lot = invlotloclpn.Lot,
                             Lpn = invlotloclpn.Lpn,
                             UomName = u.UOMCode,
                             PackFactor = outboundDetail.PackFactor
                         }).Distinct().OrderByDescending(x => x.Qty);

            if (query.Count() > 3)
            {
                return query.Take(3).ToList();
            }

            return query.ToList();
        }


        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrintOutboundDto GetPrintOutboundDto(Guid sysId)
        {
            #region 注释主表信息查询方法
            //var skuQuery = from od in (from od in Context.outbounddetails
            //                           group new { od.OutboundSysId, od.SkuSysId } by new { od.OutboundSysId, od.SkuSysId } into g
            //                           select new { OutboundSysId = g.Key.OutboundSysId, SkuSysId = g.Key.SkuSysId })
            //               group new { od.OutboundSysId } by new { od.OutboundSysId } into g
            //               select new { OutboundSysId = g.Key.OutboundSysId, SkuCount = g.Count() };

            //var outboundDto = (from o in Context.outbounds
            //                   join sq in skuQuery on o.SysId equals sq.OutboundSysId
            //                   join od in Context.outbounddetails on o.SysId equals od.OutboundSysId
            //                   where o.OutboundOrder == outboundOrder
            //                   group new { o, sq, od } by new { o.SysId, o.ExternOrderId, o.ServiceStationName, o.OutboundType, o.ActualShipDate, o.ConsigneeName, o.ConsigneePhone, o.ConsigneeAddress, sq.SkuCount } into g
            //                   select new PrintOutboundDto()
            //                   {
            //                       SysId = g.Key.SysId,
            //                       ActualShipDate = g.Key.ActualShipDate,
            //                       ConsigneeName = g.Key.ConsigneeName,
            //                       ConsigneePhone = g.Key.ConsigneePhone,
            //                       ConsigneeAddress = g.Key.ConsigneeAddress,
            //                       SkuCount = g.Key.SkuCount,
            //                       SkuQty = g.Sum(x => x.od.ShippedQty),
            //                       ExternOrderId = g.Key.ExternOrderId,
            //                       ServiceStationName = g.Key.ServiceStationName,
            //                       OutboundType = g.Key.OutboundType.Value
            //                   }).FirstOrDefault();
            #endregion

            var outSql = new StringBuilder();
            outSql.Append(@"SELECT o.SysId,o.ActualShipDate,o.OutboundType,o.OutboundChildType,
                                  o.ExternOrderId,o.ConsigneeName,o.ConsigneeAddress,
                                  o.ConsigneePhone,o.ServiceStationName,o.AppointUserNames,
                                  (SELECT COUNT(1) AS A1
                                    FROM (SELECT DISTINCT
                                        o1.OutboundSysId,o1.SkuSysId
                                      FROM outbounddetail o1
                                      WHERE o1.OutboundSysId = @SysId AND o1.ShippedQty != 0) AS od
                                    GROUP BY od.OutboundSysId) AS SkuCount
                                FROM outbound o
                                WHERE o.SysId =@SysId ;");

            var outboundDto = base.Context.Database.SqlQuery<PrintOutboundDto>(outSql.ToString(),
                new MySqlParameter("@SysId", sysId)).AsQueryable().FirstOrDefault();

            if (outboundDto != null)
            {
                if (outboundDto.OutboundType == (int)OutboundType.TransferInventory)
                {
                    var transferInvInfo = (from ti in Context.transferinventorys
                                           where ti.TransferOutboundSysId == outboundDto.SysId
                                           select new
                                           {
                                               TransferInventoryOrder = ti.TransferInventoryOrder,
                                               FromWareHouseName = ti.FromWareHouseName,
                                               ToWareHouseName = ti.ToWareHouseName
                                           }).FirstOrDefault();
                    if (transferInvInfo != null)
                    {
                        outboundDto.TransferInventoryOrder = transferInvInfo.TransferInventoryOrder;
                        outboundDto.FromWareHouseName = transferInvInfo.FromWareHouseName;
                        outboundDto.ToWareHouseName = transferInvInfo.ToWareHouseName;
                    }
                }

                var query = from od in Context.outbounddetails
                            join s in Context.skus on od.SkuSysId equals s.SysId
                            where od.OutboundSysId == outboundDto.SysId
                            group new { od, s } by new { od, s } into g
                            select new PrintOutboundDetailDto()
                            {
                                SkuSysId = g.Key.s.SysId,
                                SkuCode = g.Key.s.SkuCode,
                                SkuName = g.Key.s.SkuName,
                                UPC = g.Key.s.UPC,
                                SkuDescr = g.Key.s.SkuDescr,
                                Qty = g.Sum(p => p.od.Qty),
                                ShippedQty = g.Sum(x => x.od.ShippedQty),
                                PackFactor = g.Key.od.PackFactor,
                                Memo = g.Key.od.Memo
                            };
                var list = query.ToList();
                var displayQuery = from a in list
                                   join s in Context.skus on a.SkuSysId equals s.SysId
                                   join p in Context.packs on s.PackSysId equals p.SysId into t2
                                   from p1 in t2.DefaultIfEmpty()
                                   join u in Context.uoms on p1.FieldUom01 equals u.SysId into t3
                                   from u1 in t3.DefaultIfEmpty()
                                   join u in Context.uoms on p1.FieldUom02 equals u.SysId into t4
                                   from u2 in t4.DefaultIfEmpty()
                                   select new PrintOutboundDetailDto()
                                   {
                                       SkuSysId = a.SkuSysId,
                                       SkuCode = a.SkuCode,
                                       SkuName = a.SkuName,
                                       UPC = a.UPC,
                                       SkuDescr = a.SkuDescr,
                                       UomCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                                ? u2.UOMCode : u1.UOMCode,
                                       Qty = a.Qty,
                                       DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (a.Qty.HasValue ? a.Qty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (a.Qty.HasValue ? a.Qty.Value : 0),
                                       ShippedQty = a.ShippedQty,
                                       DisplayShippedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (a.ShippedQty.HasValue ? a.ShippedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (a.ShippedQty.HasValue ? a.ShippedQty.Value : 0),
                                       PackFactor = a.PackFactor,
                                       Memo = a.Memo
                                   };

                outboundDto.PrintOutboundDetailDto = displayQuery.ToList();
            }

            return outboundDto;
        }

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrintOutboundPrePackDiffDto GetPrintOutboundPrePackDiffDto(Guid sysId)
        {
            #region 注释原有查询
            //var skuQuery = from od in (from od in Context.outbounddetails
            //                           group new { od.OutboundSysId, od.SkuSysId } by new { od.OutboundSysId, od.SkuSysId } into g
            //                           select new { OutboundSysId = g.Key.OutboundSysId, SkuSysId = g.Key.SkuSysId })
            //               group new { od.OutboundSysId } by new { od.OutboundSysId } into g
            //               select new { OutboundSysId = g.Key.OutboundSysId, SkuCount = g.Count() };

            //var outboundDto = (from o in Context.outbounds
            //                   join sq in skuQuery on o.SysId equals sq.OutboundSysId
            //                   join od in Context.outbounddetails on o.SysId equals od.OutboundSysId
            //                   where o.OutboundOrder == outboundOrder
            //                   group new { o, sq, od } by new { o.SysId, o.OutboundType, o.ActualShipDate, o.ConsigneeName, o.ConsigneePhone, o.ConsigneeAddress, sq.SkuCount, o.ServiceStationName } into g
            //                   select new PrintOutboundPrePackDiffDto()
            //                   {
            //                       SysId = g.Key.SysId,
            //                       ActualShipDate = g.Key.ActualShipDate,
            //                       ConsigneeName = g.Key.ConsigneeName,
            //                       ConsigneePhone = g.Key.ConsigneePhone,
            //                       ConsigneeAddress = g.Key.ConsigneeAddress,
            //                       SkuCount = g.Key.SkuCount,
            //                       SkuQty = g.Sum(x => x.od.ShippedQty),
            //                       OutboundType = g.Key.OutboundType.Value,
            //                       ServiceStationName = g.Key.ServiceStationName
            //                   }).FirstOrDefault();
            #endregion

            var outSql = new StringBuilder();
            outSql.Append(@"SELECT o.SysId,o.ActualShipDate,o.OutboundType,
                                  o.ExternOrderId,o.ConsigneeName,o.ConsigneeAddress,
                                  o.ConsigneePhone,o.ServiceStationName,
                                  (SELECT COUNT(1) AS A1
                                    FROM (SELECT DISTINCT
                                        o1.OutboundSysId,o1.SkuSysId
                                      FROM outbounddetail o1
                                      WHERE o1.OutboundSysId IN (SELECT SysId
                                        FROM outbound o2
                                        WHERE o2.SysId = @SysId )) AS od
                                    GROUP BY od.OutboundSysId) AS SkuCount
                                FROM outbound o
                                WHERE o.SysId = @SysId;");

            var outboundDto = base.Context.Database.SqlQuery<PrintOutboundPrePackDiffDto>(outSql.ToString(),
                new MySqlParameter("@SysId", sysId)).AsQueryable().FirstOrDefault();



            if (outboundDto != null)
            {
                if (outboundDto.OutboundType == (int)OutboundType.TransferInventory)
                {
                    var transferInvInfo = (from ti in Context.transferinventorys
                                           where ti.TransferOutboundSysId == outboundDto.SysId
                                           select new
                                           {
                                               TransferInventoryOrder = ti.TransferInventoryOrder,
                                               FromWareHouseName = ti.FromWareHouseName,
                                               ToWareHouseName = ti.ToWareHouseName
                                           }).FirstOrDefault();
                    if (transferInvInfo != null)
                    {
                        outboundDto.TransferInventoryOrder = transferInvInfo.TransferInventoryOrder;
                        outboundDto.FromWareHouseName = transferInvInfo.FromWareHouseName;
                        outboundDto.ToWareHouseName = transferInvInfo.ToWareHouseName;
                    }
                }
            }

            return outboundDto;
        }

        /// <summary>
        /// 获取打印出库单对应的交接单列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<PrintTMSPackNumberListDto> GetPrintTMSPackNumberList(Guid sysId)
        {
            var query = (from ot in Context.outboundtransferorder
                         join p in Context.prebulkpack on ot.PreBulkPackSysId equals p.SysId into p1
                         from t in p1.DefaultIfEmpty()
                         where ot.OutboundSysId == sysId
                         select new PrintTMSPackNumberListDto
                         {
                             SysId = ot.SysId,
                             OutboundOrder = ot.OutboundOrder,
                             OutboundSysId = ot.OutboundSysId,
                             BoxNumber = ot.BoxNumber,
                             Qty = ot.Qty,
                             SkuQty = ot.SkuQty,
                             CreateUserName = ot.CreateUserName,
                             ConsigneeArea = ot.ConsigneeArea,
                             ServiceStationName = ot.ServiceStationName,
                             PreBulkPackSysId = ot.PreBulkPackSysId,
                             PreBulkPackOrder = ot.PreBulkPackOrder,
                             StorageCase = t.StorageCase
                         }).AsQueryable().OrderBy(x => x.BoxNumber);

            return query.ToList();

        }

        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblyDetailSysId"></param>
        /// <returns></returns>
        public List<PrintAssemblyPickDetailDto> GetPrintAssemblyRCMDPickDetail(Guid assemblyDetailSysId, Guid wareHouseSysId)
        {
            var query = (from ad in Context.assemblydetails
                         join il in Context.invlotloclpns on ad.SkuSysId equals il.SkuSysId
                         join s in Context.skus on ad.SkuSysId equals s.SysId
                         join p in Context.packs on s.PackSysId equals p.SysId
                         join u in Context.uoms on p.FieldUom02 equals u.SysId
                         where ad.SysId == assemblyDetailSysId && (il.Qty - il.AllocatedQty - il.PickedQty) != 0 && il.WareHouseSysId == wareHouseSysId
                         select new PrintAssemblyPickDetailDto
                         {
                             SkuSysId = ad.SkuSysId,
                             UPC = s.UPC,
                             SkuName = s.SkuName,
                             SkuDescr = s.SkuDescr,
                             Loc = il.Loc,
                             Lot = il.Lot,
                             Lpn = il.Lpn,
                             Qty = il.Qty - il.AllocatedQty - il.PickedQty - il.FrozenQty,
                             UnitQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                    && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                    ? Math.Round(((p.FieldValue02.Value * (il.Qty - il.AllocatedQty - il.PickedQty - il.FrozenQty) * 1.00m) / p.FieldValue01.Value), 3) : (il.Qty - il.AllocatedQty - il.PickedQty - il.FrozenQty),
                             UomName = u.UOMCode
                         }).Distinct().OrderByDescending(p => p.Qty);
            return query.Take(3).ToList();
        }


        /// <summary>
        /// 重载：生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblyDetailSysId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="assemblyrule"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public List<PrintAssemblyPickDetailDto> GetPrintAssemblyRCMDPickDetail(Guid assemblyDetailSysId, Guid wareHouseSysId, assemblyrule assemblyrule, string channel)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT DISTINCT  Filter1.SkuSysId, Filter1.SkuName,Filter1.UPC,Filter1.SkuDescr,
                      Filter1.Loc,Filter1.Lot,Filter1.Lpn,u.UOMCode AS UomName,
                      IFNULL((Filter1.QTY1 - Filter1.ALLOCATEDQTY1 - Filter1.PICKEDQTY1 - Filter1.FrozenQty),0) AS Qty ,
                      CASE WHEN ((((Filter1.InLabelUnit01 IS NOT NULL) AND
                          ((1 = Filter1.InLabelUnit01) AND
                          (Filter1.InLabelUnit01 IS NOT NULL))) AND
                          (Filter1.FieldValue01 > 0)) AND
                          (Filter1.FieldValue02 > 0)) THEN (ROUND(((Filter1.FieldValue02 * (((Filter1.QTY1 - Filter1.ALLOCATEDQTY1) - Filter1.PICKEDQTY1) - Filter1.FrozenQty)) * 1.00) / (Filter1.FieldValue01), 3)) ELSE (((Filter1.QTY1 - Filter1.ALLOCATEDQTY1) - Filter1.PICKEDQTY1) - Filter1.FrozenQty) END AS UnitQty
                        FROM (SELECT    ab.SysId, ab.SkuSysId,ilp.WareHouseSysId,ilp.Loc,ilp.Lot,ilp.Lpn,ilp.Qty AS QTY1,
                            ilp.AllocatedQty AS ALLOCATEDQTY1,ilp.PickedQty AS PICKEDQTY1,ilp.FrozenQty,
                            s.SkuCode,s.SkuName,s.SkuDescr,s.UPC,p.FieldValue01,p.InLabelUnit01,
                            p.FieldUom02,p.FieldValue02,ilot.ProduceDate,ilot.ReceiptDate,ilot.LotAttr01
                          FROM assemblydetail AS ab
                            LEFT JOIN invlotloclpn AS ilp  ON ab.SkuSysId = ilp.SkuSysId
                            LEFT JOIN invlot ilot  ON ilp.SkuSysId = ilot.SkuSysId  AND ilp.lot = ilot.lot  AND ilp.WareHouseSysId = ilot.WareHouseSysId
                            LEFT JOIN sku AS s   ON ab.SkuSysId = s.SysId
                            LEFT JOIN pack AS p  ON s.PackSysId = p.SysId
                          WHERE NOT ((0 = ((ilp.Qty - ilp.AllocatedQty) - ilp.PickedQty))
                          AND ((ilp.Qty - ilp.AllocatedQty) - ilp.PickedQty IS NOT NULL))) AS Filter1
                          LEFT JOIN uom AS u  ON Filter1.FieldUom02 = u.SysId
                        WHERE Filter1.SysId =@assemblyDetailSysId AND Filter1.WareHouseSysId =@WareHouseSysId @WhereSql LIMIT 3;");

            List<MySqlParameter> paraList = new List<MySqlParameter>();
            paraList.Add(new MySqlParameter($"@assemblyDetailSysId", assemblyDetailSysId));
            paraList.Add(new MySqlParameter($"@WareHouseSysId", wareHouseSysId));

            var whereSql = new StringBuilder();

            #region  加工领料增加规则
            if (assemblyrule != null)
            {
                //开启规则
                if (assemblyrule.Status)
                {
                    if (assemblyrule.MatchingLotAttr)
                    {
                        whereSql.AppendFormat(" And ifnull(Filter1.LotAttr01,'') =@LotAttr01 ");

                        paraList.Add(new MySqlParameter($"@LotAttr01", string.IsNullOrEmpty(channel) ? "" : channel));
                    }

                    //先生产先分配
                    if (assemblyrule.DeliverySortRules == (int)DeliveryAssemblyRule.FirstProduceFirstAssembly)
                    {
                        whereSql.Append(" ORDER BY Filter1.ProduceDate ASC");
                    }
                    //后生产先分配
                    else if (assemblyrule.DeliverySortRules == (int)DeliveryAssemblyRule.AfterProduceFirstAssembly)
                    {
                        whereSql.Append(" ORDER BY Filter1.ProduceDate DESC");
                    }
                    //先入库先分配
                    else if (assemblyrule.DeliverySortRules == (int)DeliveryAssemblyRule.FirstReceiptFirstAssembly)
                    {
                        whereSql.Append(" ORDER BY Filter1.ReceiptDate ASC");
                    }
                    else
                    {
                        whereSql.Append(" ORDER BY  Filter1.QTY1 DESC");
                    }
                }
                else
                {
                    whereSql.Append(" ORDER BY  Filter1.QTY1 DESC");
                }
            }
            else
            {
                whereSql.Append(" ORDER BY  Filter1.QTY1 DESC");
            }
            #endregion

            return base.Context.Database.SqlQuery<PrintAssemblyPickDetailDto>(strSql.ToString().Replace("@WhereSql", whereSql.ToString()), paraList.ToArray()).ToList();

        }

        /// <summary>
        /// 打印上架单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<PrintReceiptDetailDto> GetPrintReceiptAutoShelvesDetail(Guid receiptSysId)
        {
            const string sql =
                "SELECT  i.SkuSysId,s.SkuCode,s.SkuName,s.SkuDescr,s.UPC,i.ToLoc,i.Lot, i.PackCode,i.UOMCode,Sum(i.Qty) AS ShelveQty FROM invtrans i "
                + "LEFT JOIN sku s ON s.SysId = i.SkuSysId WHERE i.TransType = 'IN' AND i.SourceTransType = 'Shelve'  AND i.DocSysId=@DocSysId and i.Status='OK' and i.Loc != @Loc  group by i.SkuSysId,s.SkuCode,s.SkuName,s.SkuDescr,s.UPC,i.ToLoc,i.Lot,i.PackCode,i.UOMCode ";
            var printReceiptDetailDtoList = base.Context.Database.SqlQuery<PrintReceiptDetailDto>(sql,
                new MySqlParameter("@DocSysId", receiptSysId),
                new MySqlParameter("@Loc", PublicConst.PickingSkuLoc)).ToList();

            var list = from a in printReceiptDetailDtoList
                       join s in Context.skus on a.SkuSysId equals s.SysId
                       join p in Context.packs on s.PackSysId equals p.SysId
                       select new PrintReceiptDetailDto
                       {
                           SkuSysId = a.SkuSysId,
                           SkuCode = a.SkuCode,
                           SkuName = a.SkuName,
                           SkuDescr = a.SkuDescr,
                           UPC = a.UPC,
                           ToLoc = a.ToLoc,
                           Lot = a.Lot,
                           UOMCode = a.UOMCode,
                           ShelveQty = a.ShelveQty,
                           DisplayShelveQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                               && p.FieldValue01 > 0 && p.FieldValue02 > 0
                               ? Math.Round(((p.FieldValue02.Value * a.ShelveQty * 1.00m) / p.FieldValue01.Value), 3) : a.ShelveQty,
                       };

            return list.ToList();
        }

        /// <summary>
        /// 打印盘点单明细
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <returns></returns>
        public List<PrintStockTakeDetailDto> GetPrintStockTakeDetails(Guid stockTakeSysId)
        {
            var query = from sd in Context.stocktakedetails
                        join s in Context.stocktakes on sd.StockTakeSysId equals s.SysId
                        join sk in Context.skus on sd.SkuSysId equals sk.SysId
                        join p in Context.packs on sk.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on ti1.FieldUom01 equals u.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on ti1.FieldUom02 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where s.SysId == stockTakeSysId
                        select new { sd, s, sk, ti1, UOMCode1 = ti2.UOMCode, UOMCode2 = ti3.UOMCode };
            return query.Select(p => new PrintStockTakeDetailDto
            {
                SkuCode = p.sk.SkuCode,
                SkuUPC = p.sk.UPC,
                SkuName = p.sk.SkuName,
                SkuDescr = p.sk.SkuDescr,
                UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                StockTakeQty = p.sd.StockTakeQty,
                DisplayStockTakeQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * p.sd.StockTakeQty * 1.00m) / p.ti1.FieldValue01.Value), 3) : p.sd.StockTakeQty,
                ReplayQty = p.sd.ReplayQty,
                DisplayReplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * (int)p.sd.ReplayQty * 1.00m) / p.ti1.FieldValue01.Value), 3) : (int)p.sd.ReplayQty,
                CreateDate = p.sd.CreateDate,
                Loc = p.sd.Loc
            }).OrderByDescending(p => p.CreateDate).ToList();
        }

        /// <summary>
        /// 打印盘点汇总报告明细
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <returns></returns>
        public List<PrintStockTakeReportDetailDto> GetPrintStockTakeReportDetails(List<Guid> sysIds)
        {
            var query = from sd in Context.stocktakedetails
                        join sk in Context.skus on sd.SkuSysId equals sk.SysId
                        join p in Context.packs on sk.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on ti1.FieldUom01 equals u.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where sysIds.Contains(sd.StockTakeSysId)
                        select new { sd, sk, ti1, UOMCode1 = ti2.UOMCode, UOMCode2 = ti3.UOMCode };
            return query.Select(p => new PrintStockTakeReportDetailDto
            {
                SkuCode = p.sk.SkuCode,
                SkuUPC = p.sk.UPC,
                SkuName = p.sk.SkuName,
                SkuDescr = p.sk.SkuDescr,
                UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                Status = p.sd.Status,
                Qty = p.sd.Qty,
                DisplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * p.sd.Qty * 1.00m) / p.ti1.FieldValue01.Value), 3) : p.sd.Qty,
                StockTakeQty = p.sd.StockTakeQty,
                DisplayStockTakeQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * p.sd.StockTakeQty * 1.00m) / p.ti1.FieldValue01.Value), 3) : p.sd.StockTakeQty,
                ReplayQty = p.sd.ReplayQty,
                DisplayReplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * (int)p.sd.ReplayQty * 1.00m) / p.ti1.FieldValue01.Value), 3) : (int)p.sd.ReplayQty,
                CreateDate = p.sd.CreateDate
            }).OrderByDescending(p => p.CreateDate).ToList();
        }

        /// <summary>
        /// 打印质检单明细
        /// </summary>
        /// <param name="qcSysId"></param>
        /// <returns></returns>
        public List<PrintQualityControlDetailDto> GetPrintQualityControlDetails(Guid qcSysId)
        {
            var query = from qd in Context.qualitycontroldetail
                        join s in Context.skus on qd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u1 in Context.uoms on ti1.FieldUom01 equals u1.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where qd.QualityControlSysId == qcSysId
                        select new { qd, s, ti1, UOMCode1 = ti2.UOMCode, UOMCode2 = ti3.UOMCode };
            return query.Select(p => new PrintQualityControlDetailDto
            {
                SkuName = p.s.SkuName,
                UPC = p.s.UPC,
                DisplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? Math.Round(((p.ti1.FieldValue02.Value * (p.qd.Qty.HasValue ? p.qd.Qty.Value : 0) * 1.00m) / p.ti1.FieldValue01.Value), 3) : (p.qd.Qty.HasValue ? p.qd.Qty.Value : 0),
                UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                               && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                               ? p.UOMCode2 : p.UOMCode1,
                Descr = p.qd.Descr
            }).OrderByDescending(p => p.UPC).ToList();
        }

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public PrintPrebulkPackByBatchDto GetPrintPrebulkPackByBatchDto(List<Guid> sysIds)
        {
            PrintPrebulkPackByBatchDto rsp = new PrintPrebulkPackByBatchDto();
            rsp.PrintPreBulkPackDtoList = (from p in Context.prebulkpack
                                           where sysIds.Contains(p.SysId)
                                           select new PreBulkPackDto { SysId = p.SysId, StorageCase = p.StorageCase, CreateDate = p.CreateDate, CreateUserName = p.CreateUserName }).OrderByDescending(p => p.StorageCase).ToList();
            if (rsp.PrintPreBulkPackDtoList.Count != 0)
            {
                var details = (from pd in Context.prebulkpackdetail
                               join s in Context.skus on pd.SkuSysId equals s.SysId
                               join p in Context.packs on s.PackSysId equals p.SysId into t1
                               from ti1 in t1.DefaultIfEmpty()
                               join u1 in Context.uoms on ti1.FieldUom01 equals u1.SysId into t2
                               from ti2 in t2.DefaultIfEmpty()
                               join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                               from ti3 in t3.DefaultIfEmpty()
                               where sysIds.Contains(pd.PreBulkPackSysId)
                               select new PreBulkPackDetailDto()
                               {
                                   SysId = pd.SysId,
                                   PreBulkPackSysId = pd.PreBulkPackSysId,
                                   SkuSysId = pd.SkuSysId,
                                   SkuCode = s.SkuCode,
                                   SkuName = s.SkuName,
                                   UPC = s.UPC,
                                   Qty = pd.Qty,
                                   Loc = pd.Loc,
                                   UomCode = ti1.InLabelUnit01.HasValue && ti1.InLabelUnit01.Value == true && ti1.FieldValue01 > 0 && ti1.FieldValue02 > 0 ? ti3.UOMCode : ti2.UOMCode,
                                   PackCode = ti1.PackCode
                               }).ToList();
                foreach (var printPreBulkPackDto in rsp.PrintPreBulkPackDtoList)
                {
                    printPreBulkPackDto.PreBulkPackDetailList = details.Where(p => p.PreBulkPackSysId == printPreBulkPackDto.SysId).ToList();
                }
            }
            return rsp;
        }

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <summary>
        public List<PrintPickingMaterialDetailDto> GetPrintPickingMaterialDetailList(PrintPickingMaterialQuery request)
        {
            var strSql = new StringBuilder(@"select pr.ReceiptSysId,pr.ReceiptOrder,pr.SkuSysId,s.SkuCode,s.SkuName,
                               s.UPC,s.SkuDescr,pr.Qty,pr.PickingDate,pr.PickingUserName,
                              CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                              THEN ROUND(p.FieldValue02 * (IFNULL(pr.Qty,0)/p.FieldValue01),3)
                               ELSE IFNULL(pr.Qty,0) END AS DisplayQty
                              from pickingrecords pr
                            LEFT JOIN sku s ON s.SysId = pr.SkuSysId
                            LEFT JOIN pack p ON s.PackSysId = p.SysId
                            where 1=1");

            var paraList = new List<MySqlParameter>();
            if (request.SysIds != null && request.SysIds.Count > 0)
            {
                strSql.AppendFormat(" and pr.SysId in (");

                var strSysIds = string.Empty;
                var i = 0;
                foreach (var info in request.SysIds)
                {
                    strSql.Append($"@strSysIds{i},");
                    paraList.Add(new MySqlParameter($"@strSysIds{i}", info));
                    //strSysIds += "'" + info + "',";
                    i++;
                }
                strSql = new StringBuilder(strSql.ToString().TrimEnd(','));
                strSql.Append(" )");


                //strSysIds = !string.IsNullOrEmpty(strSysIds) ? strSysIds.Substring(0, strSysIds.Length - 1) : strSysIds;
                //{ 0})", strSysIds);
            }
            else
            {
                strSql.Append(@" and pr.ReceiptSysId =@ReceiptSysId  
                                        AND pr.PickingUserName =@PickingUserName 
                                        AND pr.pickingNumber IN (SELECT MAX(PickingNumber) FROM pickingrecords 
                                        WHERE ReceiptSysId =@ReceiptSysId AND PickingUserName =@PickingUserName )");


                paraList.Add(new MySqlParameter($"@ReceiptSysId", request.ReceiptSysId));
                paraList.Add(new MySqlParameter($"@PickingUserName", request.PickingUserName.Trim()));
            }

            var printPickingMaterialDetailList = base.Context.Database.SqlQuery<PrintPickingMaterialDetailDto>(strSql.ToString(), paraList.ToArray()).ToList();
            return printPickingMaterialDetailList;
        }
    }
}