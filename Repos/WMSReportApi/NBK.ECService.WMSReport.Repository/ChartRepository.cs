using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.EntityFramework;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility.Enum;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMSReport.Repository
{
    public class ChartRepository : CrudRepository, IChartRepository
    {
        /// <param name="dbContextProvider"></param>
        public ChartRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public List<AdventSkuChartDto> GetAdventSkuChartDto(DateTime warningDate, Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            var query = from illl in Context.invlotloclpns
                        join il in Context.invlots on illl.SkuSysId equals il.SkuSysId
                        join s in Context.skus on illl.SkuSysId equals s.SysId
                        where illl.Lot == il.Lot && illl.WareHouseSysId == il.WareHouseSysId && il.ExpiryDate != null && il.ExpiryDate <= warningDate
                        && illl.WareHouseSysId == wareHouseSysId
                        orderby il.ExpiryDate ascending
                        select new AdventSkuChartDto
                        {
                            SkuSysId = s.SysId,
                            SkuName = s.SkuName,
                            Loc = illl.Loc,
                            Lot = illl.Lot,
                            Qty = illl.Qty,
                            ExpiryDate = il.ExpiryDate
                        };
            return query.Take(5).ToList();
        }

        public List<SkuReceiptReportDto> GetSkuReceiptChart(DateTime startDate, DateTime endDate, Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            var query = from r in Context.receipts
                        join rd in Context.receiptdetails on r.SysId equals rd.ReceiptSysId
                        where r.Status == (int)ReceiptStatus.Received
                            && r.ReceiptDate > startDate
                            && r.ReceiptDate < endDate
                            && r.WarehouseSysId == wareHouseSysId
                        select new SkuReceiptReportDto
                        {
                            ReceiptDate = r.ReceiptDate.Value,
                            ReceivedQty = rd.ReceivedQty.Value
                        };

            return query.ToList();
        }

        public List<SkuOutboundReportDto> GetSkuOutboundChart(DateTime startDate, DateTime endDate, Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            var query = from r in Context.outbounds
                        join rd in Context.outbounddetails on r.SysId equals rd.OutboundSysId
                        where r.Status == (int)OutboundStatus.Delivery
                            && r.OutboundDate > startDate
                            && r.OutboundDate < endDate
                            && r.WareHouseSysId == wareHouseSysId
                        select new SkuOutboundReportDto
                        {
                            OutboundDate = r.OutboundDate.Value,
                            OutboundQty = rd.Qty.Value
                        };

            return query.ToList();
        }

        /// <summary>
        /// 预包装看板
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<PrePackBoardDto> GetPrePackBoardTop12(Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            var sqlstr = new StringBuilder();
            sqlstr.Append(" SELECT p.SysId, p.PrePackOrder, IFNULL(p.OutboundOrder, '暂无') AS OutboundOrder,p.OutboundSysId, ");
            sqlstr.Append(" IFNULL(p.StorageLoc, '暂未指定') AS StorageLoc,");
            sqlstr.Append(" IFNULL(SUM(p1.PreQty),0) as PreQty,IFNULL(SUM(p1.Qty),0) AS Qty FROM prepack p");
            sqlstr.Append(" LEFT JOIN prepackdetail p1 ON p.SysId=p1.PrePackSysId");
            sqlstr.Append(" WHERE p.WarehouseSysId=@WarehouseSysId");
            sqlstr.Append(" GROUP BY p1.PrePackSysId  ORDER BY p.UpdateDate DESC LIMIT 0,12;");
            var queryList = base.Context.Database.SqlQuery<PrePackBoardDto>(sqlstr.ToString(),new MySqlParameter("@WarehouseSysId",wareHouseSysId)).ToList();
            if (queryList != null && queryList.Count > 0)
            {
                var sysIds = string.Empty;
                var sysPreIds = string.Empty;
                foreach (var item in queryList)
                {
                    if (!string.IsNullOrEmpty(item.OutboundOrder) && item.OutboundSysId != null)
                    {
                        sysIds += "'" + item.OutboundSysId.ToString() + "',";
                        sysPreIds += "'" + item.SysId.ToString() + "',";
                    }
                }
                if (!string.IsNullOrEmpty(sysIds) && !string.IsNullOrEmpty(sysPreIds))
                {
                    sysIds = sysIds.Substring(0, sysIds.Length - 1);
                    sysPreIds = sysPreIds.Substring(0, sysPreIds.Length - 1);
                    var listsqlstr = new StringBuilder();
                    listsqlstr.Append(" SELECT o.SysId as OutboundSysId,o.OutboundOrder,o1.SkuSysId,o1.Qty FROM outbound o");
                    listsqlstr.Append(" LEFT JOIN outbounddetail o1 ON o.SysId = o1.OutboundSysId");
                    listsqlstr.AppendFormat(" WHERE o.SysId IN ({0});", sysIds);
                    var list1 = base.Context.Database.SqlQuery<OutboundSkuList>(listsqlstr.ToString()).ToList();

                    var listsqlstr1 = new StringBuilder();
                    listsqlstr1.Append(" SELECT p.OutboundSysId,p.OutboundOrder,p1.SkuSysId,p1.Qty FROM prepack p");
                    listsqlstr1.Append(" LEFT JOIN prepackdetail p1 ON p.SysId=p1.PrePackSysId");
                    listsqlstr1.AppendFormat(" WHERE p.SysId IN ({0});", sysPreIds);
                    var list2 = base.Context.Database.SqlQuery<PrePackOutboundSkuList>(listsqlstr1.ToString()).ToList();
                    foreach (var dome in queryList)
                    {
                        var flag = false;
                        var outQty = list1.Where(x => x.OutboundSysId == dome.OutboundSysId).ToList();
                        if (outQty != null && outQty.Count > 0)
                        {
                            foreach (var model in outQty)
                            {
                                var hsaQty = list2.Where(x => x.OutboundSysId == model.OutboundSysId && x.SkuSysId == model.SkuSysId && x.Qty == model.Qty).FirstOrDefault();
                                if (hsaQty == null)
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (!flag)
                        {
                            dome.Type = 1;
                        }
                        else
                        {
                            dome.Type = 0;
                        }
                    }
                }
            }
            return queryList;
        }

        /// <summary>
        /// 获取时间最长的6条超过3天未收货的入库单
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<ExceedThreeDaysPurchase> GetExceedThreeDaysPurchase(Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            var date = DateTime.Now.AddDays(-3);
            var query = from purch in Context.purchases
                        where purch.Status == (int)PurchaseStatus.New && purch.AuditingDate <= date
                        && purch.WarehouseSysId == wareHouseSysId
                        select new ExceedThreeDaysPurchase()
                        {
                            SysId = purch.SysId,
                            PurchaseOrder = purch.PurchaseOrder,
                            AuditingDate = purch.AuditingDate
                        };

            return query.OrderBy(x => x.AuditingDate).Take(6).ToList();
        }



        /// <summary>
        /// 出入库单据
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public PurchaseAndOutboundChartDto GetPurchaseAndOutboundChart(Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            
            var sql = new StringBuilder();
            sql.Append(@"SELECT   COUNT(CASE WHEN p.Status=0 THEN 1 END) AS PurchaseNewCount,
                              COUNT(CASE WHEN p.Status=10 OR p.Status=20 THEN 1 END) AS PurchaseInReceiptCount,
                              COUNT(CASE WHEN p.Status=30 THEN 1 END) AS PurchaseFinishCount 
                              FROM purchase p WHERE p.WareHouseSysId=@WareHouseSysId;");


            var purchase = base.Context.Database.SqlQuery<PurchaseChartData>(sql.ToString(), new MySqlParameter("@WareHouseSysId",wareHouseSysId)).FirstOrDefault();

            var sqlStr = new StringBuilder();
            sqlStr.Append(@"SELECT COUNT(CASE WHEN o.Status=10 THEN 1 END) AS OutboundNewCount,
                                  COUNT(CASE WHEN o.Status=35 OR o.Status=40 THEN 1 END) AS OutboundPickCount,
                                  COUNT(CASE WHEN o.Status=70 THEN 1 END) AS OutboundDeliveryCount 
                                  FROM outbound o WHERE o.WareHouseSysId=@WareHouseSysId;");
            var outbound = base.Context.Database.SqlQuery<OutboundChartData>(sqlStr.ToString(), new MySqlParameter("@WareHouseSysId", wareHouseSysId)).FirstOrDefault();

            return new PurchaseAndOutboundChartDto()
            {
                PurchaseNewCount = purchase.PurchaseNewCount,
                PurchaseInReceiptCount = purchase.PurchaseInReceiptCount,
                PurchaseFinishCount = purchase.PurchaseFinishCount,

                OutboundNewCount = outbound.OutboundNewCount,
                OutboundPickCount = outbound.OutboundPickCount,
                OutboundDeliveryCount = outbound.OutboundDeliveryCount
            };
        }

        /// <summary>
        /// 近10日订单和退货
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<OutboundAndReturnCharDto> GetOutboundAndReturnCharDataOfLastTenDays(Guid wareHouseSysId)
        {
            ChangeDB(wareHouseSysId);
            List<OutboundAndReturnCharDto> charDto = new List<OutboundAndReturnCharDto>();
            const int pastDays = 10;
            DateTime startDate = DateTime.Now.Date.AddDays(-pastDays);
            DateTime endDate = DateTime.Now.Date.AddMilliseconds(-1);
            var outboundList = (from ob in Context.outbounds
                                where ob.CreateDate >= startDate && ob.CreateDate <= endDate && ob.WareHouseSysId == wareHouseSysId
                                select new
                                {
                                    ob.SysId,
                                    ob.CreateDate
                                }).ToList();

            var receiptList = (from r in Context.receipts
                               where r.CreateDate >= startDate && r.CreateDate <= endDate && r.WarehouseSysId == wareHouseSysId
                              && r.ReceiptType == (int)ReceiptType.Return
                               select new
                               {
                                   r.SysId,
                                   r.CreateDate
                               }).ToList();

            for (int i = 1; i <= pastDays; i++)
            {
                DateTime fromDate = DateTime.Now.Date.AddDays(-i);
                DateTime toDate = DateTime.Now.Date.AddDays(-i + 1).AddMilliseconds(-1);

                charDto.Add(new OutboundAndReturnCharDto
                {
                    DisplayOrder = 10 - i + 1,
                    Date = fromDate,
                    OutboundTotalCount = outboundList.Where(p => fromDate <= p.CreateDate && p.CreateDate <= toDate).Count(),
                    ReturnTotalCount = receiptList.Where(p => fromDate <= p.CreateDate && p.CreateDate <= toDate).Count()
                });
            }
            return charDto;
        }

        public List<OutboundChartDto> GetToDayOrderStatusTotal(Guid wareHouseSysId, string startTime, string endTime)
        {
            ChangeDB(wareHouseSysId);
            var sqlstr = new StringBuilder();
            sqlstr.Append(" SELECT o.OutboundOrder,o.OutboundDate,o.OutboundType,o.Status, ");
            sqlstr.Append(
                "  (select Count(*) from outboundDetail od where od.outboundSysId = o.sysId ) AS OutboundSkuCount ");
            sqlstr.AppendFormat(
                " FROM outbound o WHERE o.OutboundDate>=@StartTime AND o.OutboundDate<=@EndTime  AND WareHouseSysId=@WareHouseSysId",
                startTime, endTime, wareHouseSysId);
            return base.Context.Database.SqlQuery<OutboundChartDto>(sqlstr.ToString(), 
                new MySqlParameter("@WareHouseSysId", wareHouseSysId)
                , new MySqlParameter("@StartTime", startTime)
                , new MySqlParameter("@EndTime", endTime)).ToList();
        }
    }
}