using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class OutboundRepository : CrudRepository, IOutboundRepository
    {
        public OutboundRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request)
        {
            var query = from ob in Context.outbounds
                        join obDetail in Context.outbounddetails on ob.SysId equals obDetail.OutboundSysId
                        join s in Context.skus on obDetail.SkuSysId equals s.SysId
                        join ti in Context.transferinventorys on ob.SysId equals ti.TransferOutboundSysId into t1
                        from s1 in t1.DefaultIfEmpty()
                        select new
                        {
                            ob,
                            obDetail,
                            s,
                            s1
                        };

            #region 查询条件

            query = query.Where(x => x.ob.WareHouseSysId == request.WarehouseSysId);
            if (!string.IsNullOrEmpty(request.ExternOrderId))
            {
                request.ExternOrderId = request.ExternOrderId.Trim();
                query = query.Where(x => x.ob.ExternOrderId.Contains(request.ExternOrderId));
            }
            if (!string.IsNullOrEmpty(request.OutboundOrder))
            {
                request.OutboundOrder = request.OutboundOrder.Trim();
                query = query.Where(x => x.ob.OutboundOrder.Contains(request.OutboundOrder));
            }
            if (!string.IsNullOrEmpty(request.ConsigneeName))
            {
                request.ConsigneeName = request.ConsigneeName.Trim();
                query = query.Where(x => x.ob.ConsigneeName.Contains(request.ConsigneeName));
            }
            if (!string.IsNullOrEmpty(request.ConsigneePhone))
            {
                request.ConsigneePhone = request.ConsigneePhone.Trim();
                query = query.Where(x => x.ob.ConsigneePhone == request.ConsigneePhone);
            }
            if (!string.IsNullOrEmpty(request.ConsigneeAddress))
            {
                request.ConsigneeAddress = request.ConsigneeAddress.Trim();
                query = query.Where(x => (x.ob.ConsigneeProvince + x.ob.ConsigneeCity + x.ob.ConsigneeArea + x.ob.ConsigneeAddress).Contains(request.ConsigneeAddress));
            }
            if (request.CreateDateFrom.HasValue)
            {
                query = query.Where(x => x.ob.CreateDate > request.CreateDateFrom.Value);
            }
            if (request.CreateDateTo.HasValue)
            {
                query = query.Where(x => x.ob.CreateDate < request.CreateDateTo.Value);
            }
            if (request.ActualShipDateFrom.HasValue)
            {
                query = query.Where(x => x.ob.ActualShipDate.Value > request.ActualShipDateFrom.Value);
            }
            if (request.ActualShipDateTo.HasValue)
            {
                query = query.Where(x => x.ob.ActualShipDate.Value < request.ActualShipDateTo.Value);
            }
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.ob.Status == request.Status.Value);
            }
            if (request.AuditingDateFrom.HasValue)
            {
                query = query.Where(x => x.ob.AuditingDate.Value > request.AuditingDateFrom.Value);
            }
            if (request.AuditingDateTo.HasValue)
            {
                query = query.Where(x => x.ob.AuditingDate.Value < request.AuditingDateTo.Value);
            }
            if (request.OutboundType.HasValue)
            {
                query = query.Where(x => x.ob.OutboundType == request.OutboundType.Value);
            }
            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(x => x.s.SkuName == request.SkuName);
            }
            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(x => x.s.UPC == request.UPC);
            }
            if (!string.IsNullOrEmpty(request.SkuCode))
            {
                request.SkuCode = request.SkuCode.Trim();
                query = query.Where(x => x.s.SkuCode == request.SkuCode);
            }
            if (!string.IsNullOrEmpty(request.ToWareHouseSysId))
            {
                var toWareHouseSysId = Guid.Parse(request.ToWareHouseSysId);
                query = query.Where(x => x.s1.ToWareHouseSysId == toWareHouseSysId);
            }
            if (!string.IsNullOrEmpty(request.ServiceStationName))
            {
                request.ServiceStationName = request.ServiceStationName.Trim();
                query = query.Where(p => p.ob.ServiceStationName.Contains(request.ServiceStationName));
            }
            //if (!string.IsNullOrEmpty(request.CarrierNumber))
            //{
            //    query = query.Where(x => x.ob.CarrierNumber == request.CarrierNumber);
            //}
            if (request.IsMaterial.HasValue)
            {
                query = query.Where(p => p.s.IsMaterial == request.IsMaterial);
            }
            if (!string.IsNullOrEmpty(request.OutboundChildType))
            {
                request.OutboundChildType = request.OutboundChildType.Trim();
                query = query.Where(p => p.ob.OutboundChildType.Contains(request.OutboundChildType));
            }
            if (!string.IsNullOrEmpty(request.PurchaseOrder))
            {
                request.PurchaseOrder = request.PurchaseOrder.Trim();
                query = query.Where(p => p.ob.PurchaseOrder == request.PurchaseOrder);
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                var regionList = request.Region.Split(',');
                for (int i = 0; i < regionList.Length; i++)
                {
                    var region = regionList[i];
                    if (i == 0) query = query.Where(p => p.ob.ConsigneeProvince.Contains(region));
                    if (i == 1) query = query.Where(p => p.ob.ConsigneeCity.Contains(region));
                    if (i == 2) query = query.Where(p => p.ob.ConsigneeArea.Contains(region));
                    if (i == 3) query = query.Where(p => p.ob.ConsigneeTown.Contains(region));
                    if (i == 4) query = query.Where(p => p.ob.ConsigneeVillage.Contains(region));
                }
            }
            #endregion 查询条件

            var outbound = query.Select(x => new OutboundListDto()
            {
                SysId = x.ob.SysId,
                OutboundOrder = x.ob.OutboundOrder,
                Status = x.ob.Status.Value,
                OutboundType = x.ob.OutboundType.Value,
                OutboundChildType = x.ob.OutboundChildType,
                CreateDate = x.ob.CreateDate,
                ExternOrderId = x.ob.ExternOrderId,
                ExternOrderDate = x.ob.ExternOrderDate.Value,
                AuditingDate = x.ob.AuditingDate,
                ActualShipDate = x.ob.ActualShipDate,
                ConsigneeName = x.ob.ConsigneeName,
                ConsigneeArea = x.ob.ConsigneeArea,
                ConsigneeProvince = x.ob.ConsigneeProvince,
                ConsigneeCity = x.ob.ConsigneeCity,
                ConsigneeAddress = x.ob.ConsigneeAddress,
                TotalQty = x.ob.TotalQty.Value,
                Remark = x.ob.Remark,
                ServiceStationName = x.ob.ServiceStationName
            }).Distinct();

            var obdetailQuery = from baseQuery in query
                                group baseQuery by new { baseQuery.ob.SysId, baseQuery.ob.OutboundOrder };

            request.iTotalDisplayRecords = outbound.Count();
            outbound = outbound.OrderByDescending(p => p.AuditingDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages<OutboundListDto>(outbound, request);
        }

        public List<OutboundListDto> GetOutboundDetailBySummary(List<Guid> SysIds)
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
                             ? Math.Round(((p1.FieldValue02.Value * (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0)
                        })
                        group new { od } by new { od.OutboundSysId } into g
                        select new OutboundListDto
                        {
                            SysId = (Guid)g.Key.OutboundSysId,
                            DisplayTotalQty = g.Sum(x => x.od.DisplayTotalQty)
                        };

            return query.ToList();
        }

        public OutboundViewDto GetOutboundBySysId(Guid outboundSysId)
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
                            DetailedAddress = (ob.ConsigneeProvince ?? string.Empty) + (ob.ConsigneeCity ?? string.Empty) + (ob.ConsigneeArea ?? string.Empty) + (ob.ConsigneeAddress ?? string.Empty),
                            Remark = ob.Remark,
                            ServiceStationName = ob.ServiceStationName,
                            PrePackOrder = p0.PrePackOrder,
                            FromWareHouseName = tfinfo.FromWareHouseName,
                            ToWareHouseName = tfinfo.ToWareHouseName,
                            TransferInventoryOrder = tfinfo.TransferInventoryOrder,
                            ReceiptSysId = ob.ReceiptSysId,
                            PurchaseOrder = ob.PurchaseOrder
                        };

            return query.FirstOrDefault();
        }

        public List<OutboundDetailViewDto> GetOutboundDetails(Guid outboundSysId)
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
                    UPC = s.UPC,
                    SkuName = s.SkuName,
                    SkuDescr = s.SkuDescr,
                    Qty = obDetail.Qty.Value,
                    PackFactor = obDetail.PackFactor,
                    DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                ? Math.Round(((p1.FieldValue02.Value * (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (obDetail.Qty.HasValue ? obDetail.Qty.Value : 0),
                    UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? u2.UOMCode : u.UOMCode,
                    SkuSysId = s.SysId
                };

            return query.ToList();
        }

        /// <summary>
        /// 发货箱子信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public List<ScanDeliveryDto> GetDeliveryBoxByByOrderNumber(string type, string orderNumber, Guid wareHouseSysId)
        {
            var query =
                from v in Context.vannings
                join vd in Context.vanningdetails on v.SysId equals vd.VanningSysId
                join o in Context.outbounds on v.OutboundSysId equals o.SysId
                join c in Context.carriers on vd.CarrierSysId equals c.SysId into x
                from y in x.DefaultIfEmpty()
                where v.Status != (int)VanningStatus.Cancel && vd.Status != (int)VanningStatus.Cancel
                select new ScanDeliveryDto
                {
                    WarehouseSysId = v.WarehouseSysId,
                    VanningOrder = v.VanningOrder,
                    OutboundSysId = v.OutboundSysId.Value,
                    VanningSysId = v.SysId,
                    CarrierSysId = vd.CarrierSysId.Value,
                    CarrierName = y.CarrierName,
                    CarrierNumber = vd.CarrierNumber,
                    ContainerNumber = vd.ContainerNumber,
                    Weight = vd.Weight.Value,
                    OutboundStatus = o.Status
                };
            if (type == "VanningOrder")
            {
                var number = orderNumber.Split('-')[0];
                query = query.Where(x => x.VanningOrder == number);
                query = query.Where(x => x.WarehouseSysId == wareHouseSysId);
            }
            else
            {
                var detail = from o in Context.vanningdetails
                             join v in Context.vannings on o.VanningSysId equals v.SysId
                             where o.CarrierNumber == orderNumber && v.WarehouseSysId == wareHouseSysId
                             && o.Status != (int)VanningStatus.Cancel && v.Status != (int)VanningStatus.Cancel
                             select new { o.VanningSysId };
                var vanningDetails = detail.FirstOrDefault();

                if (vanningDetails != null)
                {
                    query = query.Where(x => x.VanningSysId == vanningDetails.VanningSysId);
                }
                else
                {
                    return new List<ScanDeliveryDto>();
                }

            }
            query = query.OrderBy(x => x.ContainerNumber);
            return query.ToList();
        }

        /// <summary>
        /// 快速发货检查
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public bool CheckDeliveryIntercept(Guid outboundSysId)
        {
            //var  query =" select Count(*) from outboundDetail od  "
            return true;

        }

        public List<PurchaseDetailReturnDto> GetPurchasedetailForOutboundReturn(Guid outboundSysId, Guid purchaseSysId,long userID,string userName)
        {
            var query = from outbounddetail in Context.outbounddetails
                        join sku in Context.skus on outbounddetail.SkuSysId equals sku.SysId
                        join pack in Context.packs on outbounddetail.PackSysId equals pack.SysId
                        join uom in Context.uoms on outbounddetail.UOMSysId equals uom.SysId
                        where outbounddetail.OutboundSysId == outboundSysId
                        select new PurchaseDetailReturnDto
                        {
                            PurchaseSysId = purchaseSysId,
                            SkuSysId = outbounddetail.SkuSysId,
                            SkuClassSysId = sku.SkuClassSysId,
                            UomCode = uom.UOMCode,
                            UOMSysId = uom.SysId,
                            PackCode = pack.PackCode,
                            PackSysId = pack.SysId,
                            Qty = outbounddetail.ShippedQty.Value,
                            ReceivedQty = 0,
                            RejectedQty = 0,
                            PurchasePrice = outbounddetail.Price.Value,
                            OtherSkuId = sku.OtherId,
                            PackFactor = outbounddetail.PackFactor,
                            UpdateDate = DateTime.Now,
                            UpdateBy = userID,
                            UpdateUserName = userName
                        };

            return query.ToList();
        }

        public List<invlot> GetInvlotForOutboundCancel(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join invlot in Context.invlots
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Lot } equals new { invlot.WareHouseSysId, invlot.SkuSysId, invlot.Lot }
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select invlot;

            return query.ToList();
        }

        public List<invskuloc> GetInvskulocForOutboundCancel(Guid outboundSysId)
        {
            var query = from pickdetails in Context.pickdetails
                        join invskuloc in Context.invskulocs
                            on new { pickdetails.WareHouseSysId, pickdetails.SkuSysId, pickdetails.Loc } equals new { invskuloc.WareHouseSysId, invskuloc.SkuSysId, invskuloc.Loc }
                        where pickdetails.OutboundSysId == outboundSysId
                            && pickdetails.Status == (int)PickDetailStatus.Finish
                        select invskuloc;

            return query.ToList();
        }

        public List<invlotloclpn> GetInvlotloclpnForOutboundCancel(Guid outboundSysId)
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

        /// <summary>
        /// 获取库存不足商品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public List<InsufficientStockSkuListDto> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto)
        {
            var outboundSysIds = string.Empty;
            if (dto.Type == 1)
            {
                outboundSysIds = "'" + string.Join("','", dto.OutboundSysIdList.ToArray()) + "'";
            }
            else
            {
                outboundSysIds = "'" + dto.SysId.ToString() + "'";
            }

            var sql = new StringBuilder();
            sql.AppendFormat(@"SELECT A.SkuSysId,A.SkuName,A.SkuCode,A.UPC,
                               A.OutboundQty, A.AllocatedQty, A.PickedQty, A.FrozenQty,
                                A.StockQty,(A.OutboundQty - A.StockQty) AS DiffQty FROM (
                                SELECT  s.SkuName, s.SkuCode,s.UPC, 
                                IFNULL(SUM(o.Qty),0) AS OutboundQty,
                                o.SkuSysId,
                                IFNULL(inv.AllocatedQty,0) AS AllocatedQty,
                                IFNULL(inv.PickedQty,0) AS PickedQty,
                                IFNULL(inv.FrozenQty,0) AS FrozenQty,
                                IFNULL(inv.StockQty,0) AS StockQty,
                                inv.LotAttr01
                                FROM outbounddetail o
                                LEFT JOIN outbound o1 ON o.OutboundSysId=o1.SysId
                                LEFT JOIN sku s ON o.SkuSysId = s.SysId
                                LEFT JOIN (SELECT  i.SkuSysId,
                                SUM(AllocatedQty) AS AllocatedQty, 
                                SUM(PickedQty) AS PickedQty, 
                                SUM(FrozenQty) AS FrozenQty,
                                SUM(i.Qty - i.AllocatedQty - i.PickedQty - i.FrozenQty) AS StockQty,i.LotAttr01
                                FROM invlot i  WHERE i.WareHouseSysId = @WareHouseSysId
                                GROUP BY i.SkuSysId,i.LotAttr01) inv
                                ON o.SkuSysId= inv.SkuSysId AND o1.Channel=inv.LotAttr01
                                WHERE o.OutboundSysId IN ({0}) 
                                GROUP BY  o.SkuSysId) A 
                                WHERE A.OutboundQty >A.StockQty;", outboundSysIds);

            var result = base.Context.Database.SqlQuery<InsufficientStockSkuListDto>(sql.ToString(),
               new MySqlParameter("@WareHouseSysId", dto.WarehouseSysId));
            return result.ToList();
        }

        public void BatchUpdateSNListForOutbound(List<string> SNList, Guid warehouseSysId, Guid outboundSysId, long userID, string userName)
        {
            if (SNList != null && SNList.Count > 0)
            {
                List<MySqlParameter> paraList = new List<MySqlParameter>();

                string sql =
                    $@"UPDATE receiptsn
                        SET Status = {(int)ReceiptSNStatus.Outbound},
                            OutboundSysId =@OutboundSysId ,
                            UpdateBy =@UpdateBy ,
                            UpdateDate = NOW(),
                            UpdateUserName =@UpdateUserName
                        WHERE SN IN (";

                int i = 0;
                SNList.ForEach(p =>
                {
                    sql += $"@sn{i},";
                    paraList.Add(new MySqlParameter($"@sn{i}", p));
                    i++;
                });
                sql = sql.TrimEnd(',');
                sql += $@")
                            AND WarehouseSysId =@WarehouseSysId
                            AND Status = {(int)ReceiptSNStatus.Receive};";

                paraList.Add(new MySqlParameter("@WareHouseSysId", warehouseSysId));
                paraList.Add(new MySqlParameter("@UpdateBy", userID));
                paraList.Add(new MySqlParameter("@UpdateUserName", userName));
                paraList.Add(new MySqlParameter("@OutboundSysId", outboundSysId));

                int result = base.Context.Database.ExecuteSqlCommand(sql, paraList.ToArray());
                if (result != SNList.Count)
                {
                    throw new Exception("有SN已被发货，请重新扫描SN发货!");
                }
            }
        }

        public void CancelReceiptsnByOutbound(Guid outboundSysId, long userID, string userName)
        {
            string sql =
                    $@"UPDATE receiptsn
                        SET Status = {(int)ReceiptSNStatus.Receive},
                            OutboundSysId = null,
                            UpdateBy =@UpdateBy,
                            UpdateDate = NOW(),
                            UpdateUserName = @UpdateUserName 
                        WHERE OutboundSysId =@OutboundSysId ;";

            base.Context.Database.ExecuteSqlCommand(sql
                , new MySqlParameter("@UpdateBy", userID)
                , new MySqlParameter("@UpdateUserName", userName)
                , new MySqlParameter("@OutboundSysId", outboundSysId));
        }

        public outbound GetOutboundInfoBySysId(Guid sysId)
        {
            string sql = "SELECT * FROM outbound o WHERE o.SysId = @SysId;";
            var outbound = base.Context.Database.SqlQuery<outbound>(sql, new MySqlParameter("@SysId", sysId)).ToList().FirstOrDefault();
            if (outbound != null)
            {
                string detailSql = "SELECT * FROM outbounddetail o WHERE o.OutboundSysId = @SysId;";
                outbound.outbounddetails = base.Context.Database.SqlQuery<outbounddetail>(detailSql, new MySqlParameter("@SysId", sysId)).ToList();
            }
            return outbound;
        }

        public List<PartShipmentDetailDto> GetPartShipmentSkuList(OutboundAllocationDeliveryDto dto)
        {
            var query = from od in Context.outbounddetails
                        join s in Context.skus on od.SkuSysId equals s.SysId
                        join pd in Context.pickdetails on od.SysId equals pd.OutboundDetailSysId
                        where od.OutboundSysId == dto.SysId
                        group new { od.SysId, od.SkuSysId, s.UPC, s.SkuName, od.Qty, pd } by new { od.SysId, od.SkuSysId, s.UPC, s.SkuName, od.Qty } into g
                        select new PartShipmentDetailDto
                        {
                            SysId = g.Key.SysId,
                            SkuSysId = g.Key.SkuSysId,
                            SkuUPC = g.Key.UPC,
                            SkuName = g.Key.SkuName,
                            Qty = (int)g.Key.Qty,
                            PickedQty = g.Sum(p => p.pd.PickedQty),
                            Memo = string.Empty
                        };
            return query.ToList();
        }

        public void CancelOutboundReturnByPurchase(PurchaseForReturnDto request)
        {
            var strSql = new StringBuilder();
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            var i = 1;

            foreach (var info in request.purchasedetails)
            {
                strSql.Append($@"
                    UPDATE outbounddetail o
                    SET o.ReturnQty = o.ReturnQty - @ReturnQty{i},
                        o.UpdateBy = @UpdateBy{i},
                        o.UpdateDate = NOW(),
                        o.UpdateUserName = @UpdateUserName{i}
                    WHERE o.OutboundSysId = @OutboundSysId{i}
                      AND o.SkuSysId = @SkuSysId{i}
                      AND o.ReturnQty >= @ReturnQty{i};
                ");

                parameters.Add(new MySqlParameter($"@ReturnQty{i}", info.Qty - info.ReceivedQty));
                parameters.Add(new MySqlParameter($"@SkuSysId{i}", info.SkuSysId));
                parameters.Add(new MySqlParameter($"@UpdateBy{i}", request.UpdateBy));
                parameters.Add(new MySqlParameter($"@UpdateUserName{i}", request.UpdateUserName));
                parameters.Add(new MySqlParameter($"@OutboundSysId{i}", request.OutboundSysId));
                i++;
            }
            int result = base.Context.Database.ExecuteSqlCommand(strSql.ToString(), parameters.ToArray());
            if (result != request.purchasedetails.Count)
            {
                throw new Exception($"退货数量更新异常，请检查!");
            }
        }
    }
}
