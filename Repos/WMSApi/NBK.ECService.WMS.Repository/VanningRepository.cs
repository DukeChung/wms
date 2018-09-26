using System;
using System.Collections.Generic;
using System.Linq;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System.Text;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMS.Repository
{
    public class VanningRepository : CrudRepository, IVanningRepository
    {
        /// <param name="dbContextProvider"></param>
        public VanningRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        public List<VanningDeliveryDto> GetVanningPickDetailDtoByVanningSysId(Guid sysId, Guid wareHouseSysId)
        {
            var query = from vd in Context.vanningdetails
                        join vpd in Context.vanningpickdetails on vd.SysId equals vpd.VanningDetailSysId
                        join pd in Context.pickdetails on vpd.PickDetailSysId equals pd.SysId
                        join illl in Context.invlotloclpns on pd.SkuSysId equals illl.SkuSysId
                        join il in Context.invlots on pd.SkuSysId equals il.SkuSysId
                        join ils in Context.invskulocs on pd.SkuSysId equals ils.SkuSysId
                        where vd.VanningSysId == sysId && pd.Loc == illl.Loc && pd.Lot == illl.Lot && pd.Lpn == illl.Lpn &&
                              pd.Lot == il.Lot && pd.Loc == ils.Loc
                              && pd.WareHouseSysId == wareHouseSysId && illl.WareHouseSysId == wareHouseSysId && il.WareHouseSysId == wareHouseSysId && ils.WareHouseSysId == wareHouseSysId
                        select new VanningDeliveryDto
                        {
                            InvLotLocLpnSysId = illl.SysId,
                            InvLotSysId = il.SysId,
                            InvSkuLocSysId = ils.SysId,
                            OutboundSysId = pd.OutboundSysId,
                            OutboundDetailSysId = pd.OutboundDetailSysId,
                            PickDetailSysId = vpd.PickDetailSysId,
                            SkuSysId = vpd.SkuSysId,
                            Loc = vpd.Loc,
                            Lot = vpd.Lot,
                            Lpn = vpd.Lpn,
                            Qty = vpd.Qty
                        };
            return query.ToList();
        }

        public Pages<VanningDto> GetVanningList(VanningQueryDto vanningQueryDto)
        {
            var query = from v in Context.vannings
                        join o in Context.outbounds on v.OutboundSysId equals o.SysId
                        select new { v, o };

            query = query.Where(p => p.v.WarehouseSysId == vanningQueryDto.WarehouseSysId);
            if (!string.IsNullOrEmpty(vanningQueryDto.OutboundOrderSearch))
            {
                vanningQueryDto.OutboundOrderSearch = vanningQueryDto.OutboundOrderSearch.Trim();
                query = query.Where(p => p.o.OutboundOrder == vanningQueryDto.OutboundOrderSearch);
            }
            if (!string.IsNullOrEmpty(vanningQueryDto.VanningOrderSearch))
            {
                vanningQueryDto.VanningOrderSearch = vanningQueryDto.VanningOrderSearch.Trim();
                query = query.Where(p => p.v.VanningOrder.Contains(vanningQueryDto.VanningOrderSearch));
            }

            //出库单状态
            if (vanningQueryDto.OutboundStatusSearch.HasValue)
            {
                query = query.Where(p => p.o.Status == vanningQueryDto.OutboundStatusSearch);
            }

            var outboundIdList = new List<Guid>();
            var pickDetailQuery = from pd in Context.pickdetails join s in Context.skus on pd.SkuSysId equals s.SysId select new { pd, s };
            //拣货单号
            if (!string.IsNullOrEmpty(vanningQueryDto.PickDetailOrderSearch))
            {
                vanningQueryDto.PickDetailOrderSearch = vanningQueryDto.PickDetailOrderSearch.Trim();
                pickDetailQuery = pickDetailQuery.Where(x => x.pd.PickDetailOrder == vanningQueryDto.PickDetailOrderSearch);
            }

            //商品名称
            if (!string.IsNullOrEmpty(vanningQueryDto.SkuNameSearch))
            {
                vanningQueryDto.SkuNameSearch = vanningQueryDto.SkuNameSearch.Trim();
                pickDetailQuery = pickDetailQuery.Where(x => x.s.SkuName.Contains(vanningQueryDto.SkuNameSearch));
            }

            //商品UPC
            if (!string.IsNullOrEmpty(vanningQueryDto.SkuUPCSearch))
            {
                vanningQueryDto.SkuUPCSearch = vanningQueryDto.SkuUPCSearch.Trim();
                pickDetailQuery = pickDetailQuery.Where(x => x.s.UPC == vanningQueryDto.SkuUPCSearch);
            }

            if (!string.IsNullOrEmpty(vanningQueryDto.PickDetailOrderSearch) || !string.IsNullOrEmpty(vanningQueryDto.SkuNameSearch)
              || !string.IsNullOrEmpty(vanningQueryDto.SkuUPCSearch))
            {
                var outboundSysIds = pickDetailQuery.Select(p => p.pd.OutboundSysId).ToList();
                query = query.Where(x => outboundSysIds.Contains(x.o.SysId));
            }

            var vannings = query.Select(p => new VanningDto()
            {
                SysId = p.v.SysId,
                VanningOrder = p.v.VanningOrder,
                OutboundOrder = p.o.OutboundOrder,
                Status = p.v.Status,
                VanningType = p.v.VanningType,
                VanningDate = p.v.VanningDate
            }).Distinct();
            vanningQueryDto.iTotalDisplayRecords = vannings.Count();
            vannings = vannings.OrderByDescending(p => p.VanningDate)
                    .Skip(vanningQueryDto.iDisplayStart)
                    .Take(vanningQueryDto.iDisplayLength);
            return ConvertPages(vannings, vanningQueryDto);
        }

        public Pages<HandoverGroupDto> GetHandoverGroupByPage(HandoverGroupQuery request)
        {
            var query = from vd in Context.vanningdetails
                        join v in Context.vannings on vd.VanningSysId equals v.SysId
                        join outbound in Context.outbounds on v.OutboundSysId equals outbound.SysId
                        join carrier in Context.carriers on vd.CarrierSysId equals carrier.SysId into tempCarrier
                        from tt in tempCarrier.DefaultIfEmpty()
                        where vd.HandoverGroupOrder != null && vd.HandoverGroupOrder.Length > 0
                        select new
                        {
                            v.WarehouseSysId,
                            vd.HandoverCreateDate,
                            vd.HandoverGroupOrder,
                            vd.CarrierNumber,
                            vd.ContainerNumber,
                            CarrierName = tt == null ? "" : tt.CarrierName,
                            v.VanningOrder,
                            vd.Weight,
                            outbound.ExternOrderId,
                            vpQty = Context.vanningpickdetails.Where(p => p.VanningDetailSysId == vd.SysId).Sum(q => q.Qty)
                        };

            query = query.Where(p => p.WarehouseSysId == request.WarehouseSysId);

            if (!request.HandoverGroupOrder.IsNull())
            {
                request.HandoverGroupOrder = request.HandoverGroupOrder.Trim();
                query = query.Where(p => p.HandoverGroupOrder.Equals(request.HandoverGroupOrder, StringComparison.OrdinalIgnoreCase));
            }
            if (request.HandoverCreateDateFrom.HasValue)
            {
                query = query.Where(x => x.HandoverCreateDate > request.HandoverCreateDateFrom.Value);
            }
            if (request.HandoverCreateDateTo.HasValue)
            {
                query = query.Where(x => x.HandoverCreateDate < request.HandoverCreateDateTo.Value);
            }
            if (!request.ExternOrderId.IsNull())
            {
                request.ExternOrderId = request.ExternOrderId.Trim();
                query = query.Where(p => p.ExternOrderId.Equals(request.ExternOrderId, StringComparison.OrdinalIgnoreCase));
            }
            if (!request.VanningOrder.IsNull())
            {
                var vanningNumberArray = request.VanningOrder.Split('-');
                if (vanningNumberArray.Count() == 2)
                {
                    string vanningOrder = vanningNumberArray[0];
                    string containerNumber = vanningNumberArray[1];
                    query = query.Where(p => p.VanningOrder.Equals(vanningOrder, StringComparison.OrdinalIgnoreCase));
                    query = query.Where(p => p.ContainerNumber.Equals(containerNumber, StringComparison.OrdinalIgnoreCase));
                }

            }

            var handoverGroups = query.GroupBy(x => new
            {
                x.HandoverGroupOrder,
                x.CarrierName
            }).Select(p => new HandoverGroupDto()
            {
                HandoverGroupOrder = p.Key.HandoverGroupOrder,
                TotalCount = p.Sum(q => q.vpQty).Value,
                TotalWeight = p.Sum(q => q.Weight).Value,
                CarrierName = p.Key.CarrierName,
            });

            request.iTotalDisplayRecords = handoverGroups.Count();
            handoverGroups = handoverGroups.OrderByDescending(p => p.HandoverGroupOrder).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(handoverGroups, request);
        }

        public HandoverGroupDto GetHandoverGroupByOrder(string HandoverGroupOrder)
        {
            var query = from vd in Context.vanningdetails
                        join v in Context.vannings on vd.VanningSysId equals v.SysId
                        join carrier in Context.carriers on vd.CarrierSysId equals carrier.SysId into tempCarrier
                        from tt in tempCarrier.DefaultIfEmpty()
                        where vd.HandoverGroupOrder.Equals(HandoverGroupOrder, StringComparison.OrdinalIgnoreCase)
                        select new
                        {
                            vd.HandoverCreateDate,
                            vd.HandoverGroupOrder,
                            vd.CarrierNumber,
                            CarrierName = tt == null ? "" : tt.CarrierName,
                            v.VanningOrder,
                            vd.Weight,
                            vpQty = Context.vanningpickdetails.Where(p => p.VanningDetailSysId == vd.SysId).Sum(q => q.Qty)
                        };

            var handoverGroups = query.GroupBy(x => new
            {
                x.HandoverGroupOrder,
                x.CarrierName,
                x.HandoverCreateDate
            }).Select(p => new HandoverGroupDto()
            {
                HandoverGroupOrder = p.Key.HandoverGroupOrder,
                TotalCount = p.Sum(q => q.vpQty).Value,
                TotalWeight = p.Sum(q => q.Weight).Value,
                CarrierName = p.Key.CarrierName,
                HandoverCreateDate = p.Key.HandoverCreateDate
            });

            return handoverGroups.FirstOrDefault();
        }

        public List<HandoverGroupDetailDto> GetHandoverGroupDetailByOrder(string handoverGroupOrder)
        {
            var query = from vd in Context.vanningdetails
                        join v in Context.vannings on vd.VanningSysId equals v.SysId
                        join outbound in Context.outbounds on v.OutboundSysId equals outbound.SysId
                        where vd.HandoverGroupOrder == handoverGroupOrder
                        select new HandoverGroupDetailDto()
                        {
                            VanningOrder = v.VanningOrder,
                            ContainerNumber = vd.ContainerNumber,
                            CarrierNumber = vd.CarrierNumber,
                            Weight = vd.Weight.Value,
                            ExternOrderId = outbound.ExternOrderId
                        };
            return query.ToList();
        }

        public List<VanningRecordDto> GetVanningRecordByOrder(string orderNumber, Guid wareHouseSysId, pickdetail pickDetail)
        {
            var sql = new StringBuilder();
            if(pickDetail != null)
            {
                sql.Append(@"SELECT pd.OutboundSysId,
                                    (SELECT CONCAT(MAX(cast(ContainerNumber as SIGNED INTEGER)),'') FROM  vanningdetail 
                                    WHERE VanningSysId = v.SysId) as VanningNumber FROM pickdetail pd
                                    INNER JOIN vanning v ON v.OutboundSysId = pd.OutboundSysId
                                    WHERE pd.WareHouseSysId = @WareHouseSysId AND pd.PickDetailOrder = @orderNumber
                                    AND v.Status != @Status;");
            }
            else {
                sql.Append(@"SELECT o.SysId as OutboundSysId,
                                    (SELECT CONCAT(MAX(cast(ContainerNumber as SIGNED INTEGER)),'') FROM  vanningdetail 
                                    WHERE VanningSysId = v.SysId) as VanningNumber FROM outbound o 
                                    INNER JOIN vanning v ON v.OutboundSysId = o.SysId
                                    WHERE o.WareHouseSysId = @WareHouseSysId AND o.OutboundOrder = @orderNumber
                                    AND v.Status != @Status;");
            }
            
            var result = base.Context.Database.SqlQuery<VanningRecordDto>(sql.ToString()
                , new MySqlParameter("@WareHouseSysId", wareHouseSysId)
                , new MySqlParameter("@orderNumber", orderNumber)
                , new MySqlParameter("@Status", (int)VanningStatus.Cancel));
            return result.ToList();
        }

        public Pages<VanningDetailViewDto> GetVanningDetailViewListByPaging(VanningViewQuery vanningViewQuery)
        {
            var vanningDetailSku = from vd in (
                                   from vd in Context.vanningdetails
                                   join vpd in Context.vanningpickdetails on vd.SysId equals vpd.VanningDetailSysId
                                   where vd.VanningSysId == vanningViewQuery.VanningSysIdSearch
                                   group new { vd, vpd } by new { vd.SysId, vpd.SkuSysId } into g
                                   select new
                                   {
                                       SysId = g.Key.SysId,
                                       SkuSysId = g.Key.SkuSysId
                                   })
                                   group vd.SysId by new { vd.SysId } into g
                                   select new { SysId = g.Key.SysId, VannginSkuCount = g.Count() };

            var vanningDetailContainers = (from vd in Context.vanningdetails
                                           where vd.VanningSysId == vanningViewQuery.VanningSysIdSearch
                                           select new { ContainerNumber = vd.ContainerNumber }).ToList();

            var vanningDetailContainer = (from vd in vanningDetailContainers select new { ContainerNumber = int.Parse(vd.ContainerNumber) }).OrderByDescending(x => x.ContainerNumber).FirstOrDefault();

            var vanningDetails = (from vd in Context.vanningdetails
                                  join v in Context.vannings on vd.VanningSysId equals v.SysId
                                  join o in Context.outbounds on v.OutboundSysId equals o.SysId
                                  join vpd in vanningDetailSku on vd.SysId equals vpd.SysId
                                  join c in Context.carriers on vd.CarrierSysId equals c.SysId into t1
                                  where vd.VanningSysId == vanningViewQuery.VanningSysIdSearch
                                  from it1 in t1.DefaultIfEmpty()
                                  group new { vd, v, o, vpd, it1 } by new { vd, v, o, vpd, it1 } into g
                                  select new VanningDetailViewDto()
                                  {
                                      ContainerNumber = g.Key.vd.ContainerNumber,
                                      CarrierNumber = g.Key.vd.CarrierNumber,
                                      Weight = g.Key.vd.Weight,
                                      CarrierName = g.Key.it1.CarrierName,
                                      HandoverCreateDate = g.Key.vd.HandoverCreateDate,
                                      CreateDate = g.Key.vd.CreateDate,
                                      SysId = g.Key.vd.SysId,
                                      OutboundType = g.Key.o.OutboundType,
                                      ExternOrderId = g.Key.o.ExternOrderId,
                                      VanningOrderNumber = g.Key.v.VanningOrder + "-" + g.Key.vd.ContainerNumber,
                                      ConsigneePhone = g.Key.o.ConsigneePhone,
                                      VannginSkuCount = g.Key.vpd.VannginSkuCount,
                                      MaxContainerNumber = vanningDetailContainer.ContainerNumber,
                                      Marke = g.Key.vd.Marke,
                                  }).Distinct();

            vanningViewQuery.iTotalDisplayRecords = vanningDetails.Count();
            vanningDetails = vanningDetails.OrderByDescending(p => p.CreateDate).Skip(vanningViewQuery.iDisplayStart).Take(vanningViewQuery.iDisplayLength);
            return ConvertPages(vanningDetails, vanningViewQuery);
        }

        public List<VanningPickDetailDto> GetVanningPickDetailByOrder(string orderNumber, pickdetail pickDetail)
        {
            if (pickDetail != null)
            {
                var query = from pd in Context.pickdetails
                            join o in Context.outbounds on pd.OutboundSysId equals o.SysId
                            join v in Context.vannings on pd.OutboundSysId equals v.OutboundSysId
                            join vd in Context.vanningdetails on v.SysId equals vd.VanningSysId
                            join vpd in Context.vanningpickdetails on
                            new { PickDetailSysId = pd.SysId, VanningDetailSysId = vd.SysId } equals new { PickDetailSysId = vpd.PickDetailSysId.Value, VanningDetailSysId = vpd.VanningDetailSysId.Value }
                            where pd.PickDetailOrder == orderNumber
                            && v.Status != (int)VanningStatus.Cancel && vd.Status != (int)VanningStatus.Cancel
                            group vpd by new { vpd.SkuSysId } into g
                            select new VanningPickDetailDto()
                            {
                                SkuSysId = g.Key.SkuSysId,
                                Qty = g.Sum(p => p.Qty)
                            };
                return query.ToList();
            }
            else
            {
                var query = from pd in Context.pickdetails
                            join o in Context.outbounds on pd.OutboundSysId equals o.SysId
                            join v in Context.vannings on pd.OutboundSysId equals v.OutboundSysId
                            join vd in Context.vanningdetails on v.SysId equals vd.VanningSysId
                            join vpd in Context.vanningpickdetails on
                            new { PickDetailSysId = pd.SysId, VanningDetailSysId = vd.SysId } equals new { PickDetailSysId = vpd.PickDetailSysId.Value, VanningDetailSysId = vpd.VanningDetailSysId.Value }
                            where o.OutboundOrder == orderNumber
                            && v.Status != (int)VanningStatus.Cancel && vd.Status != (int)VanningStatus.Cancel
                            group vpd by new { vpd.SkuSysId } into g
                            select new VanningPickDetailDto()
                            {
                                SkuSysId = g.Key.SkuSysId,
                                Qty = g.Sum(p => p.Qty)
                            };
                return query.ToList();
            }
        }


        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CommonResponse CancelVanning(vanning model, int CurrentUserId, string CurrentDisplayName)
        {
            var rsp = new CommonResponse() { IsSuccess = false };
            try
            {
                var upOutbound = new StringBuilder();
                upOutbound.Append(@"UPDATE outbound o
                                            SET o.Status = @Status,o.TotalAllocatedQty = o.TotalPickedQty,
                                                o.TotalPickedQty = 0, o.UpdateBy =@UpdateBy, o.UpdateDate = NOW(),
                                                o.UpdateUserName = @UpdateUserName,
                                                o.TS = @TS
                                            WHERE o.SysId = @SysId
                                            AND o.Status = @PickingStatus;");

                var outCount = base.Context.Database.ExecuteSqlCommand(upOutbound.ToString()
                    , new MySqlParameter("@Status", (int)OutboundStatus.Allocation)
                    , new MySqlParameter("@UpdateBy", CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", CurrentDisplayName)
                    , new MySqlParameter("@TS", Guid.NewGuid())
                    , new MySqlParameter("@SysId", model.OutboundSysId)
                    , new MySqlParameter("@PickingStatus", (int)OutboundStatus.Picking));
                if (outCount < 1)
                {
                    throw new Exception("装箱单对应出库单不存在或装箱单对应的出库单状态不等于拣货完成，无法取消装箱");
                }

                var upOutboundDetail = new StringBuilder();
                upOutboundDetail.Append(@" UPDATE outbounddetail o 
                                                    SET o.Status=@Status,  o.PickedQty=0, o.UpdateBy=@UpdateBy,
                                                    o.UpdateDate=NOW(), o.UpdateUserName=@UpdateUserName
                                                    WHERE o.OutboundSysId=@OutboundSysId;");

                var outdCount = base.Context.Database.ExecuteSqlCommand(upOutboundDetail.ToString()
                    , new MySqlParameter("@Status", (int)OutboundDetailStatus.Allocation)
                    , new MySqlParameter("@UpdateBy", CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", CurrentDisplayName)
                    , new MySqlParameter("@OutboundSysId", model.OutboundSysId));
                if (outdCount < 1)
                {
                    throw new Exception("未找到出库单明细");
                }

                var upPickdetail = new StringBuilder();
                upPickdetail.Append(@"UPDATE pickdetail p
                                        SET p.PickDate = NOW(), p.Status = @Status, p.UpdateBy = @UpdateBy,
                                            p.UpdateDate = NOW(), p.UpdateUserName = @UpdateUserName
                                        WHERE p.OutboundSysId = @OutboundSysId
                                        AND p.Status = @FinishStatus;");
                var pickCount = base.Context.Database.ExecuteSqlCommand(upPickdetail.ToString()
                    , new MySqlParameter("@Status", (int)PickDetailStatus.New)
                    , new MySqlParameter("@UpdateBy", CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", CurrentDisplayName)
                    , new MySqlParameter("@OutboundSysId", model.OutboundSysId)
                    , new MySqlParameter("@FinishStatus", (int)PickDetailStatus.Finish));
                if (pickCount < 1)
                {
                    throw new Exception("未找到拣货明细");
                }


                var vdSql = new StringBuilder();
                vdSql.AppendFormat(@" UPDATE vanningdetail v 
                                      SET v.Status=@Status,  v.UpdateBy=@UpdateBy,  
                                      v.UpdateDate=NOW(), v.UpdateUserName=@UpdateUserName
                                      WHERE v.VanningSysId=@VanningSysId AND v.Status!=@Status;");

                var vdCount = base.Context.Database.ExecuteSqlCommand(vdSql.ToString()
                    , new MySqlParameter("@Status", (int)VanningStatus.Cancel)
                    , new MySqlParameter("@UpdateBy", CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", CurrentDisplayName)
                    , new MySqlParameter("@VanningSysId", model.SysId));
                if (vdCount < 1)
                {
                    throw new Exception("未找到装箱明细");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 取消装箱明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CommonResponse CancelVanningDetail(vanning model, int CurrentUserId, string CurrentDisplayName)
        {
            var rsp = new CommonResponse() { IsSuccess = false };
            try
            {
                var vdSql = new StringBuilder();
                vdSql.Append(@" UPDATE vanningdetail v 
                                      SET v.Status=@Status,  v.UpdateBy=@UpdateBy,  
                                      v.UpdateDate=NOW(), v.UpdateUserName=@UpdateUserName
                                      WHERE v.VanningSysId=@VanningSysId AND v.Status!=@Status;");

                var vdCount = base.Context.Database.ExecuteSqlCommand(vdSql.ToString()
                    , new MySqlParameter("@Status", (int)VanningStatus.Cancel)
                    , new MySqlParameter("@UpdateBy", CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", CurrentDisplayName)
                    , new MySqlParameter("@VanningSysId", model.SysId));
                if (vdCount < 1)
                {
                    throw new Exception("未找到装箱明细");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rsp;
        }
    }
}