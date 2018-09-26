using System;
using System.Collections.Generic;
using System.Linq;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System.Text;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMS.Repository
{
    public class PreBulkPackRepository : CrudRepository, IPreBulkPackRepository
    {
        /// <param name="dbContextProvider"></param>
        public PreBulkPackRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<PreBulkPackDto> GetPreBulkPackByPage(PreBulkPackQuery request)
        {
            var query = from prebulkpack in Context.prebulkpack
                        join temp1 in Context.prebulkpackdetail on prebulkpack.SysId equals temp1.PreBulkPackSysId
                            into T1
                        from prebulkpackdetail in T1.DefaultIfEmpty()
                        join temp2 in Context.skus on prebulkpackdetail.SkuSysId equals temp2.SysId
                            into T2
                        from sku in T2.DefaultIfEmpty()
                        select new { prebulkpack, prebulkpackdetail, sku };

            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.sku.SkuName.Contains(request.SkuName));
            }

            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.sku.UPC.Equals(request.UPC, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Status.HasValue)
            {
                if (request.Status.Value == (int)PreBulkPackStatus.PrePack)
                {
                    query = query.Where(p => p.prebulkpack.Status == request.Status.Value || p.prebulkpack.Status == (int)PreBulkPackStatus.RFPicking);
                }
                else
                {
                    query = query.Where(p => p.prebulkpack.Status == request.Status.Value);
                }
            }

            if (!string.IsNullOrEmpty(request.StorageCase))
            {
                request.StorageCase = request.StorageCase.Trim();
                query = query.Where(p => p.prebulkpack.StorageCase.Contains(request.StorageCase));
            }
            if (!string.IsNullOrEmpty(request.OutboundOrder))
            {
                request.OutboundOrder = request.OutboundOrder.Trim();
                query = query.Where(p => p.prebulkpack.OutboundOrder == request.OutboundOrder);
            }

            query = query.Where(p => p.prebulkpack.WareHouseSysId == request.WarehouseSysId);

            var response = query.Select(p => new PreBulkPackDto()
            {
                SysId = p.prebulkpack.SysId,
                WarehouseSysId = p.prebulkpack.WareHouseSysId,
                PreBulkPackOrder = p.prebulkpack.PreBulkPackOrder,
                StorageCase = p.prebulkpack.StorageCase,
                Status = p.prebulkpack.Status.Value,
                CreateDate = p.prebulkpack.CreateDate,
                OutboundOrder = p.prebulkpack.OutboundOrder,
                OutboundSysId = p.prebulkpack.OutboundSysId
            }).Distinct();

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderByDescending(p => p.PreBulkPackOrder).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }

        public PreBulkPackDto GetPreBulkPackBySysId(Guid sysId)
        {
            var query = from prebulkpack in Context.prebulkpack
                        where prebulkpack.SysId == sysId
                        select new PreBulkPackDto()
                        {
                            SysId = prebulkpack.SysId,
                            WarehouseSysId = prebulkpack.WareHouseSysId,
                            PreBulkPackOrder = prebulkpack.PreBulkPackOrder,
                            Status = prebulkpack.Status.Value,
                            StorageCase = prebulkpack.StorageCase,
                            CreateDate = prebulkpack.CreateDate
                        };

            var preBulkPack = query.FirstOrDefault();
            if (preBulkPack != null)
            {
                var queryDetail =
                        from prebulkpackdetail in Context.prebulkpackdetail
                        join sku in Context.skus on prebulkpackdetail.SkuSysId equals sku.SysId
                        where prebulkpackdetail.PreBulkPackSysId == sysId

                        join p in Context.packs on sku.PackSysId equals p.SysId into t4
                        from p4 in t4.DefaultIfEmpty()

                        join u in Context.uoms on p4.FieldUom01 equals u.SysId into t2
                        from ut2 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on p4.FieldUom02 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()

                        select new PreBulkPackDetailDto()
                        {
                            SysId = prebulkpackdetail.SysId,
                            PreBulkPackSysId = prebulkpackdetail.PreBulkPackSysId,
                            SkuSysId = prebulkpackdetail.SkuSysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            UPC = sku.UPC,
                            Qty = prebulkpackdetail.Qty,
                            Loc = prebulkpackdetail.Loc,
                            PreQty = prebulkpackdetail.PreQty,
                            UomCode = p4.InLabelUnit01.HasValue && p4.InLabelUnit01.Value == true && p4.FieldValue01 > 0 && p4.FieldValue02 > 0 ? ti3.UOMCode : ut2.UOMCode,
                            PackCode = p4.PackCode
                        };

                preBulkPack.PreBulkPackDetailList = queryDetail.ToList();
            }

            return preBulkPack;
        }

        /// <summary>
        /// 根据出库单更新散货封箱单状态
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        public void UpdaPreBulkPack(Guid outboundSysId, int userId, string userName)
        {
            var sql = new StringBuilder();
            //装箱中的修改成完成
            sql.AppendFormat(@"UPDATE prebulkpack SET Status = {0}, 
                        UpdateDate = NOW(), 
                        UpdateBy =@UpdateBy , 
                        UpdateUserName =@UpdateUserName
                        where OutboundSysId = @OutboundSysId AND Status = {1};", (int)PreBulkPackStatus.Finish, (int)PreBulkPackStatus.PrePack);

            //新建的修改成作废
            sql.AppendFormat(@"UPDATE prebulkpack SET Status = {0}, 
                        UpdateDate = NOW(), 
                        UpdateBy = @UpdateBy ,
                        UpdateUserName = @UpdateUserName
                        where OutboundSysId =  @OutboundSysId AND Status = {1};", (int)PreBulkPackStatus.Cancel, (int)PreBulkPackStatus.New);
            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                        , new MySqlParameter("@UpdateBy", userId)
                        , new MySqlParameter("@UpdateUserName", userName)
                        , new MySqlParameter("@OutboundSysId", outboundSysId));
        }

        /// <summary>
        /// 作废散货封箱
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        public void CancelPreBulkPack(Guid outboundSysId, int userId, string userName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE prebulkpack SET Status = {0}, 
                        UpdateDate = NOW(), 
                        UpdateBy = @UpdateBy ,
                        UpdateUserName = @UpdateUserName
                        where OutboundSysId =  @OutboundSysId  AND Status = {1};", (int)PreBulkPackStatus.PrePack, (int)PreBulkPackStatus.Finish);
            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                        , new MySqlParameter("@UpdateBy", userId)
                        , new MySqlParameter("@UpdateUserName", userName)
                        , new MySqlParameter("@OutboundSysId", outboundSysId));
        }

        /// <summary>
        /// 修改散货状态
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        /// <param name="userId">修改人ID</param>
        /// <param name="userName">修改人名称</param>
        /// <param name="toStatus">目标状态</param>
        /// <param name="fromStatus">原始装填，可不填</param>
        public void UpdatePreBulkPackStatus(Guid outboundSysId, int userId, string userName, int toStatus, int fromStatus = 0)
        {
            var sql = new StringBuilder();
            sql.Append(@"UPDATE prebulkpack SET Status =@Status, 
                        UpdateDate = NOW(), 
                        UpdateBy = @UpdateBy ,
                        UpdateUserName = @UpdateUserName
                        where OutboundSysId = @OutboundSysId ");
            if (fromStatus != 0)
            {
                sql.AppendFormat(" AND Status = {0}", fromStatus);
            }
            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                        , new MySqlParameter("@Status", toStatus)
                        , new MySqlParameter("@UpdateBy", userId)
                        , new MySqlParameter("@UpdateUserName", userName)
                        , new MySqlParameter("@OutboundSysId", outboundSysId));
        }

        /// <summary>
        /// 根据出库单ID回去散货封箱单列表
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<OutboundPreBulkPackDto> GetOutboundPreBulkPackList(Guid outboundSysId)
        {
            var result = new List<OutboundPreBulkPackDto>();
            var sql = new StringBuilder();

            sql.Append(@" SELECT p.SysId as PreBulkPackSysId,p.PreBulkPackOrder,COUNT(p1.SkuSysId) AS SkuQty,
                        IFNULL(SUM(p1.Qty),0) AS Qty,p.OutboundOrder 
                        FROM prebulkpack p
                        LEFT JOIN prebulkpackdetail p1 ON p.SysId=p1.PreBulkPackSysId
                        WHERE p.OutboundSysId=@OutboundSysId
                        GROUP BY  p.SysId,p.PreBulkPackOrder;");

            result = base.Context.Database.SqlQuery<OutboundPreBulkPackDto>(sql.ToString()
                , new MySqlParameter("@OutboundSysId", outboundSysId)).ToList();
            return result;
        }

        /// <summary>
        /// 获取出库单散货箱明细
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<PreBulkPackDetailDto> GetPreBulkPackDetailByPreBulkPackSysIds(List<Guid> preBulkPackSysIds)
        {
            var query = from pd in Context.prebulkpackdetail
                        join p in Context.prebulkpack on pd.PreBulkPackSysId equals p.SysId
                        where preBulkPackSysIds.Contains(p.SysId)
                        group new { pd.Qty } by new { pd.SkuSysId } into g
                        join s in Context.skus on g.Key.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        join u1 in Context.uoms on ti.FieldUom01 equals u1.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u2 in Context.uoms on ti.FieldUom02 equals u2.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        select new PreBulkPackDetailDto
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
    }
}
