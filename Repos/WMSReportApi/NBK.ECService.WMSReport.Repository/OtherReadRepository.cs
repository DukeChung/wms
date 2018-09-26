using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using NBK.ECService.WMSReport.Model.Models;

namespace NBK.ECService.WMSReport.Repository
{
    public class OtherReadRepository : CrudRepository, IOtherReadRepository
    {
        /// <param name="dbContextProvider"></param>
        public OtherReadRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        #region 出库

        public Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request)
        {

            var query = from x in Context.outbounds
                        where x.WareHouseSysId == request.WarehouseSysId
                        select new OutboundListDto()
                        {
                            SysId = x.SysId,
                            OutboundOrder = x.OutboundOrder,
                            Status = x.Status.Value,
                            OutboundType = x.OutboundType.Value,
                            OutboundChildType = x.OutboundChildType,
                            CreateDate = x.CreateDate,
                            ExternOrderId = x.ExternOrderId,
                            ExternOrderDate = x.ExternOrderDate.Value,
                            AuditingDate = x.AuditingDate,
                            ActualShipDate = x.ActualShipDate,
                            ConsigneeName = x.ConsigneeName,
                            ConsigneeArea = x.ConsigneeArea,
                            ConsigneeProvince = x.ConsigneeProvince,
                            ConsigneeCity = x.ConsigneeCity,
                            ConsigneeAddress = x.ConsigneeAddress,
                            TotalQty = x.TotalQty.Value,
                            Remark = x.Remark,
                            ServiceStationName = x.ServiceStationName,
                            ServiceStationCode = x.ServiceStationCode,
                            ConsigneePhone = x.ConsigneePhone,
                            ConsigneeTown = x.ConsigneeTown,
                            ConsigneeVillage = x.ConsigneeVillage,
                            PlatformOrder = x.PlatformOrder,
                            PurchaseOrder = x.PurchaseOrder,
                            IsReturn = x.IsReturn,
                            SortNumber = x.SortNumber,
                            TMSOrder = x.TMSOrder,
                            DepartureDate = x.DepartureDate,
                            AppointUserNames = x.AppointUserNames,
                            Exception = x.Exception,
                            IsInvoice = x.IsInvoice,
                            Channel = x.Channel,
                        };


            #region 查询条件


            if (!string.IsNullOrEmpty(request.ExternOrderId))
            {
                query = query.Where(x => x.ExternOrderId.Contains(request.ExternOrderId));
            }
            if (!string.IsNullOrEmpty(request.OutboundOrder))
            {
                request.OutboundOrder = request.OutboundOrder.Trim();
                query = query.Where(x => x.OutboundOrder.Contains(request.OutboundOrder));
            }
            if (!string.IsNullOrEmpty(request.ConsigneeName))
            {
                request.ConsigneeName = request.ConsigneeName.Trim();
                query = query.Where(x => x.ConsigneeName.Contains(request.ConsigneeName));
            }
            if (!string.IsNullOrEmpty(request.ConsigneePhone))
            {
                request.ConsigneePhone = request.ConsigneePhone.Trim();
                query = query.Where(x => x.ConsigneePhone.Contains(request.ConsigneePhone));
            }
            if (!string.IsNullOrEmpty(request.ConsigneeAddress))
            {
                request.ConsigneeAddress = request.ConsigneeAddress.Trim();
                query =
                    query.Where(
                        x =>
                            ((x.ConsigneeProvince ?? "") + (x.ConsigneeCity ?? "") + (x.ConsigneeArea ?? "") + (x.ConsigneeAddress ?? ""))
                                .Contains(request.ConsigneeAddress));
            }
            if (request.CreateDateFrom.HasValue)
            {
                query = query.Where(x => x.CreateDate > request.CreateDateFrom.Value);
            }
            if (request.CreateDateTo.HasValue)
            {
                query = query.Where(x => x.CreateDate < request.CreateDateTo.Value);
            }
            if (request.ActualShipDateFrom.HasValue)
            {
                query = query.Where(x => x.ActualShipDate.Value > request.ActualShipDateFrom.Value);
            }
            if (request.ActualShipDateTo.HasValue)
            {
                query = query.Where(x => x.ActualShipDate.Value < request.ActualShipDateTo.Value);
            }
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }
            if (request.AuditingDateFrom.HasValue)
            {
                query = query.Where(x => x.AuditingDate.Value > request.AuditingDateFrom.Value);
            }
            if (request.AuditingDateTo.HasValue)
            {
                query = query.Where(x => x.AuditingDate.Value < request.AuditingDateTo.Value);
            }
            if (request.OutboundType.HasValue)
            {
                query = query.Where(x => x.OutboundType == request.OutboundType.Value);
            }
            if (request.IsReturn.HasValue)
            {
                if (request.IsReturn == true)
                {
                    query = query.Where(x => x.IsReturn > 0);
                }
                else
                {
                    query = query.Where(x => x.IsReturn == null);

                }
            }
            if (!string.IsNullOrEmpty(request.SkuName) || !string.IsNullOrEmpty(request.UPC) || !string.IsNullOrEmpty(request.SkuCode) || request.IsMaterial.HasValue)
            {
                var skuQuery = from obDetail in Context.outbounddetails
                               join s in Context.skus on obDetail.SkuSysId equals s.SysId
                               select new
                               {
                                   obDetail,
                                   s
                               };
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    request.SkuName = request.SkuName.Trim();
                    skuQuery = skuQuery.Where(x => x.s.SkuName.Contains(request.SkuName));
                }
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    request.UPC = request.UPC.Trim();
                    skuQuery = skuQuery.Where(x => x.s.UPC.Contains(request.UPC));
                }
                if (!string.IsNullOrEmpty(request.SkuCode))
                {
                    request.SkuCode = request.SkuCode.Trim();
                    skuQuery = skuQuery.Where(x => x.s.SkuCode.Contains(request.SkuCode));
                }

                if (request.IsMaterial.HasValue)
                {
                    if (request.IsMaterial.Value)
                    {
                        skuQuery = skuQuery.Where(x => x.s.IsMaterial == request.IsMaterial.Value);
                    }
                    else
                    {
                        //不是原材料
                        skuQuery = skuQuery.Where(x => x.s.IsMaterial != (!request.IsMaterial.Value));
                    }
                }
                var outboundSysIds = skuQuery.Select(x => x.obDetail.OutboundSysId).Distinct().ToList();
                query = query.Where(x => outboundSysIds.Contains(x.SysId));
            }

            if (!string.IsNullOrEmpty(request.ToWareHouseSysId))
            {
                var toWareHouseSysId = Guid.Parse(request.ToWareHouseSysId);
                var tfQuery = from tf in Context.transferinventorys
                              join o in Context.outbounds on tf.TransferOutboundSysId equals o.SysId
                              where tf.ToWareHouseSysId == toWareHouseSysId
                              select o.SysId;
                var outboundSysIds = tfQuery.Distinct().ToList();

                query = query.Where(x => outboundSysIds.Contains(x.SysId));
            }
            if (!string.IsNullOrEmpty(request.ServiceStationCode))
            {
                request.ServiceStationCode = request.ServiceStationCode.Trim();
                query = query.Where(p => p.ServiceStationCode.Contains(request.ServiceStationCode));
            }
            if (!string.IsNullOrEmpty(request.ServiceStationName))
            {
                request.ServiceStationName = request.ServiceStationName.Trim();
                query = query.Where(p => p.ServiceStationName.Contains(request.ServiceStationName));
            }
            if (!string.IsNullOrEmpty(request.OutboundChildType))
            {
                request.OutboundChildType = request.OutboundChildType.Trim();
                query = query.Where(p => p.OutboundChildType.Contains(request.OutboundChildType));
            }
            if (!string.IsNullOrEmpty(request.PurchaseOrder))
            {
                request.PurchaseOrder = request.PurchaseOrder.Trim();
                query = query.Where(p => p.PurchaseOrder == request.PurchaseOrder);
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                var regionList = request.Region.Split(',');
                for (int i = 0; i < regionList.Length; i++)
                {
                    var region = regionList[i];
                    if (i == 0) query = query.Where(p => p.ConsigneeProvince.Contains(region));
                    if (i == 1) query = query.Where(p => p.ConsigneeCity.Contains(region));
                    if (i == 2) query = query.Where(p => p.ConsigneeArea.Contains(region));
                    if (i == 3) query = query.Where(p => p.ConsigneeTown.Contains(region));
                    if (i == 4) query = query.Where(p => p.ConsigneeVillage.Contains(region));
                }
            }

            if (!string.IsNullOrEmpty(request.PlatformOrder))
            {
                request.PlatformOrder = request.PlatformOrder.Trim();
                query = query.Where(p => p.PlatformOrder == request.PlatformOrder);
            }
            if (!string.IsNullOrEmpty(request.TMSOrder))
            {
                request.TMSOrder = request.TMSOrder.Trim();
                query = query.Where(p => p.TMSOrder == request.TMSOrder);
            }

            if (request.IsFertilizer)
            {
                query = query.Where(x => x.OutboundType == ((int)OutboundType.Fertilizer));
            }
            else
            {
                query = query.Where(x => x.OutboundType != ((int)OutboundType.Fertilizer));
            }
            if (request.DepartureDateFrom.HasValue)
            {
                query = query.Where(x => x.DepartureDate.Value >= request.DepartureDateFrom.Value);
            }
            if (request.DepartureDateTo.HasValue)
            {
                query = query.Where(x => x.DepartureDate.Value <= request.DepartureDateTo.Value);
            }

            if (!string.IsNullOrEmpty(request.Channel))
            {
                request.Channel = request.Channel.Trim();
                query = query.Where(p => p.Channel.Contains(request.Channel));
            }


            #endregion 查询条件

            request.iTotalDisplayRecords = query.Count();
            query =
                query.OrderByDescending(p => p.AuditingDate).ThenBy(x => x.DepartureDate).ThenBy(k => k.SortNumber).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages<OutboundListDto>(query, request);
        }

        public List<OutboundListDto> GetOutboundDetailBySummary(List<Guid> SysIds, Guid warehouseSysId)
        {

            var query = from od in (
                from obDetail in Context.outbounddetails
                join s in Context.skus on obDetail.SkuSysId equals s.SysId
                join p in Context.packs on s.PackSysId equals p.SysId into t2
                from p1 in t2.DefaultIfEmpty()
                where SysIds.Contains(obDetail.OutboundSysId.Value)
                select new
                {
                    OutboundSysId = obDetail.OutboundSysId,
                    DisplayTotalQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                      && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                        ? Math.Round(
                            ((p1.FieldValue02.Value * (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0) * 1.00m) /
                             p1.FieldValue01.Value), 3)
                        : (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0)
                })
                        group new { od } by new { od.OutboundSysId }
                into g
                        select new OutboundListDto
                        {
                            SysId = (Guid)g.Key.OutboundSysId,
                            DisplayTotalQty = g.Sum(x => x.od.DisplayTotalQty)
                        };

            return query.ToList();
        }

        public OutboundViewDto GetOutboundBySysId(Guid outboundSysId, Guid wareHouseSysId)
        {
            var query = from ob in Context.outbounds
                        join pre in Context.prepacks on ob.SysId equals pre.OutboundSysId into t0
                        from p0 in t0.DefaultIfEmpty()
                        join tf in Context.transferinventorys on ob.SysId equals tf.TransferOutboundSysId into t1
                        from tfinfo in t1.DefaultIfEmpty()
                        where ob.SysId == outboundSysId
                        select new OutboundViewDto()
                        {
                            SysId = ob.SysId,
                            OutboundOrder = ob.OutboundOrder,
                            ExternOrderId = ob.ExternOrderId,
                            Status = ob.Status.Value,
                            OutboundType = ob.OutboundType.Value,
                            OutboundDate = ob.OutboundDate.Value,
                            AuditingDate = ob.AuditingDate,
                            TotalQty = ob.TotalQty.Value,
                            ConsigneeName = ob.ConsigneeName,
                            ConsigneeAddress = ob.ConsigneeAddress,
                            DetailedAddress =
                                (ob.ConsigneeProvince ?? string.Empty) + (ob.ConsigneeCity ?? string.Empty) +
                                (ob.ConsigneeArea ?? string.Empty) + (ob.ConsigneeAddress ?? string.Empty),
                            Remark = ob.Remark,
                            ServiceStationName = ob.ServiceStationName,
                            PrePackOrder = p0.PrePackOrder,
                            FromWareHouseName = tfinfo.FromWareHouseName,
                            ToWareHouseName = tfinfo.ToWareHouseName,
                            TransferInventoryOrder = tfinfo.TransferInventoryOrder,
                            ReceiptSysId = ob.ReceiptSysId,
                            PurchaseOrder = ob.PurchaseOrder,
                            SortNumber = ob.SortNumber,
                            TMSOrder = ob.TMSOrder,
                            DepartureDate = ob.DepartureDate,
                            AppointUserNames = ob.AppointUserNames,
                            Channel = ob.Channel
                        };

            return query.FirstOrDefault();
        }

        public List<OutboundDetailViewDto> GetOutboundDetails(Guid outboundSysId, Guid wareHouseSysId)
        {
            var query =
                from obDetail in Context.outbounddetails
                join s in Context.skus on obDetail.SkuSysId equals s.SysId
                join u in Context.uoms on obDetail.UOMSysId equals u.SysId
                join p in Context.packs on obDetail.PackSysId equals p.SysId into t2
                from p1 in t2.DefaultIfEmpty()
                join u1 in Context.uoms on p1.FieldUom02 equals u1.SysId into t3
                from u2 in t3.DefaultIfEmpty()
                where obDetail.OutboundSysId == outboundSysId
                select new OutboundDetailViewDto()
                {
                    SysId = obDetail.SysId,
                    OutboundSysId = obDetail.OutboundSysId.Value,
                    SkuSpecialTypes = s.SpecialTypes,
                    UPC = s.UPC,
                    SkuName = s.SkuName,
                    SkuDescr = s.SkuDescr,
                    Qty = obDetail.Qty.Value,
                    PackFactor = obDetail.PackFactor,
                    DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                 && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                        ? Math.Round(
                            ((p1.FieldValue02.Value * (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0) * 1.00m) /
                             p1.FieldValue01.Value), 3)
                        : (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0),
                    UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                              && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                        ? u2.UOMCode
                        : u.UOMCode,
                    SkuSysId = s.SysId,
                    ReturnQty = obDetail.ReturnQty,
                    DisplayReturnQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                 && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                            ? Math.Round(
                                ((p1.FieldValue02.Value * (obDetail.ReturnQty) * 1.00m) /
                                 p1.FieldValue01.Value), 3)
                            : obDetail.ReturnQty,
                    ShippedQty = obDetail.ShippedQty ?? 0,
                    DisplayShippedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                        ? Math.Round(
                            ((p1.FieldValue02.Value * (obDetail.ShippedQty.HasValue ? obDetail.ShippedQty.Value : 0) * 1.00m) /
                             p1.FieldValue01.Value), 3)
                        : (obDetail.ShippedQty.HasValue ? obDetail.ShippedQty.Value : 0),
                    Memo = obDetail.Memo
                };

            return query.ToList();
        }

        /// <summary>
        /// 分页获取出库单明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundExceptionDto> GetOutboundDetailList(OutboundExceptionQueryDto request)
        {
            var query =
              from obDetail in Context.outbounddetails
              join s in Context.skus on obDetail.SkuSysId equals s.SysId
              join u in Context.uoms on obDetail.UOMSysId equals u.SysId
              join p in Context.packs on obDetail.PackSysId equals p.SysId into t2
              from p1 in t2.DefaultIfEmpty()
              join u1 in Context.uoms on p1.FieldUom02 equals u1.SysId into t3
              from u2 in t3.DefaultIfEmpty()
              where obDetail.OutboundSysId == request.OutboundSysId
              select new OutboundExceptionDto()
              {
                  SysId = obDetail.SysId,
                  SkuSpecialTypes = s.SpecialTypes,
                  UPC = s.UPC,
                  SkuName = s.SkuName,
                  SkuDescr = s.SkuDescr,
                  Qty = obDetail.Qty.Value,
                  PackFactor = obDetail.PackFactor,
                  DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                               && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                      ? Math.Round(
                          ((p1.FieldValue02.Value * (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0) * 1.00m) /
                           p1.FieldValue01.Value), 3)
                      : (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0),
                  UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                      ? u2.UOMCode
                      : u.UOMCode,
                  SkuSysId = s.SysId
              };
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.UPC))
                {
                    request.UPC = request.UPC.Trim();
                    query = query.Where(p => p.UPC == request.UPC);
                }
                if (!string.IsNullOrEmpty(request.SkuName))
                {
                    request.SkuName = request.SkuName.Trim();
                    query = query.Where(p => p.SkuName.Contains(request.SkuName));
                }
            }

            request.iTotalDisplayRecords = query.Count();
            query =
                query.OrderByDescending(p => p.SkuName).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages<OutboundExceptionDto>(query, request);
        }


        public List<PrePackDetailDto> GetPrePackDetailByOutboundSysId(Guid outboundSysId, Guid wareHouseSysId)
        {

            var query = from pd in Context.prepackdetails
                        join p in Context.prepacks on pd.PrePackSysId equals p.SysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join u in Context.uoms on pd.UOMSysId equals u.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where p.OutboundSysId == outboundSysId
                        group pd by new { s, pd.Loc, ti.UOMCode }
                into t1
                        select new PrePackDetailDto
                        {
                            UPC = t1.Key.s.UPC,
                            SkuName = t1.Key.s.SkuName,
                            SkuDescr = t1.Key.s.SkuDescr,
                            UomCode = t1.Key.UOMCode,
                            Qty = t1.Sum(p => p.Qty),
                        };
            return query.ToList();
        }

        /// <summary>
        /// 获取出库单散货箱明细
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<OutboundTransferOrderDetailDto> GetTransferOrderDetailBySysIds(List<Guid> transferOrderSysIds)
        {
            var query = from pd in Context.outboundtransferorderdetail
                        join p in Context.outboundtransferorder on pd.OutboundTransferOrderSysId equals p.SysId
                        where transferOrderSysIds.Contains(p.SysId)
                        group new { pd.Qty } by new { pd.SkuSysId } into g
                        join s in Context.skus on g.Key.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        join u1 in Context.uoms on ti.FieldUom01 equals u1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u2 in Context.uoms on ti.FieldUom02 equals u2.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        select new OutboundTransferOrderDetailDto
                        {
                            UPC = s.UPC,
                            SkuSysId = g.Key.SkuSysId,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UomCode = ti.InLabelUnit01.HasValue && ti.InLabelUnit01.Value == true && ti.FieldValue01 > 0 && ti.FieldValue02 > 0 ? ti2.UOMCode : ti1.UOMCode,
                            Qty = g.Sum(p => p.Qty)
                        };
            return query.ToList();
        }

        /// <summary>
        /// 根据出库单ID获取异常明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<OutboundExceptionDtoList> GetOutbooundExceptionData(Guid sysId)
        {
            var query = from exception in Context.outboundexception
                        join od in Context.outbounddetails on exception.OutboundDetailSysId equals od.SysId into t
                        from detial in t.DefaultIfEmpty()
                        join s in Context.skus on detial.SkuSysId equals s.SysId into t1
                        from sku in t1.DefaultIfEmpty()
                        where exception.OutboundSysId == sysId
                        select new OutboundExceptionDtoList
                        {
                            SysId = exception.SysId,
                            OutboundSysId = exception.OutboundSysId,
                            OutboundDetailSysId = exception.OutboundDetailSysId,
                            ExceptionReason = exception.ExceptionReason,
                            ExceptionQty = exception.ExceptionQty == null ? 0 : (int)exception.ExceptionQty,
                            ExceptionDesc = exception.ExceptionDesc,
                            Result = exception.Result,
                            Department = exception.Department,
                            Responsibility = exception.Responsibility,
                            Remark = exception.Remark,
                            IsSettlement = exception.IsSettlement,
                            SkuSysId = detial.SkuSysId,
                            MaxQty = detial.Qty,
                            SkuName = sku.SkuName,
                            UPC = sku.UPC
                        };
            return query.ToList();
        }
        #endregion

        #region 拣货

        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            var sqlWhere = new StringBuilder();
            var mySqlParameter = new List<MySqlParameter>();
            var strSqlCount = new StringBuilder(@"select count(1) from pickDetail pd where 1=1 ");
            var strSqlCountWhere = new StringBuilder();

            var strSqlHead =
                @"SELECT pd.SysId,pd.PickDetailOrder,pd.OutboundOrder,pd.OutboundChildType,pd.PlatformOrder,pd.Status,pd.SkuSysId,pd.Lot,
                         pd.Loc,pd.Qty,pd.PickedQty, pd.PickDate,pd.Channel,s.SkuName,s.SkuDescr,u.UomCode,
                         pd.PackSysId,pd.ServiceStationName,pd.AppointUserNames,
                                                        CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                                                        THEN ROUND(p.FieldValue02 * (IFNULL(pd.Qty,0)/p.FieldValue01),3)
                                                        ELSE IFNULL(pd.Qty,0) END AS DisplayQty,
                                                        CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                                                        THEN ROUND(p.FieldValue02 * (IFNULL(pd.PickedQty,0)/p.FieldValue01),3)
                                                        ELSE IFNULL(pd.PickedQty,0) END AS DisplayPickedQty,
                                                        CASE when p.InLabelUnit01 IS NOT NULL AND p.InLabelUnit01 = 1 AND p.FieldValue01 > 0 AND p.FieldValue02 > 0
                                                        THEN u.UomCode
                                                        ELSE u.UomCode END AS UomCode
                                                        FROM (";


            var strSql =
                new StringBuilder(
                    @"select pp.SysId,pp.PickDetailOrder,pp.Status,pp.SkuSysId,pp.Lot,pp.Loc,pp.Qty,pp.PickedQty,pp.PickDate,o.OutboundOrder, o.OutboundChildType,o.PlatformOrder,pp.PackSysId,pp.OutboundSysId,o.ServiceStationName,o.AppointUserNames                          ,o.Channel  
                                                from pickDetail pp
                                                LEFT JOIN outbound o ON pp.OutboundSysId=o.SysId
            							        where 1=1   

            ");


            var strSqlBottom =
                ") pd inner join pack p ON p.SysId = pd.PackSysId " +
                "  inner join sku s on s.sysid = pd.Skusysid " +
                "  left join uom u on u.SysId = p.FieldUom02    " +
                //下边这句innerjoin 不知道谁加的，暂时没发现有啥作用
                //"    inner join (select SysId, OutboundOrder, OutboundType, PlatformOrder from outbound  union all " +
                //" select SysId, AssemblyOrder as OutboundOrder, 0 as OutboundType, '' as PlatformOrder from assembly) o on o.SysId = pd.OutboundSysId" +
                "  Order by pd.PickDetailOrder desc";

            sqlWhere = sqlWhere.AppendFormat(" and pp.WareHouseSysId = @WareHouseSysId");
            strSqlCountWhere = strSqlCountWhere.AppendFormat(" and pd.WareHouseSysId = @WareHouseSysId");

            mySqlParameter.Add(new MySqlParameter("@WareHouseSysId", pickDetailQuery.WarehouseSysId));
            if (!string.IsNullOrEmpty(pickDetailQuery.OutboundOrderSearch) || !string.IsNullOrEmpty(pickDetailQuery.ServiceStationNameSearch))
            {

                var outbound = "SELECT SysId FROM outbound o where 1=1 ";

                if (!string.IsNullOrEmpty(pickDetailQuery.OutboundOrderSearch))
                {
                    sqlWhere.AppendFormat(" and o.OutboundOrder like CONCAT(@OutboundOrder, '%')");
                    outbound = outbound + " and o.OutboundOrder like CONCAT(@OutboundOrder, '%')";
                    mySqlParameter.Add(new MySqlParameter("@OutboundOrder", pickDetailQuery.OutboundOrderSearch.Trim()));


                }
                if (!string.IsNullOrEmpty(pickDetailQuery.ServiceStationNameSearch))
                {
                    sqlWhere.AppendFormat(" and o.ServiceStationName LIKE CONCAT(@ServiceStationNameSearch, '%')");
                    outbound = outbound + " and o.ServiceStationName LIKE CONCAT(@ServiceStationNameSearch, '%') ";

                    mySqlParameter.Add(new MySqlParameter("@ServiceStationNameSearch", pickDetailQuery.ServiceStationNameSearch.Trim()));
                }
                strSqlCountWhere.AppendFormat(" and pd.outboundSysId in ({0})", outbound);
            }

            //出库单类型
            if (pickDetailQuery.OutboundTypeSearch.HasValue)
            {
                sqlWhere.AppendFormat(" and o.OutboundType = @OutboundType");
                strSqlCountWhere.AppendFormat(" and pd.outboundSysId in (select sysId from outbound where OutboundType =@OutboundType)");

                mySqlParameter.Add(new MySqlParameter("@OutboundType", pickDetailQuery.OutboundTypeSearch));

            }

            //平台订单号
            if (!string.IsNullOrEmpty(pickDetailQuery.PlatformOrderSearch))
            {
                strSqlCountWhere.AppendFormat(" and pd.outboundSysId in (select sysId from outbound where PlatformOrder like CONCAT(@PlatformOrder, '%'))");

                sqlWhere.AppendFormat(" and o.PlatformOrder like CONCAT(@PlatformOrder, '%') ");

                mySqlParameter.Add(new MySqlParameter("@PlatformOrder", pickDetailQuery.PlatformOrderSearch.Trim()));
            }


            if (!string.IsNullOrEmpty(pickDetailQuery.PickDetailOrderSearch))
            {
                sqlWhere.AppendFormat(" and pp.PickDetailOrder = @PickDetailOrder");
                strSqlCountWhere.AppendFormat(" and pd.PickDetailOrder = @PickDetailOrder");

                mySqlParameter.Add(new MySqlParameter("@PickDetailOrder", pickDetailQuery.PickDetailOrderSearch.Trim()));
            }

            if (pickDetailQuery.StatusSearch.HasValue)
            {
                if(pickDetailQuery.StatusSearch == (int)PickDetailStatus.New)
                {
                    sqlWhere.AppendFormat("  and (pp.PickedQty = 0 and pp.Status = @NewStatus) ");
                    strSqlCountWhere.AppendFormat(" and (pd.PickedQty = 0 and pd.Status = @NewStatus) ");
                    mySqlParameter.Add(new MySqlParameter("@NewStatus", (int)PickDetailStatus.New));
                }
                else if(pickDetailQuery.StatusSearch == (int)PickDetailStatus.PartPick)
                {
                    sqlWhere.AppendFormat(" and (pp.PickedQty != 0 and pp.Qty > pp.PickedQty and pp.Status = @NewStatus) ");
                    strSqlCountWhere.AppendFormat(" and (pd.PickedQty != 0 and pd.Qty > pd.PickedQty and pd.Status = @NewStatus) ");
                    mySqlParameter.Add(new MySqlParameter("@NewStatus", (int)PickDetailStatus.New));
                }
                else if(pickDetailQuery.StatusSearch == (int)PickDetailStatus.Finish)
                {
                    sqlWhere.AppendFormat(" and ((pp.PickedQty != 0 and pp.Qty = pp.PickedQty and pp.Status = @NewStatus) or pp.Status = @FinishStatus) ");
                    strSqlCountWhere.AppendFormat(" and ((pd.PickedQty != 0 and pd.Qty = pd.PickedQty and pd.Status = @NewStatus) or pd.Status = @FinishStatus) ");
                    mySqlParameter.Add(new MySqlParameter("@NewStatus", (int)PickDetailStatus.New));
                    mySqlParameter.Add(new MySqlParameter("@FinishStatus", (int)PickDetailStatus.Finish));
                }
                else
                {
                    sqlWhere.AppendFormat(" and pp.Status = @Status");
                    strSqlCountWhere.AppendFormat(" and pd.Status =@Status");
                    mySqlParameter.Add(new MySqlParameter("@Status", pickDetailQuery.StatusSearch));
                }
               
            }

            if (!string.IsNullOrEmpty(pickDetailQuery.SkuUPCSearch) ||
                !string.IsNullOrEmpty(pickDetailQuery.SkuNameSearch))
            {
                var skusql = @"select SysId from sku ";
                //商品UPC
                if (!string.IsNullOrEmpty(pickDetailQuery.SkuUPCSearch))
                {
                    sqlWhere.AppendFormat(" and pp.SkusysId in ({0}) ", skusql + " where upc like  CONCAT(@SkuUPC, '%') ");

                    strSqlCountWhere.AppendFormat(" and pd.SkusysId  in ({0}) ", skusql + " where upc like CONCAT(@SkuUPC, '%') ");

                    mySqlParameter.Add(new MySqlParameter("@SkuUPC", pickDetailQuery.SkuUPCSearch.Trim()));
                }

                //商品名称
                if (!string.IsNullOrEmpty(pickDetailQuery.SkuNameSearch))
                {
                    sqlWhere.AppendFormat(" and pp.SkusysId  in ({0}) ", skusql + " where SkuName like  CONCAT(@SkuName, '%')  ");
                    strSqlCountWhere.AppendFormat(" and pd.SkusysId  in ({0}) ", skusql + " where SkuName like CONCAT(@SkuName, '%') ");
                    mySqlParameter.Add(new MySqlParameter("@SkuName", pickDetailQuery.SkuNameSearch.Trim()));
                }

            }

            var rowCount = base.Context.Database.SqlQuery<int>(strSqlCount.ToString() + strSqlCountWhere, mySqlParameter.ToArray()).FirstOrDefault();

            strSql = strSql.AppendFormat(" {0}", sqlWhere, ToString());
            strSql = strSql.Append(" Order by pp.PickDetailOrder desc LIMIT @Start, @Length");

            mySqlParameter.Add(new MySqlParameter("@Start", pickDetailQuery.iDisplayStart));
            mySqlParameter.Add(new MySqlParameter("@Length", pickDetailQuery.iDisplayLength));
            var queryList =
                base.Context.Database.SqlQuery<PickDetailListDto>(strSqlHead + strSql.ToString() + strSqlBottom, mySqlParameter.ToArray()).AsQueryable();

            pickDetailQuery.iTotalDisplayRecords = rowCount;
            return ConvertPages(queryList, pickDetailQuery);
        }

        public List<PickDetailListDto> GetSummaryPickDetailListDto(List<Guid?> pickDetailSysIds, Guid wareHouseSysId)
        {

            var query = from pd in (
                from pd in Context.pickdetails
                where pickDetailSysIds.Contains(pd.SysId)
                group new { pd.OutboundSysId, pd.PickDetailOrder } by new { pd.OutboundSysId, pd.PickDetailOrder }
                into g
                select new { g.Key.OutboundSysId, g.Key.PickDetailOrder })
                        group new { pd.PickDetailOrder } by new { pd.PickDetailOrder }
                into g
                        select new PickDetailListDto
                        {
                            PickDetailOrder = g.Key.PickDetailOrder,
                            OutboundCount = g.Count()
                        };

            return query.ToList();
        }

        public List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid?> outboundSysIds, Guid wareHouseSysId)
        {

            var query = from obd in Context.outbounddetails
                        where outboundSysIds.Contains(obd.OutboundSysId)
                        group obd by new { obd.OutboundSysId }
                into g
                        select new PickOutboundDetailListDto()
                        {
                            OutboundSysId = g.Key.OutboundSysId,
                            SkuTypeQty = g.Count()
                        };

            return query.ToList();
        }

        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            var query = from ob in Context.outbounds
                        where ob.WareHouseSysId == pickDetailQuery.WarehouseSysId
                            && ob.Status == (int)OutboundStatus.New
                        select new { ob };

            #region 查询条件

            //UPC
            if (!string.IsNullOrEmpty(pickDetailQuery.SkuUPCSearch))
            {
                List<string> skuUPC = pickDetailQuery.SkuUPCSearch.Split(',').ToList();
                query = from tempQuery in query
                        join obd in Context.outbounddetails on tempQuery.ob.SysId equals obd.OutboundSysId
                        join s in Context.skus on obd.SkuSysId equals s.SysId
                        where skuUPC.Contains(s.UPC)
                        select new { tempQuery.ob };

                //query = query.Where(x => skuUPC.Contains(x.s.UPC));
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
                pickDetailQuery.ServiceStationNameSearch = pickDetailQuery.ServiceStationNameSearch.Trim();
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
                        select new { ob = q.ob };
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

            if (!pickDetailQuery.OutboundChildTypeSearch.IsNull())
            {
                pickDetailQuery.OutboundChildTypeSearch = pickDetailQuery.OutboundChildTypeSearch.Trim();
                query = query.Where(p => p.ob.OutboundChildType.Contains(pickDetailQuery.OutboundChildTypeSearch));
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
                ServiceStationName = x.ob.ServiceStationName,
                OutboundChildType = x.ob.OutboundChildType
            }).Distinct();

            pickDetailQuery.iTotalDisplayRecords = pickDetail.Count();
            pickDetail = pickDetail.OrderByDescending(p => p.AuditingDate).Skip(pickDetailQuery.iDisplayStart).Take(pickDetailQuery.iDisplayLength);
            return ConvertPages<PickOutboundListDto>(pickDetail, pickDetailQuery);

        }

        #endregion

        #region 入库

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery)
        {
            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId

                        select new { po, v };

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

            if (!string.IsNullOrEmpty(purchaseQuery.SkuCodeSearch) || !string.IsNullOrEmpty(purchaseQuery.SkuNameSearch) || !string.IsNullOrEmpty(purchaseQuery.UpcCodeSearch))
            {
                var skuQuery = from tempQuery in query
                               join poDetail in Context.purchasedetails on tempQuery.po.SysId equals poDetail.PurchaseSysId
                               join s in Context.skus on poDetail.SkuSysId equals s.SysId
                               select new { tempQuery.po, tempQuery.v, s };

                if (!string.IsNullOrEmpty(purchaseQuery.SkuCodeSearch))
                {
                    purchaseQuery.SkuCodeSearch = purchaseQuery.SkuCodeSearch.Trim();

                    skuQuery = skuQuery.Where(x => x.s.SkuCode == purchaseQuery.SkuCodeSearch);
                }
                if (!string.IsNullOrEmpty(purchaseQuery.SkuNameSearch))
                {
                    purchaseQuery.SkuNameSearch = purchaseQuery.SkuNameSearch.Trim();
                    skuQuery = skuQuery.Where(x => x.s.SkuName.Contains(purchaseQuery.SkuNameSearch));
                }
                if (!string.IsNullOrEmpty(purchaseQuery.UpcCodeSearch))
                {
                    purchaseQuery.UpcCodeSearch = purchaseQuery.UpcCodeSearch.Trim();
                    skuQuery = skuQuery.Where(x => purchaseQuery.UpcCodeSearch == x.s.UPC);
                }

                query = from tempQuery in skuQuery
                        select new
                        {
                            tempQuery.po,
                            tempQuery.v
                        };
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

            if (!string.IsNullOrEmpty(purchaseQuery.ReceiptOrderSearch))
            {
                purchaseQuery.ReceiptOrderSearch = purchaseQuery.ReceiptOrderSearch.Trim();
                query = from tempQuery in query
                        join y in Context.receipts on tempQuery.po.PurchaseOrder equals y.ExternalOrder
                        where y.ReceiptOrder.Contains(purchaseQuery.ReceiptOrderSearch)
                        select new { tempQuery.po, tempQuery.v };
                //query = query.Where(x => x.y.ReceiptOrder.Contains(purchaseQuery.ReceiptOrderSearch));
            }

            if (!string.IsNullOrEmpty(purchaseQuery.TransferInventoryOrderSearch))
            {
                purchaseQuery.TransferInventoryOrderSearch = purchaseQuery.TransferInventoryOrderSearch.Trim();
                query = from tempQuery in query
                        join t in Context.transferinventorys on tempQuery.po.SysId equals t.TransferPurchaseSysId
                        where t.TransferInventoryOrder.Contains(purchaseQuery.TransferInventoryOrderSearch)
                        select new { tempQuery.po, tempQuery.v };
                //query = query.Where(x => x.w.TransferInventoryOrder.Contains(purchaseQuery.TransferInventoryOrderSearch));
            }

            if (purchaseQuery.ToWareHouseSysId.HasValue)
            {
                query = from tempQuery in query
                        join t in Context.transferinventorys on tempQuery.po.SysId equals t.TransferPurchaseSysId
                        where t.ToWareHouseSysId == purchaseQuery.ToWareHouseSysId
                        select new { tempQuery.po, tempQuery.v };
                //query = query.Where(x => x.w.ToWareHouseSysId == purchaseQuery.ToWareHouseSysId);
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

            if (purchaseQuery.IsPurchaseReturn)
            {
                query = query.Where(x => x.po.Type == ((int)PurchaseType.Return));
            }
            else
            {
                query = query.Where(x => x.po.Type != ((int)PurchaseType.Return));
            }
            //渠道
            if (!string.IsNullOrEmpty(purchaseQuery.Channel))
            {
                purchaseQuery.Channel = purchaseQuery.Channel.Trim();
                query = query.Where(x => x.po.Channel.Contains(purchaseQuery.Channel));
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
                BatchNumber = x.po.BatchNumber,
                BusinessType = x.po.BusinessType,
                Channel = x.po.Channel
            }).Distinct();

            purchaseQuery.iTotalDisplayRecords = purchase.Count();
            purchase = purchase.OrderByDescending(p => p.AuditingDate).Skip(purchaseQuery.iDisplayStart).Take(purchaseQuery.iDisplayLength);
            return ConvertPages<PurchaseListDto>(purchase, purchaseQuery);


        }


        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseReturnListDto> GetPurchaseReturnDtoListByPageInfo(PurchaseReturnQuery purchaseQuery)
        {
            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId

                        join p in Context.purchaseextend on po.SysId equals p.PurchaseSysId into t1
                        from pe in t1.DefaultIfEmpty()

                        select new { po, v, pe };

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

            if (!string.IsNullOrEmpty(purchaseQuery.SkuCodeSearch) || !string.IsNullOrEmpty(purchaseQuery.SkuNameSearch) || !string.IsNullOrEmpty(purchaseQuery.UpcCodeSearch))
            {
                var skuQuery = from tempQuery in query
                               join poDetail in Context.purchasedetails on tempQuery.po.SysId equals poDetail.PurchaseSysId
                               join s in Context.skus on poDetail.SkuSysId equals s.SysId
                               select new { tempQuery.po, tempQuery.v, tempQuery.pe, s };

                if (!string.IsNullOrEmpty(purchaseQuery.SkuCodeSearch))
                {
                    purchaseQuery.SkuCodeSearch = purchaseQuery.SkuCodeSearch.Trim();

                    skuQuery = skuQuery.Where(x => x.s.SkuCode == purchaseQuery.SkuCodeSearch);
                }
                if (!string.IsNullOrEmpty(purchaseQuery.SkuNameSearch))
                {
                    purchaseQuery.SkuNameSearch = purchaseQuery.SkuNameSearch.Trim();
                    skuQuery = skuQuery.Where(x => x.s.SkuName.Contains(purchaseQuery.SkuNameSearch));
                }
                if (!string.IsNullOrEmpty(purchaseQuery.UpcCodeSearch))
                {
                    purchaseQuery.UpcCodeSearch = purchaseQuery.UpcCodeSearch.Trim();
                    skuQuery = skuQuery.Where(x => purchaseQuery.UpcCodeSearch == x.s.UPC);
                }

                query = from tempQuery in skuQuery
                        select new
                        {
                            tempQuery.po,
                            tempQuery.v,
                            tempQuery.pe
                        };
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

            if (!string.IsNullOrEmpty(purchaseQuery.ReceiptOrderSearch))
            {
                purchaseQuery.ReceiptOrderSearch = purchaseQuery.ReceiptOrderSearch.Trim();
                query = from tempQuery in query
                        join y in Context.receipts on tempQuery.po.PurchaseOrder equals y.ExternalOrder
                        where y.ReceiptOrder.Contains(purchaseQuery.ReceiptOrderSearch)
                        select new { tempQuery.po, tempQuery.v, tempQuery.pe };
                //query = query.Where(x => x.y.ReceiptOrder.Contains(purchaseQuery.ReceiptOrderSearch));
            }
            //渠道
            if (!string.IsNullOrEmpty(purchaseQuery.Channel))
            {
                purchaseQuery.Channel = purchaseQuery.Channel.Trim();
                query = query.Where(x => x.po.Channel.Contains(purchaseQuery.Channel));
            }

            //服务站
            if (!string.IsNullOrEmpty(purchaseQuery.ServiceStationName))
            {
                purchaseQuery.Channel = purchaseQuery.ServiceStationName.Trim();
                query = query.Where(x => x.pe.ServiceStationName.Contains(purchaseQuery.ServiceStationName));
            }

            if (!string.IsNullOrEmpty(purchaseQuery.TransferInventoryOrderSearch))
            {
                purchaseQuery.TransferInventoryOrderSearch = purchaseQuery.TransferInventoryOrderSearch.Trim();
                query = from tempQuery in query
                        join t in Context.transferinventorys on tempQuery.po.SysId equals t.TransferPurchaseSysId
                        where t.TransferInventoryOrder.Contains(purchaseQuery.TransferInventoryOrderSearch)
                        select new { tempQuery.po, tempQuery.v, tempQuery.pe };
                //query = query.Where(x => x.w.TransferInventoryOrder.Contains(purchaseQuery.TransferInventoryOrderSearch));
            }

            if (purchaseQuery.ToWareHouseSysId.HasValue)
            {
                query = from tempQuery in query
                        join t in Context.transferinventorys on tempQuery.po.SysId equals t.TransferPurchaseSysId
                        where t.ToWareHouseSysId == purchaseQuery.ToWareHouseSysId
                        select new { tempQuery.po, tempQuery.v, tempQuery.pe };
                //query = query.Where(x => x.w.ToWareHouseSysId == purchaseQuery.ToWareHouseSysId);
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
                query = query.Where(x => x.po.BatchNumber == purchaseQuery.BatchNumber.Trim());
            }

            if (purchaseQuery.IsPurchaseReturn)
            {
                query = query.Where(x => x.po.Type == ((int)PurchaseType.Return));
            }
            else
            {
                query = query.Where(x => x.po.Type != ((int)PurchaseType.Return));
            }


            if (!string.IsNullOrEmpty(purchaseQuery.ExpressNumber))
            {
                purchaseQuery.ExpressNumber = purchaseQuery.ExpressNumber.Trim();
                query = query.Where(x => x.pe.ExpressNumber == purchaseQuery.ExpressNumber);

                //var skuQuery = from tempQuery in query
                //               join poDetail in Context.purchasedetails on tempQuery.po.SysId equals poDetail.PurchaseSysId
                //               join s in Context.skus on poDetail.SkuSysId equals s.SysId
                //               select new { tempQuery.po, tempQuery.v, s };

                //query = from tempQuery in query
                //        join p in Context.purchaseextend on tempQuery.po.SysId equals p.PurchaseSysId into t1
                //        from pe in t1.DefaultIfEmpty()
                //        select new { tempQuery.po, tempQuery.v };
            }


            #endregion

            var purchase = query.Select(x => new PurchaseReturnListDto()
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
                BatchNumber = x.po.BatchNumber,
                BusinessType = x.po.BusinessType,
                ExpressNumber = x.pe.ExpressNumber,
                Channel = x.po.Channel,
                ServiceStationName = x.pe.ServiceStationName
            }).Distinct();

            purchaseQuery.iTotalDisplayRecords = purchase.Count();
            purchase = purchase.OrderByDescending(p => p.AuditingDate).Skip(purchaseQuery.iDisplayStart).Take(purchaseQuery.iDisplayLength);
            return ConvertPages<PurchaseReturnListDto>(purchase, purchaseQuery);


        }

        /// <summary>
        /// 获取采购订单信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PurchaseViewDto GetPurchaseViewDtoBySysId(Guid sysId, Guid wareHouseSysId)
        {

            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId
                        //join tf in Context.transferinventorys on po.SysId equals tf.TransferPurchaseSysId into t0
                        //from tfInfo in t0.DefaultIfEmpty()
                        join o in Context.outbounds on po.OutboundSysId equals o.SysId into t1
                        from ot in t1.DefaultIfEmpty()
                        join w in Context.warehouses on po.FromWareHouseSysId equals w.SysId into t2
                        from w1 in t2.DefaultIfEmpty()
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
                            FromWareHouseName = w1.Name,
                            //  ToWareHouseName = tfInfo.ToWareHouseName,
                            TransferInventoryOrder = po.ExternalOrder,  //,tfInfo.TransferInventoryOrder,
                            OutboundSysId = po.OutboundSysId,
                            OutboundOrder = po.OutboundOrder,
                            OutboundType = ot.OutboundType,
                            IsReturn = ot.IsReturn,
                            Channel = po.Channel,
                            Descr = po.Descr
                        };
            return query.FirstOrDefault();
        }


        public PurchaseReturnViewDto GetPurchaseReturnViewDtoBySysId(Guid sysId, Guid wareHouseSysId)
        {

            var query = from po in Context.purchases
                        join v in Context.vendors on po.VendorSysId equals v.SysId
                        //join tf in Context.transferinventorys on po.SysId equals tf.TransferPurchaseSysId into t0
                        //from tfInfo in t0.DefaultIfEmpty()
                        join o in Context.outbounds on po.OutboundSysId equals o.SysId into t1
                        from ot in t1.DefaultIfEmpty()
                        join w in Context.warehouses on po.FromWareHouseSysId equals w.SysId into t2
                        from w1 in t2.DefaultIfEmpty()
                        join e in Context.purchaseextend on po.SysId equals e.PurchaseSysId into t3
                        from pe in t3.DefaultIfEmpty()
                        join q in Context.qualitycontrol on po.PurchaseOrder equals q.DocOrder into t4
                        from qc in t4.DefaultIfEmpty()

                        where po.SysId == sysId
                        select new PurchaseReturnViewDto()
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
                            FromWareHouseName = w1.Name,
                            //  ToWareHouseName = tfInfo.ToWareHouseName,
                            TransferInventoryOrder = po.ExternalOrder,  //,tfInfo.TransferInventoryOrder,
                            OutboundSysId = po.OutboundSysId,
                            OutboundOrder = po.OutboundOrder,
                            OutboundType = ot.OutboundType,
                            IsReturn = ot.IsReturn,
                            CustomerName = pe.CustomerName,
                            ReturnContact = pe.ReturnContact,
                            ShippingAddress = pe.ShippingAddress,
                            ExpressCompany = pe.ExpressCompany,
                            ExpressNumber = pe.ExpressNumber,
                            ReturnTime = pe.ReturnTime,
                            ReturnReason = pe.ReturnReason,
                            QCTime = qc.QCDate,
                            PlatformOrderId = pe.PlatformOrderId,
                            Channel = po.Channel,
                            FromWareHouseSysId = po.FromWareHouseSysId
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取PurchaseDetailViewDto 根据主表的SysId
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailViewDto> GetPurchaseDetailViewBySysId(Guid sysId, Guid wareHouseSysId)
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
                        where poDetail.PurchaseSysId == sysId
                        select new PurchaseDetailViewDto
                        {
                            SysId = poDetail.SysId,
                            SkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            IsMaterial = sku.IsMaterial,
                            SkuUPC = sku.UPC,
                            SkuDescr = sku.SkuDescr,
                            PackSysId = poDetail.PackSysId,
                            UomSysId = poDetail.UOMSysId,
                            PackCode = poDetail.PackCode,
                            UomCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti4.UOMCode : ti3.UOMCode,
                            Qty = poDetail.Qty,
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
                            GiftQty = poDetail.GiftQty,
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
        /// PurchaseDetailReturnViewDto 根据主表的SysId
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailReturnViewDto> GetPurchaseDetailReturnViewBySysId(Guid sysId, Guid wareHouseSysId)
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

                        join pu in Context.purchases on poDetail.PurchaseSysId equals pu.SysId into t5
                        from ti5 in t5.DefaultIfEmpty()

                        join qc in Context.qualitycontrol on new { ID1 = ti5.PurchaseOrder, ID2 = (int)QCStatus.Finish } equals new { ID1 = qc.DocOrder, ID2 = qc.Status } into t6
                        from ti6 in t6.DefaultIfEmpty()

                        join qcd in Context.qualitycontroldetail on new { ID1 = ti6.SysId, ID2 = poDetail.SkuSysId } equals new { ID1 = qcd.QualityControlSysId, ID2 = qcd.SkuSysId } into t7
                        from ti7 in t7.DefaultIfEmpty()

                        where poDetail.PurchaseSysId == sysId
                        select new PurchaseDetailReturnViewDto
                        {
                            SysId = poDetail.SysId,
                            SkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            IsMaterial = sku.IsMaterial,
                            SkuUPC = sku.UPC,
                            SkuDescr = sku.SkuDescr,
                            PackSysId = poDetail.PackSysId,
                            UomSysId = poDetail.UOMSysId,
                            PackCode = poDetail.PackCode,
                            UomCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti4.UOMCode : ti3.UOMCode,
                            Qty = poDetail.Qty,
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
                            GiftQty = poDetail.GiftQty,
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
                            AbnormalQty = ti7.Qty,
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
        #endregion

        #region 收货

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Pages<ReceiptListDto> GetReceiptListByPaging(ReceiptQuery receiptQuery)
        {

            var receiptDetail = from rd in Context.receiptdetails
                                group rd by new { rd.ReceiptSysId } into g
                                select new
                                {
                                    ReceiptSysId = g.Key.ReceiptSysId,
                                    TotalReceivedQty = g.Sum(x => x.ReceivedQty),
                                    TotalShelvesQty = g.Sum(x => x.ShelvesQty)
                                };

            var query = from r in Context.receipts
                        join p in Context.purchases on r.ExternalOrder equals p.PurchaseOrder
                        join v in Context.vendors on p.VendorSysId equals v.SysId
                        join trd in receiptDetail on r.SysId equals trd.ReceiptSysId into temprd
                        from rd in temprd.DefaultIfEmpty()

                        join trd1 in Context.receiptdetails on r.SysId equals trd1.ReceiptSysId into temprd1
                        from rd1 in temprd1.DefaultIfEmpty()

                        join s in Context.skus on rd1.SkuSysId equals s.SysId into sTemp
                        from sku in sTemp.DefaultIfEmpty()
                        where r.Status != 0
                        select new { r, rd, v.VendorName, sku };

            query = query.Where(p => p.r.WarehouseSysId == receiptQuery.WarehouseSysId);

            if (!receiptQuery.ReceiptOrderSearch.IsNull())
            {
                receiptQuery.ReceiptOrderSearch = receiptQuery.ReceiptOrderSearch.Trim();
                query = query.Where(p => p.r.ReceiptOrder == receiptQuery.ReceiptOrderSearch);
            }
            if (!receiptQuery.VendorNameSearch.IsNull())
            {
                receiptQuery.VendorNameSearch = receiptQuery.VendorNameSearch.Trim();
                query = query.Where(p => p.VendorName.Contains(receiptQuery.VendorNameSearch));
            }
            if (receiptQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.r.Status == receiptQuery.StatusSearch.Value);
            }
            if (!receiptQuery.ExternalOrderSearch.IsNull())
            {
                receiptQuery.ExternalOrderSearch = receiptQuery.ExternalOrderSearch.Trim();
                query = query.Where(p => p.r.ExternalOrder == receiptQuery.ExternalOrderSearch);
            }
            if (receiptQuery.ReceiptDateFromSearch.HasValue && receiptQuery.ReceiptDateToSearch.HasValue)
            {
                query = query.Where(p => receiptQuery.ReceiptDateFromSearch <= p.r.ReceiptDate && p.r.ReceiptDate <= receiptQuery.ReceiptDateToSearch);
            }
            if (receiptQuery.CreateDateFromSearch.HasValue && receiptQuery.CreateDateToSearch.HasValue)
            {
                query = query.Where(p => receiptQuery.CreateDateFromSearch <= p.r.CreateDate && p.r.CreateDate <= receiptQuery.CreateDateToSearch);
            }
            if (receiptQuery.IsMaterial.HasValue)
            {
                if (receiptQuery.IsMaterial.Value)
                {
                    query = query.Where(p => p.sku.IsMaterial == receiptQuery.IsMaterial.Value);
                }
                else
                {
                    //不是原材料
                    query = query.Where(p => p.sku.IsMaterial != (!receiptQuery.IsMaterial.Value));
                }
            }

            //商品名称
            if (!string.IsNullOrEmpty(receiptQuery.SkuNameSearch))
            {
                receiptQuery.SkuNameSearch = receiptQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.sku.SkuName.Contains(receiptQuery.SkuNameSearch));
            }

            //商品UPC
            if (!string.IsNullOrEmpty(receiptQuery.UPCSearch))
            {
                receiptQuery.UPCSearch = receiptQuery.UPCSearch.Trim();
                query = query.Where(p => p.sku.UPC == receiptQuery.UPCSearch);
            }

            if (receiptQuery.ShelvesStatusSearch != null)
            {
                //未上架
                if (receiptQuery.ShelvesStatusSearch == (int)ShelvesStatus.NotOnShelves)
                {
                    query = query.Where(p => p.rd.TotalReceivedQty == null || p.rd.TotalShelvesQty == 0);
                }

                //上架中
                if (receiptQuery.ShelvesStatusSearch == (int)ShelvesStatus.Shelves)
                {
                    query = query.Where(p => p.rd.TotalShelvesQty > 0 && p.rd.TotalShelvesQty < p.rd.TotalReceivedQty);
                }

                //上架完成
                if (receiptQuery.ShelvesStatusSearch == (int)ShelvesStatus.Finish)
                {
                    query = query.Where(p => p.rd.TotalReceivedQty != null && p.rd.TotalReceivedQty != 0 && p.rd.TotalShelvesQty == p.rd.TotalReceivedQty);
                }
            }

            var receipts = query.Select(p => new ReceiptListDto()
            {
                SysId = p.r.SysId,
                ReceiptOrder = p.r.ReceiptOrder,
                //DisplayExternalOrder = p.r.DisplayExternalOrder,
                VendorName = p.VendorName,
                ReceiptType = p.r.ReceiptType,
                Status = p.r.Status,
                ExpectedReceiptDate = p.r.ExpectedReceiptDate,
                ExternalOrder = p.r.ExternalOrder,
                ReceiptDate = p.r.ReceiptDate,
                CreateDate = p.r.CreateDate,
                TotalReceivedQty = p.rd.TotalReceivedQty,
                TotalShelvesQty = p.rd.TotalShelvesQty,
                AppointUserNames = p.r.AppointUserNames
            }).Distinct();
            receiptQuery.iTotalDisplayRecords = receipts.Count();
            receipts = receipts.OrderByDescending(p => p.CreateDate).Skip(receiptQuery.iDisplayStart).Take(receiptQuery.iDisplayLength);
            return ConvertPages(receipts, receiptQuery);
        }

        /// <summary>
        /// 根据Id获取收货单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ReceiptViewDto GetReceiptViewById(Guid sysId, Guid wareHouseSysId)
        {

            var query = from r in Context.receipts
                        join v in Context.vendors on r.VendorSysId equals v.SysId
                        where r.SysId == sysId
                        select new { r, VendorName = v.VendorName };
            var receipt = query.Select(p => new ReceiptViewDto()
            {
                SysId = p.r.SysId,
                ReceiptOrder = p.r.ReceiptOrder,
                //DisplayExternalOrder = p.r.DisplayExternalOrder,
                ExternalOrder = p.r.ExternalOrder,
                AppointUserNames = p.r.AppointUserNames,
                ReceiptType = p.r.ReceiptType,
                WarehouseSysId = p.r.WarehouseSysId,
                ExpectedReceiptDate = p.r.ExpectedReceiptDate,
                ReceipDate = p.r.ReceiptDate,
                Status = p.r.Status,
                Descr = p.r.Descr,
                ReturnDescr = p.r.ReturnDescr,
                CreateBy = p.r.CreateBy,
                CreateDate = p.r.CreateDate,
                UpdateBy = p.r.UpdateBy,
                UpdateDate = p.r.UpdateDate,
                IsActive = p.r.IsActive,
                VendorId = p.r.VendorSysId,
                VendorName = p.VendorName,
                ClosedDate = p.r.ClosedDate,
                ArrivalDate = p.r.ArrivalDate,
                TotalExpectedQty = p.r.TotalExpectedQty,
                TotalReceivedQty = p.r.TotalReceivedQty,
                TotalRejectedQty = p.r.TotalRejectedQty,

            }).Distinct().FirstOrDefault();
            return receipt;
        }

        /// <summary>
        /// 获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid receiptSysId, Guid wareHouseSysId)
        {

            var query = from rd in Context.receiptdetails
                        join rdc in Context.receiptdatarecord on new { rd.ReceiptSysId, rd.SkuSysId } equals new { rdc.ReceiptSysId, rdc.SkuSysId } into t0
                        from rdcp in t0.DefaultIfEmpty()
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        join u in Context.uoms on rd.UOMSysId equals u.SysId into t1
                        where rd.ReceiptSysId == receiptSysId
                        from ut1 in t1.DefaultIfEmpty()
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join uu in Context.uoms on p1.FieldUom02 equals uu.SysId into t3
                        from uu1 in t3.DefaultIfEmpty()
                        join r in Context.receipts on rd.ReceiptSysId equals r.SysId into t4
                        from r1 in t4.DefaultIfEmpty()
                        join pu in Context.purchases on r1.ExternalOrder equals pu.PurchaseOrder into t5
                        from pu1 in t5.DefaultIfEmpty()
                        join pd in Context.purchasedetails on new { PurchaseSysId = pu1.SysId, rd.SkuSysId } equals new { pd.PurchaseSysId, pd.SkuSysId } into t6
                        from pd1 in t6.DefaultIfEmpty()
                        select new { rd, s, UOMDescr = ut1.UOMCode, p1, uu1, pd1, rdcp };
            var receiptDetails = query.GroupBy(p => new
            {
                p.rd.ReceiptSysId,
                p.rd.SkuSysId,
                p.rd.Remark,
                p.rd.UOMSysId,
                p.rd.PackSysId,
                p.s.SkuCode,
                p.s.SkuName,
                p.s.UPC,
                p.s.OtherId,
                p.s.SkuDescr,
                p.p1.InLabelUnit01,
                p.uu1.UOMCode,
                p.UOMDescr,
                p.pd1.PackFactor,
                p.p1.UPC01,
                p.p1.UPC02,
                p.p1.UPC03,
                p.p1.UPC04,
                p.p1.UPC05,
                p.p1.FieldValue01,
                p.p1.FieldValue02,
                p.p1.FieldValue03,
                p.p1.FieldValue04,
                p.p1.FieldValue05,
                p.rd.IsMustLot,
                p.rdcp.RejectedQty,
                p.rdcp.GiftQty,
                p.rdcp.AdjustmentQty
            })
                .Select(p => new ReceiptDetailViewDto()
                {
                    ReceiptSysId = p.Key.ReceiptSysId,
                    SkuSysId = p.Key.SkuSysId,
                    ExpectedQty = p.Sum(x => x.rd.ExpectedQty),
                    ReceivedQty = p.Sum(x => x.rd.ReceivedQty),
                    RejectedQty = p.Key.RejectedQty,
                    Remark = p.Key.Remark,
                    UOMSysId = p.Key.UOMSysId,
                    PackSysId = p.Key.PackSysId,
                    SkuCode = p.Key.SkuCode,
                    SkuName = p.Key.SkuName,
                    SkuUPC = p.Key.UPC,
                    OtherId = p.Key.OtherId,
                    SkuDescr = p.Key.SkuDescr,
                    UOMDescr = p.Key.InLabelUnit01.HasValue && p.Key.InLabelUnit01.Value == true && p.Key.FieldValue01 > 0 && p.Key.FieldValue02 > 0 ? p.Key.UOMCode : p.Key.UOMDescr,
                    ShelvesQty = p.Sum(x => x.rd.ShelvesQty),
                    PackFactor = p.Key.PackFactor,
                    UPC01 = p.Key.UPC01,
                    UPC02 = p.Key.UPC02,
                    UPC03 = p.Key.UPC03,
                    UPC04 = p.Key.UPC04,
                    UPC05 = p.Key.UPC05,
                    GiftQty = p.Key.GiftQty,
                    AdjustmentQty = p.Key.AdjustmentQty,
                    FieldValue01 = p.Key.FieldValue01,
                    FieldValue02 = p.Key.FieldValue02,
                    FieldValue03 = p.Key.FieldValue03,
                    FieldValue04 = p.Key.FieldValue04,
                    FieldValue05 = p.Key.FieldValue05,
                    IsMustLot = p.Key.IsMustLot,
                    DisplayExpectedQty = p.Key.InLabelUnit01.HasValue && p.Key.InLabelUnit01.Value == true
                            && p.Key.FieldValue01 > 0 && p.Key.FieldValue02 > 0
                            ? Math.Round(((p.Key.FieldValue02.Value * (p.Sum(x => x.rd.ExpectedQty.HasValue ? x.rd.ExpectedQty.Value : 0)) * 1.00m) / p.Key.FieldValue01.Value), 3) : (p.Sum(x => x.rd.ExpectedQty.HasValue ? x.rd.ExpectedQty.Value : 0)),
                    DisplayReceivedQty = p.Key.InLabelUnit01.HasValue && p.Key.InLabelUnit01.Value == true
                            && p.Key.FieldValue01 > 0 && p.Key.FieldValue02 > 0
                            ? Math.Round(((p.Key.FieldValue02.Value * (p.Sum(x => x.rd.ReceivedQty.HasValue ? x.rd.ReceivedQty.Value : 0)) * 1.00m) / p.Key.FieldValue01.Value), 3) : (p.Sum(x => x.rd.ReceivedQty.HasValue ? x.rd.ReceivedQty.Value : 0)),
                    DisplayRejectedQty = p.Key.InLabelUnit01.HasValue && p.Key.InLabelUnit01.Value == true
                            && p.Key.FieldValue01 > 0 && p.Key.FieldValue02 > 0
                            ? Math.Round(((p.Key.FieldValue02.Value * p.Key.RejectedQty * 1.00m) / p.Key.FieldValue01.Value), 3) : p.Key.RejectedQty,
                    DisplayShelvesQty = p.Key.InLabelUnit01.HasValue && p.Key.InLabelUnit01.Value == true
                            && p.Key.FieldValue01 > 0 && p.Key.FieldValue02 > 0
                            ? Math.Round(((p.Key.FieldValue02.Value * p.Sum(x => x.rd.ShelvesQty) * 1.00m) / p.Key.FieldValue01.Value), 3) : p.Sum(x => x.rd.ShelvesQty)

                }).Distinct().OrderBy(p => p.SkuName).ToList();
            return receiptDetails;
        }

        /// <summary>
        /// 获取收货批次清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailLotViewList(Guid receiptSysId, Guid wareHouseSysId)
        {
            var query = from rd in Context.receiptdetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        join u in Context.uoms on rd.UOMSysId equals u.SysId into t1
                        where rd.ReceiptSysId == receiptSysId
                        from ut1 in t1.DefaultIfEmpty()
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join uu in Context.uoms on p1.FieldUom02 equals uu.SysId into t3
                        from uu1 in t3.DefaultIfEmpty()
                        join r in Context.receipts on rd.ReceiptSysId equals r.SysId into t4
                        from r1 in t4.DefaultIfEmpty()
                        join pu in Context.purchases on r1.ExternalOrder equals pu.PurchaseOrder into t5
                        from pu1 in t5.DefaultIfEmpty()
                        join pd in Context.purchasedetails on new { PurchaseSysId = pu1.SysId, rd.SkuSysId } equals new { pd.PurchaseSysId, pd.SkuSysId } into t6
                        from pd1 in t6.DefaultIfEmpty()
                        select new { rd, s, UOMDescr = ut1.UOMCode, p1, uu1, pd1 };
            var receiptDetails = query.Select(p => new ReceiptDetailViewDto()
            {
                SysId = p.rd.SysId,
                ReceiptSysId = p.rd.ReceiptSysId,
                SkuSysId = p.rd.SkuSysId,
                ExpectedQty = p.rd.ExpectedQty,
                ReceivedQty = p.rd.ReceivedQty,
                RejectedQty = p.rd.RejectedQty,
                Remark = p.rd.Remark,
                UOMSysId = p.rd.UOMSysId,
                PackSysId = p.rd.PackSysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuUPC = p.s.UPC,
                OtherId = p.s.OtherId,
                SkuDescr = p.s.SkuDescr,
                UOMDescr = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0 ? p.uu1.UOMCode : p.UOMDescr,
                ShelvesQty = p.rd.ShelvesQty,
                PackFactor = p.pd1.PackFactor,
                ToLot = p.rd.ToLot,
                LotAttr01 = p.rd.LotAttr01,
                UPC01 = p.p1.UPC01,
                UPC02 = p.p1.UPC02,
                UPC03 = p.p1.UPC03,
                UPC04 = p.p1.UPC04,
                UPC05 = p.p1.UPC05,
                FieldValue01 = p.p1.FieldValue01,
                FieldValue02 = p.p1.FieldValue02,
                FieldValue03 = p.p1.FieldValue03,
                FieldValue04 = p.p1.FieldValue04,
                FieldValue05 = p.p1.FieldValue05,
                DisplayExpectedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ExpectedQty.HasValue ? p.rd.ExpectedQty.Value : 0),
                DisplayReceivedQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.rd.ReceivedQty.HasValue ? p.rd.ReceivedQty.Value : 0),
                DisplayShelvesQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.rd.ShelvesQty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.rd.ShelvesQty

            }).Distinct().OrderBy(p => p.SkuName).ToList();
            return receiptDetails;
        }

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailViewListByCollectionLot(Guid receiptSysId)
        {
            var query = (from rd in Context.receiptdetails
                         join s in Context.skus on rd.SkuSysId equals s.SysId
                         join temp in Context.lottemplates on s.LotTemplateSysId equals temp.SysId
                         join p in Context.packs on s.PackSysId equals p.SysId into t2
                         from p1 in t2.DefaultIfEmpty()
                         where rd.ReceiptSysId == receiptSysId
                         group new { rd, s, p1, temp } by new { rd, s, p1, temp } into g
                         select new ReceiptDetailViewDto()
                         {
                             ReceiptSysId = g.Key.rd.ReceiptSysId,
                             SkuSysId = g.Key.rd.SkuSysId,
                             SkuCode = g.Key.s.SkuCode,
                             SkuName = g.Key.s.SkuName,
                             SkuUPC = g.Key.s.UPC,
                             UPC01 = g.Key.p1.UPC01,
                             UPC02 = g.Key.p1.UPC02,
                             UPC03 = g.Key.p1.UPC03,
                             UPC04 = g.Key.p1.UPC04,
                             UPC05 = g.Key.p1.UPC05,
                             FieldValue01 = g.Key.p1.FieldValue01,
                             FieldValue02 = g.Key.p1.FieldValue02,
                             FieldValue03 = g.Key.p1.FieldValue03,
                             FieldValue04 = g.Key.p1.FieldValue04,
                             FieldValue05 = g.Key.p1.FieldValue05,
                             LotTemplateDto = new LotTemplateDto()
                             {
                                 Lot01 = g.Key.temp.Lot01,
                                 LotVisible01 = g.Key.temp.LotVisible01,
                                 LotMandatory01 = g.Key.temp.LotMandatory01,
                                 LotType01 = g.Key.temp.LotType01,
                                 LotValue01 = g.Key.temp.LotValue01,
                                 DefaultIN01 = g.Key.temp.DefaultIN01,
                                 Lot02 = g.Key.temp.Lot02,
                                 LotVisible02 = g.Key.temp.LotVisible02,
                                 LotMandatory02 = g.Key.temp.LotMandatory02,
                                 LotType02 = g.Key.temp.LotType02,
                                 LotValue02 = g.Key.temp.LotValue02,
                                 DefaultIN02 = g.Key.temp.DefaultIN02,
                                 Lot03 = g.Key.temp.Lot03,
                                 LotVisible03 = g.Key.temp.LotVisible03,
                                 LotMandatory03 = g.Key.temp.LotMandatory03,
                                 LotType03 = g.Key.temp.LotType03,
                                 LotValue03 = g.Key.temp.LotValue03,
                                 DefaultIN03 = g.Key.temp.DefaultIN03,
                                 Lot04 = g.Key.temp.Lot04,
                                 LotVisible04 = g.Key.temp.LotVisible04,
                                 LotMandatory04 = g.Key.temp.LotMandatory04,
                                 LotType04 = g.Key.temp.LotType04,
                                 LotValue04 = g.Key.temp.LotValue04,
                                 DefaultIN04 = g.Key.temp.DefaultIN04,
                                 Lot05 = g.Key.temp.Lot05,
                                 LotVisible05 = g.Key.temp.LotVisible05,
                                 LotMandatory05 = g.Key.temp.LotMandatory05,
                                 LotType05 = g.Key.temp.LotType05,
                                 LotValue05 = g.Key.temp.LotValue05,
                                 DefaultIN05 = g.Key.temp.DefaultIN05,
                                 Lot06 = g.Key.temp.Lot06,
                                 LotVisible06 = g.Key.temp.LotVisible06,
                                 LotMandatory06 = g.Key.temp.LotMandatory06,
                                 LotType06 = g.Key.temp.LotType06,
                                 LotValue06 = g.Key.temp.LotValue06,
                                 DefaultIN06 = g.Key.temp.DefaultIN06,
                                 Lot07 = g.Key.temp.Lot07,
                                 LotVisible07 = g.Key.temp.LotVisible07,
                                 LotMandatory07 = g.Key.temp.LotMandatory07,
                                 LotType07 = g.Key.temp.LotType07,
                                 LotValue07 = g.Key.temp.LotValue07,
                                 DefaultIN07 = g.Key.temp.DefaultIN07,
                                 Lot08 = g.Key.temp.Lot08,
                                 LotVisible08 = g.Key.temp.LotVisible08,
                                 LotMandatory08 = g.Key.temp.LotMandatory08,
                                 LotType08 = g.Key.temp.LotType08,
                                 LotValue08 = g.Key.temp.LotValue08,
                                 DefaultIN08 = g.Key.temp.DefaultIN08,
                                 Lot09 = g.Key.temp.Lot09,
                                 LotVisible09 = g.Key.temp.LotVisible09,
                                 LotMandatory09 = g.Key.temp.LotMandatory09,
                                 LotType09 = g.Key.temp.LotType09,
                                 LotValue09 = g.Key.temp.LotValue09,
                                 DefaultIN09 = g.Key.temp.DefaultIN09,
                                 Lot10 = g.Key.temp.Lot10,
                                 LotVisible10 = g.Key.temp.LotVisible10,
                                 LotMandatory10 = g.Key.temp.LotMandatory10,
                                 Lot11 = g.Key.temp.Lot11,
                                 LotVisible11 = g.Key.temp.LotVisible11,
                                 LotMandatory11 = g.Key.temp.LotMandatory11,
                                 Lot12 = g.Key.temp.Lot12,
                                 LotVisible12 = g.Key.temp.LotVisible12,
                                 LotMandatory12 = g.Key.temp.LotMandatory12
                             }
                         }
                ).Distinct().OrderBy(p => p.SkuName);
            return query.ToList();
        }

        #region 库存转移
        /// <summary>
        /// 库存转移分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            var query = from invlotloclpn in Context.invlotloclpns
                        join invlot in Context.invlots on new { SkuSysId = invlotloclpn.SkuSysId, WareHouseSysId = invlotloclpn.WareHouseSysId, Lot = invlotloclpn.Lot } equals new { SkuSysId = invlot.SkuSysId, WareHouseSysId = invlot.WareHouseSysId, Lot = invlot.Lot }
                        join sku in Context.skus on invlot.SkuSysId equals sku.SysId
                        //join warehouse in Context.warehouses on invlot.WareHouseSysId equals warehouse.SysId
                        join pk in Context.packs on sku.PackSysId equals pk.SysId
                        where invlotloclpn.WareHouseSysId == request.WarehouseSysId
                        //where invlot.WareHouseSysId == invlotloclpn.WareHouseSysId
                        //    && invlot.Lot == invlotloclpn.Lot
                        //    && invlot.WareHouseSysId == request.WarehouseSysId
                        select new
                        {
                            invlot,
                            invlotloclpnSysId = invlotloclpn.SysId,
                            invlotloclpnQty = invlotloclpn.Qty,
                            AvailableQty = invlotloclpn.Qty - invlotloclpn.PickedQty - invlotloclpn.AllocatedQty - invlotloclpn.FrozenQty,
                            invlotloclpn.Loc,
                            sku.SkuName,
                            sku.SkuCode,
                            sku.SkuDescr,
                            sku.UPC,
                            //WarehouseName = warehouse.Name,
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
                //WarehouseName = p.WarehouseName,
                Loc = p.Loc,
                Lot = p.invlot.Lot,
                Qty = p.invlotloclpnQty,
                AvailableQty = p.AvailableQty,
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
                                  ? Math.Round(((p.FieldValue02.Value * p.invlotloclpnQty * 1.00m) / p.FieldValue01.Value), 3) : p.invlotloclpnQty,
                DisplayAvailableQty = p.InLabelUnit01.HasValue && p.InLabelUnit01.Value == true
                                  && p.FieldValue01 > 0 && p.FieldValue02 > 0
                                  ? Math.Round(((p.FieldValue02.Value * p.AvailableQty * 1.00m) / p.FieldValue01.Value), 3) : p.AvailableQty
            }).Distinct();

            request.iTotalDisplayRecords = query.Count();
            inventorys = inventorys.OrderBy(p => p.SkuName).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(inventorys, request);
        }
        #endregion

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<OutboundBoxDto> GetOutboundBox(Guid outboundSysId, Guid wareHouseSysId)
        {

            var strSql = $@"SELECT DISTINCT od.SkuSysId,od.Qty,s.SkuName,pbp.SysId as BoxSysId,pbp.TransferOrder as BoxName,IFNULL(pbp.Qty,0) AS CaseQty, p.SysId,p.FieldValue02,p.FieldValue03 FROM outbounddetail od
                                LEFT join
                                (select pbpd.SkuSysId,pbp.OutboundSysId,pbpd.Qty,pbp.SysId,pbp.TransferOrder from outboundtransferorderdetail pbpd
                                 left join outboundtransferorder pbp on pbp.SysId = pbpd.OutboundTransferOrderSysId
                                 where pbp.OutboundSysId = @OutboundSysId AND pbp.Status != @Status AND pbp.TransferType = {(int)OutboundTransferOrderType.Scattered}) pbp 
                                  on od.OutboundSysId = pbp.OutboundSysId AND od.SkuSysId = pbp.SkuSysId
                                LEFT JOIN sku s ON s.SysId = od.SkuSysId
                                LEFT JOIN pack p on p.SysId = s.PackSysId
                                WHERE od.OutboundSysId = @OutboundSysId
                                Order By od.SkuSysId";

            var outboundBoxList = base.Context.Database.SqlQuery<OutboundBoxDto>(strSql
                , new MySqlParameter("@OutboundSysId", outboundSysId)
                , new MySqlParameter("@Status", -999)).ToList();
            return outboundBoxList;
        }

        #endregion
    }
}
