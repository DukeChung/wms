using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class RFOutboundRepository : CrudRepository, IRFOutboundRepository
    {
        public RFOutboundRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public RFWaitingReviewDto GetWaitingReviewList(RFOutboundQuery outboundQuery)
        {
            RFWaitingReviewDto rsp = new RFWaitingReviewDto();
            var waitingReviewList = from od in Context.outbounddetails
                                    join o in Context.outbounds on od.OutboundSysId equals o.SysId
                                    join s in Context.skus on od.SkuSysId equals s.SysId
                                    join p in Context.packs on s.PackSysId equals p.SysId into t
                                    from ti in t.DefaultIfEmpty()
                                    join u1 in Context.uoms on ti.FieldUom01 equals u1.SysId into t1
                                    from ti1 in t1.DefaultIfEmpty()
                                    join u2 in Context.uoms on ti.FieldUom02 equals u2.SysId into t2
                                    from ti2 in t2.DefaultIfEmpty()
                                    where o.OutboundOrder == outboundQuery.OutboundOrder && o.WareHouseSysId == outboundQuery.WarehouseSysId
                                    select new RFOutboundDetailDto
                                    {
                                        SkuSysId = s.SysId,
                                        UPC = s.UPC,
                                        SkuName = s.SkuName,
                                        SkuQty = od.Qty.Value,
                                        DisplaySkuQty = ti.InLabelUnit01.HasValue && ti.InLabelUnit01.Value == true && ti.FieldValue01 > 0 && ti.FieldValue02 > 0
                                                            ? Math.Round(((ti.FieldValue02.Value * od.Qty.Value * 1.00m) / ti.FieldValue01.Value), 3) : od.Qty.Value,
                                        UPC01 = ti.UPC01,
                                        UPC02 = ti.UPC02,
                                        UPC03 = ti.UPC03,
                                        UPC04 = ti.UPC04,
                                        UPC05 = ti.UPC05,
                                        FieldValue01 = ti.FieldValue01,
                                        FieldValue02 = ti.FieldValue02,
                                        FieldValue03 = ti.FieldValue03,
                                        FieldValue04 = ti.FieldValue04,
                                        FieldValue05 = ti.FieldValue05
                                    };
            rsp.WaitingReviewList = waitingReviewList.OrderBy(p => p.UPC).ToList();
            return rsp;
        }

        /// <summary>
        ///  获取预包装明细
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        public List<RFPrePackDetailList> GetPrePackDetailList(RFPrePackQuery prePackQuery)
        {
            var rfPrePackDetailList = from pd in Context.prepackdetails
                                      join p in Context.prepacks on pd.PrePackSysId equals p.SysId
                                      join s in Context.skus on pd.SkuSysId equals s.SysId

                                      join pack in Context.packs on s.PackSysId equals pack.SysId into t1
                                      from p1 in t1.DefaultIfEmpty()
                                      join u in Context.uoms on p1.FieldUom01 equals u.SysId into t2
                                      from ut2 in t2.DefaultIfEmpty()
                                      join u1 in Context.uoms on p1.FieldUom02 equals u1.SysId into t3
                                      from ti3 in t3.DefaultIfEmpty()

                                      where p.StorageLoc == prePackQuery.StorageLoc && p.WareHouseSysId == prePackQuery.WarehouseSysId && p.Status == (int)PrePackStatus.New
                                      select new RFPrePackDetailList()
                                      {
                                          SysId = pd.SysId,
                                          SkuSysId = pd.SkuSysId,
                                          UPC = s.UPC,
                                          SkuName = s.SkuName,
                                          PreQty = pd.PreQty,
                                          Qty = pd.Qty.HasValue ? pd.Qty.Value : 0,
                                          UOMCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 ? ti3.UOMCode : ut2.UOMCode,
                                          UPC01 = p1.UPC01,
                                          UPC02 = p1.UPC02,
                                          UPC03 = p1.UPC03,
                                          UPC04 = p1.UPC04,
                                          UPC05 = p1.UPC05,
                                          FieldValue01 = p1.FieldValue01,
                                          FieldValue02 = p1.FieldValue02,
                                          FieldValue03 = p1.FieldValue03,
                                          FieldValue04 = p1.FieldValue04,
                                          FieldValue05 = p1.FieldValue05
                                      };

            return rfPrePackDetailList.ToList();
        }

        public List<RFPreBulkPackDetailDto> GetPreBulkPackDetailsByStorageCase(string storageCase, Guid warehouseSysId)
        {
            var query = from pd in Context.prebulkpackdetail
                        join p in Context.prebulkpack on pd.PreBulkPackSysId equals p.SysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join pa in Context.packs on pd.PackSysId equals pa.SysId
                        join u in Context.uoms on pa.FieldUom01 equals u.SysId into t
                        from ti in t.DefaultIfEmpty()
                        join u1 in Context.uoms on pa.FieldUom02 equals u1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        where p.StorageCase == storageCase && p.WareHouseSysId == warehouseSysId
                        select new RFPreBulkPackDetailDto
                        {
                            SkuSysId = pd.SkuSysId,
                            UPC = s.UPC,
                            SkuName = s.SkuName,
                            Qty = pd.Qty,
                            PreQty = pd.PreQty,
                            UOMCode = pa.InLabelUnit01.HasValue && pa.InLabelUnit01.Value == true && pa.FieldValue01 > 0 && pa.FieldValue02 > 0 ? ti1.UOMCode : ti.UOMCode,
                            UpdateDate = pd.UpdateDate,
                            UPC01 = pa.UPC01,
                            UPC02 = pa.UPC02,
                            UPC03 = pa.UPC03,
                            UPC04 = pa.UPC04,
                            UPC05 = pa.UPC05,
                            FieldValue01 = pa.FieldValue01,
                            FieldValue02 = pa.FieldValue02,
                            FieldValue03 = pa.FieldValue03,
                            FieldValue04 = pa.FieldValue04,
                            FieldValue05 = pa.FieldValue05
                        };
            return query.OrderByDescending(p => p.UpdateDate).ToList();
        }

        public List<RFPreBulkPackDetailDto> GetSkuByUPC(string upc)
        {
            var query = from s in Context.skus
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where s.UPC == upc
                        select new RFPreBulkPackDetailDto
                        {
                            SkuSysId = s.SysId,
                            UPC = s.UPC,
                            SkuName = s.SkuName,
                            UPC01 = ti.UPC01,
                            UPC02 = ti.UPC02,
                            UPC03 = ti.UPC03,
                            UPC04 = ti.UPC04,
                            UPC05 = ti.UPC05,
                            FieldValue01 = ti.FieldValue01,
                            FieldValue02 = ti.FieldValue02,
                            FieldValue03 = ti.FieldValue03,
                            FieldValue04 = ti.FieldValue04,
                            FieldValue05 = ti.FieldValue05
                        };
            return query.ToList();
        }

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        public List<SkuPackDto> GetSkuPackListByUPC(string upc)
        {
            var query = from s in Context.skus
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where s.UPC == upc
                        select new SkuPackDto
                        {
                            SkuSysId = s.SysId,
                            UPC = s.UPC,
                            SkuName = s.SkuName,
                            UPC01 = ti.UPC01,
                            UPC02 = ti.UPC02,
                            UPC03 = ti.UPC03,
                            UPC04 = ti.UPC04,
                            UPC05 = ti.UPC05,
                            FieldValue01 = ti.FieldValue01,
                            FieldValue02 = ti.FieldValue02,
                            FieldValue03 = ti.FieldValue03,
                            FieldValue04 = ti.FieldValue04,
                            FieldValue05 = ti.FieldValue05
                        };
            return query.ToList();
        }

        public void InsertOrUpdatePreBulkPackDetail(prebulkpack preBulkPack, RFPreBulkPackDetailDto preBulkPackDetailDto, sku sku, uom uom)
        {
            string insertTemplate = @"UPDATE prebulkpack p SET p.Status =@Status WHERE p.SysId =@PreBulkPackSysId  AND NOT EXISTS (SELECT 1  FROM prebulkpackdetail p1 WHERE p1.PreBulkPackSysId =@PreBulkPackSysId );
                                    INSERT INTO prebulkpackdetail(SysId, PreBulkPackSysId, SkuSysId, UOMSysId, PackSysId, Qty, CreateBy, CreateDate, UpdateBy, UpdateDate, CreateUserName, UpdateUserName)
                                    SELECT @SysId,@PreBulkPackSysId ,@SkuSysId ,@UOMSysId ,@PackSysId ,@Qty,@CurrentUserId, NOW(), @CurrentUserId, NOW(), @CurrentDisplayName, @CurrentDisplayName
                                    FROM dual WHERE NOT EXISTS(SELECT * FROM prebulkpackdetail p WHERE p.PreBulkPackSysId = @PreBulkPackSysId  AND p.SkuSysId =@SkuSysId );";

            var insertRows = Context.Database.ExecuteSqlCommand(insertTemplate
                , new MySqlParameter("@SysId", Guid.NewGuid())
                , new MySqlParameter("@PreBulkPackSysId", preBulkPack.SysId)
                , new MySqlParameter("@SkuSysId", sku.SysId)
                , new MySqlParameter("@UOMSysId", uom.SysId)
                , new MySqlParameter("@PackSysId", sku.PackSysId)
                , new MySqlParameter("@Qty", preBulkPackDetailDto.Qty)
                , new MySqlParameter("@CurrentDisplayName", preBulkPackDetailDto.CurrentDisplayName)
                , new MySqlParameter("@CurrentUserId", preBulkPackDetailDto.CurrentUserId)
                , new MySqlParameter("@Status", (int)PreBulkPackStatus.PrePack));

            if (insertRows == 0)
            {
                string updateTemplate = @"UPDATE prebulkpackdetail p SET p.Qty = p.Qty + {0}, p.UpdateBy =@UpdateBy, p.UpdateDate = NOW(), p.UpdateUserName =@UpdateUserName WHERE p.PreBulkPackSysId =@PreBulkPackSysId AND p.SkuSysId = @SkuSysId";
                string updateSql = string.Format(updateTemplate, preBulkPackDetailDto.Qty);

                Context.Database.ExecuteSqlCommand(updateSql
                    , new MySqlParameter("@UpdateBy", preBulkPackDetailDto.CurrentUserId)
                    , new MySqlParameter("@UpdateUserName", preBulkPackDetailDto.CurrentDisplayName)
                    , new MySqlParameter("@PreBulkPackSysId", preBulkPack.SysId)
                    , new MySqlParameter("@SkuSysId", sku.SysId));
            }
        }

        /// <summary>
        /// 查询出库明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<RFOutboundDetailDto> GetOutboundDetailList(string outboundOrder, Guid warehouseSysId)
        {
            var outboundDetails = from od in Context.outbounddetails
                                  join o in Context.outbounds on od.OutboundSysId equals o.SysId
                                  join s in Context.skus on od.SkuSysId equals s.SysId
                                  join p in Context.packs on s.PackSysId equals p.SysId into t1
                                  from ti1 in t1.DefaultIfEmpty()
                                  join u1 in Context.uoms on ti1.FieldUom01 equals u1.SysId into t2
                                  from ti2 in t2.DefaultIfEmpty()
                                  join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                                  from ti3 in t3.DefaultIfEmpty()
                                  where o.OutboundOrder == outboundOrder && o.WareHouseSysId == warehouseSysId
                                  select new RFOutboundDetailDto
                                  {
                                      SkuSysId = s.SysId,
                                      OutboundSysId = od.OutboundSysId,
                                      UPC = s.UPC,
                                      SkuName = s.SkuName,
                                      SkuQty = od.Qty.Value,
                                      UOMCode = ti1.InLabelUnit01.HasValue && ti1.InLabelUnit01.Value == true
                                      && ti1.FieldValue01 > 0 && ti1.FieldValue02 > 0 ? ti3.UOMCode : ti2.UOMCode
                                  };
            return outboundDetails.OrderBy(p => p.UPC).ToList();
        }

        /// <summary>
        /// 根据出库单ID和商品ID获取散货封箱已装箱数量
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        /// <param name="skuSysId">商品ID</param>
        /// <returns></returns>
        public int GetPreBulkOutboundQty(Guid outboundSysId, Guid skuSysId)
        {
            var result = 0;
            var query = (from pre in Context.prebulkpack
                         join pred in Context.prebulkpackdetail on pre.SysId equals pred.PreBulkPackSysId
                         where pre.OutboundSysId == outboundSysId && pred.SkuSysId == skuSysId
                         select new
                         {
                             pred.Qty
                         }).AsQueryable().ToList();
            result = query.Sum(x => x.Qty);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<RFOutboundTransferOrderReviewDiffDto> GetTransferOrderReviewDiffList(RFOutboundTransferOrderQuery query)
        {
            var sql = new StringBuilder();
            sql.Append(@" SELECT  od.SkuSysId,
                                  s.SkuName,
                                  s.UPC,
                                  IFNULL(SUM(od.Qty), 0) AS OutboundQty,
                                  IFNULL(r.ReviewQty, 0) ReviewQty
                                FROM outbound o
                                  LEFT JOIN outbounddetail od
                                    ON o.SysId = od.OutboundSysId

                                  LEFT JOIN (SELECT
                                      o1.SkuSysId,
                                      IFNULL(SUM(o1.Qty), 0) AS ReviewQty
                                    FROM outboundtransferorder o
                                      LEFT JOIN outboundtransferorderdetail o1
                                        ON o.SysId = o1.OutboundTransferOrderSysId
                                    WHERE o.OutboundOrder = @OutboundOrder
                                    GROUP BY o1.SkuSysId) AS r
                                    ON od.SkuSysId = r.SkuSysId
                                 LEFT JOIN sku s
                                    ON od.SkuSysId = s.SysId
                                WHERE o.OutboundOrder = @OutboundOrder
                                GROUP BY od.SkuSysId;");

            return Context.Database.SqlQuery<RFOutboundTransferOrderReviewDiffDto>(sql.ToString()
                , new MySqlParameter("@OutboundOrder", query.OutboundOrder)).ToList();
        }

        /// <summary>
        /// 获取待复核的出库单
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        public Pages<RFOutboundReviewListDto> GetWaitingOutboundReviewListByPaging(RFOutboundQuery outboundQuery)
        {
            var query = from o in Context.outbounds
                        join pd in Context.pickdetails on o.SysId equals pd.OutboundSysId
                        where o.Status == (int)OutboundStatus.Allocation && pd.Status == (int)PickDetailStatus.New && o.WareHouseSysId == outboundQuery.WarehouseSysId && pd.PickedQty != 0
                        select new RFOutboundReviewListDto
                        {
                            SysId = o.SysId,
                            OutboundOrder = o.OutboundOrder,
                            ServiceStationName = o.ServiceStationName,
                            AuditingDate = o.AuditingDate
                        };
            var waitingContainerPickingList = query.Distinct();
            outboundQuery.iTotalDisplayRecords = waitingContainerPickingList.Count();
            waitingContainerPickingList = waitingContainerPickingList.OrderByDescending(p => p.AuditingDate).Skip(outboundQuery.iDisplayStart).Take(outboundQuery.iDisplayLength);
            var rsp = ConvertPages(waitingContainerPickingList, outboundQuery);
            if (rsp != null && rsp.TableResuls != null && rsp.TableResuls.aaData.Count > 0)
            {
                List<Guid> outboundSysIds = rsp.TableResuls.aaData.Select(p => p.SysId).ToList();
                var outboundDetailList = GetPickOutboundDetailListDto(outboundSysIds, outboundQuery.WarehouseSysId);
                foreach (var item in rsp.TableResuls.aaData)
                {
                    var detail = outboundDetailList.Find(x => x.OutboundSysId == item.SysId);
                    item.SkuQty = detail.SkuTypeQty.GetValueOrDefault();
                }
            }
            return rsp;
        }

        private List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid> outboundSysIds, Guid wareHouseSysId)
        {

            var query = from obd in Context.outbounddetails
                        where outboundSysIds.Contains(obd.OutboundSysId.Value)
                        group obd by new { obd.OutboundSysId }
                into g
                        select new PickOutboundDetailListDto()
                        {
                            OutboundSysId = g.Key.OutboundSysId,
                            SkuTypeQty = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取散货待复核明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public List<RFOutboundReviewDetailDto> GetWaitingSingleReviewDetails(RFOutboundQuery outboundQuery)
        {
            var query = from pbd in Context.prebulkpackdetail
                        join pb in Context.prebulkpack on pbd.PreBulkPackSysId equals pb.SysId
                        join obd in Context.outbounddetails on new { OutboundSysId = pb.OutboundSysId, SkuSysId = pbd.SkuSysId } equals new { OutboundSysId = obd.OutboundSysId, SkuSysId = obd.SkuSysId }
                        join s in Context.skus on pbd.SkuSysId equals s.SysId
                        where pb.OutboundOrder == outboundQuery.OutboundOrder && pb.WareHouseSysId == outboundQuery.WarehouseSysId
                        group new { pbd } by new { pbd.SkuSysId, s.SkuName, s.UPC, obd.Qty } into g
                        select new RFOutboundReviewDetailDto
                        {
                            SkuSysId = g.Key.SkuSysId,
                            SkuName = g.Key.SkuName,
                            UPC = g.Key.UPC,
                            OutboundQty = g.Key.Qty,
                            PickQty = g.Sum(p => p.pbd.Qty),
                            ReviewQty = 0
                        };
            return query.OrderByDescending(p => p.UPC).ToList();
        }
    }
}
