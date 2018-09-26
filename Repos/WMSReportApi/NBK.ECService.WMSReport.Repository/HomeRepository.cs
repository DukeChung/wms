using Abp.EntityFramework;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.Repository
{
    public class HomeRepository : CrudRepository, IHomeRepository
    {
        public HomeRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        #region 报表首页相关数据查询
        /// <summary>
        ///  获取采购入库/B2B出库/B2C出库数据
        /// </summary>
        /// <returns></returns>
        public SparkLineSummaryDto GetSparkLineSummaryDto()
        {
            var result = new SparkLineSummaryDto();
            var pSql = new StringBuilder();
            pSql.Append("SELECT COUNT(*) FROM purchase p WHERE p.Status= 30;");
            result.Purchase = base.Context.Database.SqlQuery<int>(pSql.ToString()).FirstOrDefault();
            var oB2CSql = new StringBuilder();
            oB2CSql.Append("SELECT COUNT(*) FROM outbound o WHERE o.Status=70 and o.OutboundType=20;");
            result.B2COutbound = base.Context.Database.SqlQuery<int>(oB2CSql.ToString()).FirstOrDefault();
            var oB2BSql = new StringBuilder();
            oB2BSql.Append("SELECT COUNT(*) FROM outbound o WHERE o.Status=70 and o.OutboundType=30;");
            result.B2BOutbound = base.Context.Database.SqlQuery<int>(oB2BSql.ToString()).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        /// <returns></returns>
        public List<PurchaseTypePieDto> GetPurchaseTypePieDto()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT COUNT(1) AS Qty, p.Type  FROM   purchase p WHERE  p.Status = 30 AND p.Type!=50 GROUP BY p.Type;");
            return base.Context.Database.SqlQuery<PurchaseTypePieDto>(strSql.ToString()).ToList();
        }

        /// <summary>
        /// 顶部出库单据类型占比
        /// </summary>
        /// <returns></returns>
        public List<OutboundTypePieDto> GetOutboundTypePieDto()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT COUNT(1) AS Qty, o.OutboundType  FROM   outbound o WHERE o.Status=70 and o.OutboundType!=10 AND o.OutboundType!=50 AND o.OutboundType!=70 GROUP BY  o.OutboundType;");
            return base.Context.Database.SqlQuery<OutboundTypePieDto>(strSql.ToString()).ToList();
        }

        /// <summary>
        /// 当前时间起一年之前收发存统计
        /// </summary>
        /// <returns></returns>
        public List<StockInOutListDto> GetStockInOutData(string startTime, string endTime, ref decimal qty)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT   A.CreateDate, 
                          IFNULL(SUM(A.ReceiptQty),0) AS ReceiptQty,
                          IFNULL(SUM(A.OutboundQty),0) AS OutboundQty
                        FROM (SELECT
                            DATE_FORMAT(i.CreateDate, '%Y-%m') AS CreateDate,
                            CASE WHEN (
                                (i.SourceTransType = 'Losses' AND i.Qty > 0) OR
                                i.SourceTransType = 'AssemblyShelve' OR
                                i.SourceTransType = 'Shelve' OR
                                (i.SourceTransType = 'SkuBorrow' AND
                                i.TransType = 'IN')
                                ) THEN i.Qty END AS ReceiptQty,
                            CASE WHEN (
                                (i.SourceTransType = 'Losses' AND i.Qty < 0) OR
                                i.SourceTransType = 'AssemblyPicking' OR
                                i.SourceTransType = 'AD' OR
                                i.SourceTransType = 'Picking' OR
                                i.SourceTransType = 'QuickDelivery' OR
                                (i.SourceTransType = 'SkuBorrow' AND
                                i.TransType = 'OUT')
                                ) THEN i.Qty END AS OutboundQty
                            FROM invtrans i
                                LEFT JOIN sku s ON i.SkuSysId= s.SysId
                                WHERE  i.Status = 'OK' AND s.IsMaterial!=1
                                AND i.CreateDate >= @StarTime
                                AND i.CreateDate < @EndTime 
                                ) A
                                GROUP BY A.CreateDate 
                                ORDER BY A.CreateDate ASC ;");

            var allList = base.Context.Database.SqlQuery<StockInOutListDto>(sql.ToString()
             , new MySqlParameter("@StarTime", startTime)
             , new MySqlParameter("@EndTime", endTime)
             ).ToList();

            var startQty = new StringBuilder();
            startQty.Append(@" SELECT IFNULL(SUM(i.Qty),0) 
                              FROM invtrans i
                              LEFT JOIN sku s ON i.SkuSysId = s.SysId
                              WHERE i.STATUS = 'OK'
                              AND ((i.TransType = 'AJ'
                              AND SourceTransType = 'Losses')
                              OR i.TransType = 'ASSY'
                              OR i.TransType = 'IN'
                              OR i.TransType = 'OUT')
                              AND i.CreateDate < @StarTime
                              AND s.IsMaterial != 1;");
            qty = base.Context.Database.SqlQuery<decimal>(startQty.ToString()
             , new MySqlParameter("@StarTime", startTime)).FirstOrDefault();

            return allList;
        }

        /// <summary>
        ///获取所有仓库总收货分布
        /// </summary>
        /// <returns></returns>
        public List<WareHouseQtyDto> GetWareHouseReceiptQtyList()
        {
            var startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var sql = new StringBuilder();

            sql.Append(@"SELECT  w.Name AS WareHouseName,IFNULL(SUM(i.Qty),0) AS Qty
                        FROM invtrans i
                          LEFT JOIN sku s  ON i.SkuSysId = s.SysId
                          LEFT JOIN warehouse w  ON i.WareHouseSysId = w.SysId
                        WHERE i.Status = 'OK'
                        AND ((i.SourceTransType = 'Losses' AND i.Qty > 0)
                        OR i.SourceTransType = 'AssemblyShelve'
                        OR i.SourceTransType = 'Shelve'
                        OR (i.SourceTransType = 'SkuBorrow' AND i.TransType = 'IN'))
                        AND i.CreateDate < @StartTime
                        AND s.IsMaterial != 1
                        GROUP BY i.WareHouseSysId
                        ORDER BY Qty ASC;");

            return base.Context.Database.SqlQuery<WareHouseQtyDto>(sql.ToString()
                , new MySqlParameter("@StartTime", startTime)).ToList();
        }

        /// <summary>
        /// 获取所有仓库总出库分布
        /// </summary>
        /// <returns></returns>
        public List<WareHouseQtyDto> GetWareHouseOutboundQtyList()
        {
            var startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var sql = new StringBuilder();

            sql.Append(@"SELECT w.Name AS WareHouseName, IFNULL(SUM(i.Qty), 0) AS Qty
                            FROM invtrans i
                              LEFT JOIN sku s ON i.SkuSysId = s.SysId
                              LEFT JOIN warehouse w ON i.WareHouseSysId = w.SysId
                            WHERE i.Status = 'OK' AND (
                            (i.SourceTransType = 'Losses' AND i.Qty < 0)
                            OR i.SourceTransType = 'AssemblyPicking'
                            OR i.SourceTransType = 'AD'
                            OR i.SourceTransType = 'Picking'
                            OR i.SourceTransType = 'QuickDelivery'
                            OR (i.SourceTransType = 'SkuBorrow' AND i.TransType = 'OUT')
                            ) AND i.CreateDate < @StartTime
                            AND s.IsMaterial != 1
                            GROUP BY i.WareHouseSysId
                            ORDER BY Qty DESC;");

            return base.Context.Database.SqlQuery<WareHouseQtyDto>(sql.ToString()
                , new MySqlParameter("@StartTime", startTime)).ToList();
        }

        /// <summary>
        /// 获取所有仓库总库存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<WareHouseQtyDto> GetWareHouseQtyList()
        {
            var startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var sql = new StringBuilder();

            sql.Append(@"SELECT w.Name AS WareHouseName, IFNULL(SUM(i.Qty), 0) AS Qty
                            FROM invtrans i
                              LEFT JOIN sku s ON i.SkuSysId = s.SysId
                              LEFT JOIN warehouse w ON i.WareHouseSysId = w.SysId
                            WHERE i.Status = 'OK' AND (
                             (i.TransType = 'AJ'
                              AND SourceTransType = 'Losses')
                              OR i.TransType = 'ASSY'
                              OR i.TransType = 'IN'
                              OR i.TransType = 'OUT'
                            ) AND i.CreateDate < @StartTime
                            AND s.IsMaterial != 1
                            GROUP BY i.WareHouseSysId
                            ORDER BY Qty ASC;");

            return base.Context.Database.SqlQuery<WareHouseQtyDto>(sql.ToString()
                , new MySqlParameter("@StartTime", startTime)).ToList();
        }

        /// <summary>
        /// 获取库龄分布
        /// </summary>
        public StockAgeGroupDto GetStockAgeGroup()
        {
            var result = new StockAgeGroupDto();
            var outGroup = new StringBuilder();
            outGroup.Append(@" SELECT  
                                 SUM(IFNULL(CASE WHEN A.Days <= 5 THEN A.Qty END,0))  AS FirstOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 5  AND  A.Days <= 15 THEN A.Qty END,0))  AS SecondOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 15  AND  A.Days <= 30 THEN A.Qty END,0))  AS ThreeOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 30  AND  A.Days <= 50 THEN A.Qty END,0))  AS FourOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 50 THEN A.Qty END,0))  AS FiveOrder 
                                 FROM (
                                SELECT p.Qty,
                                   DATEDIFF(o.ActualShipDate,i1.ReceiptDate) AS Days
                                FROM pickdetail p
                                  LEFT JOIN invlot i1 ON p.Lot = i1.Lot
                                  LEFT JOIN outbound o ON p.OutboundSysId=o.SysId
                                WHERE  o.Status=70 ) A;");

            var outData = base.Context.Database.SqlQuery<StockAgeGroupDto>(outGroup.ToString()).FirstOrDefault();
            if (outData != null)
            {
                result = outData;
            }
            var hasGroup = new StringBuilder();
            hasGroup.Append(@"SELECT  
                                 SUM(IFNULL(CASE WHEN A.Days <= 5 THEN A.Qty END,0))  AS FirstOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 5  AND  A.Days <= 15 THEN A.Qty END,0))  AS SecondOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 15  AND  A.Days <= 30 THEN A.Qty END,0))  AS ThreeOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 30  AND  A.Days <= 50 THEN A.Qty END,0))  AS FourOrder,
                                 SUM(IFNULL(CASE WHEN A.Days > 50 THEN A.Qty END,0))  AS FiveOrder 
                                 FROM (
                                SELECT
                                   i.Qty,
                                   DATEDIFF(NOW(),i.ReceiptDate) AS Days
                                FROM invlot i
                                WHERE i.Qty > 0) A;");
            var hasData = base.Context.Database.SqlQuery<StockAgeGroupDto>(hasGroup.ToString()).FirstOrDefault();
            if (hasData != null)
            {
                result.FirstOrder = result.FirstOrder + hasData.FirstOrder;
                result.SecondOrder = result.SecondOrder + hasData.SecondOrder;
                result.ThreeOrder = result.ThreeOrder + hasData.ThreeOrder;
                result.FourOrder = result.FourOrder + hasData.FourOrder;
                result.FiveOrder = result.FiveOrder + hasData.FiveOrder;
            }
            return result;
        }

        /// <summary>
        /// 获取畅销商品Top10
        /// </summary>
        public List<SkuSaleQtyDto> GetSkuSellingTop10()
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT s.SysId,s.SkuName,SUM(o.ShippedQty) AS OutboundQty 
                        FROM outbounddetail o
                        LEFT JOIN sku s ON o.SkuSysId=s.SysId
                        WHERE o.Status=50
                        GROUP BY  s.SysId 
                        ORDER BY OutboundQty DESC LIMIT 0,10;");

            return base.Context.Database.SqlQuery<SkuSaleQtyDto>(sql.ToString()).ToList();
        }

        /// <summary>
        /// 获取滞销商品Top10
        /// </summary>
        public List<SkuSaleQtyDto> GetSkuUnsalableTop10()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  s.SkuName,IFNULL(SUM(i.Qty),0) AS OutboundQty,
                              IFNULL(DATEDIFF(@StartTime,i.ReceiptDate),0) AS Days 
                              FROM invlot i 
                              LEFT JOIN sku s ON i.SkuSysId=s.SysId
                              WHERE i.Qty>0
                              GROUP BY i.SkuSysId ORDER BY i.ReceiptDate  ASC LIMIT 0,10;");

            return base.Context.Database.SqlQuery<SkuSaleQtyDto>(strSql.ToString(),
                new MySqlParameter("@StartTime", date)).ToList();
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <returns></returns>
        public List<ServiceStationOutboundDto> GetServiceStationOutboundTopTen()
        {
            var sql = $@"SELECT o.ServiceStationName, COUNT(DISTINCT o.OutboundOrder) AS TotalOrder, SUM(o1.ShippedQty) AS TotalQty 
                        FROM outbound o
                        INNER JOIN outbounddetail o1 ON o.SysId = o1.OutboundSysId
                        WHERE o.Status = @Status AND o.ServiceStationName IS NOT NULL AND o.ServiceStationName <> ''
                        GROUP BY o.ServiceStationName
                        ORDER BY TotalQty DESC LIMIT 10;";
            return base.Context.Database.SqlQuery<ServiceStationOutboundDto>(sql, new MySqlParameter("@Status", (int)OutboundStatus.Delivery)).ToList();
        }


        /// <summary>
        /// 获取渠道库存占比
        /// </summary>
        /// <returns></returns>
        public List<ChannelQtyDto> GetChannelPieData()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT IFNULL(i.LotAttr01,'')  AS Channel ,SUM(i.Qty) AS Qty FROM invlot i
                            GROUP BY Channel ;");

            return base.Context.Database.SqlQuery<ChannelQtyDto>(strSql.ToString()).ToList();
        }

        /// <summary>
        /// 获取最新10个退货入库收货完成的单子
        /// </summary>
        /// <returns></returns>
        public List<ReturnPurchaseDto> GetReturnPurchase()
        {
            //SELECT p.ExternalOrder,p.PurchaseDate FROM  purchase p
            //                 WHERE p.Type = 10 AND p.Status = 30 ORDER BY p.PurchaseDate DESC LIMIT 0,10;
            var strSql = new StringBuilder();
            strSql.Append(@"  SELECT o.OutboundOrder AS ExternalOrder,o.OutboundDate,
                             o.UpdateDate AS PurchaseDate FROM outbound o 
                            WHERE o.IsReturn IS NOT NULL AND o.Status=70
                            ORDER BY  o.UpdateDate desc  LIMIT 0,10 ;");

            return base.Context.Database.SqlQuery<ReturnPurchaseDto>(strSql.ToString()).ToList();
        }

        /// <summary>
        /// 获取出库日历数据统计
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public CalendarOrdersOutInData GetCalendarData(string startDate, string endDate)
        {
            var result = new CalendarOrdersOutInData();
            var outSql = new StringBuilder();
            var startTime = DateTime.Parse(startDate + " 00:00:00");
            var endTime = DateTime.Parse(endDate + " 23:59:59");
            outSql = outSql.Append(@"  SELECT o.Status,o.CreateDate AS Date
                                       FROM outbound  o WHERE
                                       o.CreateDate>=@startTime  AND  o.CreateDate<=@endTime");

            result.OutboundList = base.Context.Database.SqlQuery<CalendarOrdersData>(outSql.ToString(),
                     new MySqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss")),
                     new MySqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm:ss"))).ToList();

            var inSql = new StringBuilder();
            inSql.Append(@" SELECT p.Status,p.CreateDate AS Date
                               FROM purchase p  
                               WHERE p.CreateDate>= @startTime AND p.CreateDate<=@endTime ");

            result.PurchaseList = base.Context.Database.SqlQuery<CalendarOrdersData>(inSql.ToString(),
                    new MySqlParameter("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new MySqlParameter("@endTime", endTime.ToString("yyyy-MM-dd HH:mm:ss"))).ToList();
            return result;

        }

        /// <summary>
        /// 获取所有仓库某一天出库入库数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public CalendarDataByDateOutboundOrPurchase GetCalendarDataByDate(string date)
        {
            var startTime = date + " 00:00:00";
            var endTime = Convert.ToDateTime(startTime).AddDays(1).ToString("yyyy-MM-dd 00:00:00");

            var result = new CalendarDataByDateOutboundOrPurchase();
            var outSql = new StringBuilder();
            outSql.Append(@"SELECT  w.Name,
                                  COUNT(o.Status) AS OutboundQty, 
                                  COUNT(CASE WHEN o.Status=10 THEN 1 END) AS NewOutboundQty,
                                  COUNT(CASE WHEN o.Status=30 THEN 1 END) AS AllocationQty,
                                  COUNT(CASE WHEN o.Status=40 THEN 1 END) AS PickingQty,
                                  COUNT(CASE WHEN o.Status=70 THEN 1 END) AS DeliveryQty,
                                  COUNT(CASE WHEN o.Status=-999 OR o.Status=-10 THEN 1 END) AS CancelOutboundQty
                                  FROM warehouse w
                                  LEFT JOIN outbound o ON w.SysId=o.WareHouseSysId 
                                  AND o.CreateDate>=@startTime AND o.CreateDate<@endTime
                                  GROUP BY  w.SysId;");
            result.OutboundList = base.Context.Database.SqlQuery<CalendarDataByDateDto>(outSql.ToString(),
                    new MySqlParameter("@startTime", startTime),
                    new MySqlParameter("@endTime", endTime)).ToList();

            var inSql = new StringBuilder();
            inSql.Append(@"SELECT  w.Name,  
                           COUNT(p.Status) AS PurchaseQty, 
                           COUNT(CASE WHEN p.Status=0 THEN 1 END) AS NewPurchaseQty,
                           COUNT(CASE WHEN p.Status=10 THEN 1 END) AS InReceiptQty,
                           COUNT(CASE WHEN p.Status=20 THEN 1 END) AS PartReceiptQty,
                           COUNT(CASE WHEN p.Status=30 THEN 1 END) AS FinishQty,
                           COUNT(CASE WHEN p.Status=-10 or p.Status=-999 THEN 1 END) AS CancelPurchaseQty
                           FROM warehouse w                         
                           LEFT JOIN purchase p ON w.SysId=p.WarehouseSysId 
                           AND p.CreateDate>=@startTime AND p.CreateDate<@endTime
                           GROUP BY  w.SysId;");

            result.PurchaseList = base.Context.Database.SqlQuery<CalendarDataByDateDto>(inSql.ToString(),
                    new MySqlParameter("@startTime", startTime),
                    new MySqlParameter("@endTime", endTime)).ToList();
            return result;
        }
        #endregion

        #region 定时批处理任务
        /// <summary>
        /// 获取所有经纬度为空的出库单总条数
        /// </summary>
        /// <returns></returns>
        public int GetOutboundNoLngLatCount()
        {
            var countSql = new StringBuilder();
            countSql.Append(@"SELECT COUNT(*) FROM outbound o WHERE 
                            o.lng IS NULL OR o.lng='' OR o.lat='' OR o.lat IS NULL;");
            return base.Context.Database.SqlQuery<int>(countSql.ToString()).AsQueryable().FirstOrDefault();
        }

        /// <summary>
        /// 获取所有经纬度为空的出库单
        /// </summary>
        /// <returns></returns>
        public List<OutboundLngLatDto> GetNeedToDoLngLatData(int iDisplayStart, int iDisplayLength)
        {
            var strSql = new StringBuilder();
            strSql.Append(@" SELECT o.SysId,o.ConsigneeProvince,o.ConsigneeCity,o.ConsigneeArea,
                                    o.ConsigneeAddress,o.lng,o.lat,o.OutboundType
                                    FROM outbound o WHERE o.lng IS NULL OR o.lng='' 
                                    OR o.lat='' OR o.lat IS NULL
                                    LIMIT @startIndex,@endIndex;");
            return base.Context.Database.SqlQuery<OutboundLngLatDto>(strSql.ToString(),
                new MySqlParameter("@startIndex", iDisplayStart * iDisplayLength),
                new MySqlParameter("@endIndex", iDisplayLength)).ToList();
        }

        public void UpdateOutboundLngLat(List<OutboundLngLatDto> list)
        {
            var sql = new StringBuilder();
            var i = 0;
            var para = new List<MySqlParameter>();
            foreach (var item in list)
            {
                sql.Append($@"  UPDATE outbound o SET o.lng=@Lng{i},o.lat=@Lat{i} 
                                WHERE  o.SysId=@SysId{i};");

                para.Add(new MySqlParameter($"@Lng{i}", item.lng));
                para.Add(new MySqlParameter($"@Lat{i}", item.lat));
                para.Add(new MySqlParameter($"@SysId{i}", item.SysId));
                i++;
            }
            base.Context.Database.ExecuteSqlCommand(sql.ToString(), para.ToArray());
        }

        #endregion

        #region 地图相关业务处理
        /// <summary>
        /// 获取仓库业务覆盖
        /// </summary>
        /// <returns></returns>
        public List<OutboundMapData> GetHistoryServiceStationData(int page)
        {
            string strSql = @"SELECT IFNULL(COUNT(1),0) AS TotalCount,o.ServiceStationName,o.lng,o.lat 
                              FROM outbound o WHERE 
                              o.Status=70 AND (o.lng!='' OR o.lng IS NOT NULL) 
                              AND (o.OutboundType=20 OR o.OutboundType=30)
                              GROUP BY o.lng,o.lat
                              ORDER BY o.CreateDate LIMIT @StartPage,@PageNumber;";
            return base.Context.Database.SqlQuery<OutboundMapData>(strSql,
                new MySqlParameter("@StartPage", page * 500),
                new MySqlParameter("@PageNumber", 500)).ToList();
        }

        /// <summary>
        /// 仓库城市出库关系
        /// </summary>
        /// <returns></returns>
        public List<WarehouseStationRelationDto> GetWarehouseStationRelation()
        {
            var strSql = @"SELECT  w.Name,o.ConsigneeCity,
                              IFNULL(COUNT(1), 0) AS TotalCount
                            FROM outbound o
                              LEFT JOIN warehouse w  ON o.WareHouseSysId = w.SysId
                            WHERE o.ConsigneeCity IS NOT NULL and o.ConsigneeCity !=''
                            AND (o.OutboundType=30 OR o.OutboundType= 80) 
                            GROUP BY w.Name,o.ConsigneeCity;";
            return base.Context.Database.SqlQuery<WarehouseStationRelationDto>(strSql).ToList();

        }
        #endregion

        #region  作业时间分布相关业务处理
        /// <summary>
        /// 获取仓库作业数据
        /// </summary>
        /// <returns></returns>
        public List<WorkDistributionDataDto> GetWorkDistributionData()
        {
            var start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            var end = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var result = new List<WorkDistributionDataDto>();
            var receiptSql = new StringBuilder();
            ///收货
            receiptSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times FROM (
                                SELECT DATE_FORMAT(r1.CreateDate,'%H') AS Hours ,r.WarehouseSysId FROM receipt r
                                LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId 
                                 WHERE r.Status=40  AND r.UpdateDate< @endTime
                                 AND r.UpdateDate>= @startTime  GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                )A GROUP BY A.Hours,A.WarehouseSysId;");
            var receiptData = Context.Database.SqlQuery<WorkDistributionDataDto>(receiptSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();

            result = result.Concat(receiptData).ToList();
            ///上架
            var shelvesSql = new StringBuilder();
            shelvesSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times FROM (
                                SELECT DATE_FORMAT( r1.UpdateDate,'%H') AS Hours,r.WarehouseSysId FROM  receipt r 
                                LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId  AND r1.Status!=10
                                WHERE r.Status=40 AND r1.UpdateDate< @endTime 
                                AND r1.UpdateDate>= @startTime  GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                )A GROUP BY A.Hours ,A.WarehouseSysId ;");
            var shelvesData = Context.Database.SqlQuery<WorkDistributionDataDto>(shelvesSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(shelvesData).ToList();
            ///分配
            var distribSql = new StringBuilder();
            distribSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times FROM (
                                  SELECT DATE_FORMAT(p.CreateDate,'%H') AS Hours,p.WareHouseSysId 
                                  FROM pickdetail p WHERE p.Status!= -999 AND p.CreateDate< @endTime 
                                  AND p.CreateDate>= @startTime
                                )A GROUP BY A.Hours,A.WarehouseSysId ;");
            var distribData = Context.Database.SqlQuery<WorkDistributionDataDto>(distribSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(distribData).ToList();
            ///拣货
            var pickSql = new StringBuilder();
            pickSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times  FROM (
                              SELECT DATE_FORMAT(p.UpdateDate,'%H') AS Hours,p.WareHouseSysId 
                              FROM pickdetail p  WHERE p.Status=50  and p.UpdateDate< @endTime 
                               AND  p.UpdateDate>= @startTime
                            )A GROUP BY A.Hours,A.WarehouseSysId ;");
            var pickData = Context.Database.SqlQuery<WorkDistributionDataDto>(pickSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(pickData).ToList();
            ///复核
            var reviewSql = new StringBuilder();
            reviewSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times  FROM (
                              SELECT DATE_FORMAT(o.ReviewDate,'%H') AS Hours,o.WareHouseSysId 
                              FROM outboundtransferorder o WHERE o.Status!=-999 AND o.ReviewDate IS  NOT NULL
                              AND o.ReviewDate< @endTime AND o.ReviewDate>= @startTime 
                            )A GROUP BY A.Hours,A.WarehouseSysId ;");
            var reviewData = Context.Database.SqlQuery<WorkDistributionDataDto>(reviewSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(reviewData).ToList();
            ///出库
            var outboundSql = new StringBuilder();
            outboundSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times  FROM (
                                SELECT DATE_FORMAT(o.ActualShipDate,'%H') AS Hours,o.WareHouseSysId FROM outbound o
                                  WHERE o.Status=70  AND o.ActualShipDate< @endTime 
                                  AND o.ActualShipDate>= @startTime
                                )A GROUP BY A.Hours,A.WarehouseSysId ;");
            var outboundData = Context.Database.SqlQuery<WorkDistributionDataDto>(outboundSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(outboundData).ToList();
            ///装箱
            var vanningSql = new StringBuilder();
            vanningSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,A.WarehouseSysId,COUNT(1) AS Times  FROM (
                                SELECT  DATE_FORMAT(v.CreateDate,'%H') AS Hours,v1.WarehouseSysId 
                                FROM vanningdetail v
                                LEFT JOIN vanning v1 ON v.VanningSysId=v1.SysId
                                WHERE v.Status!=-999 AND v.CreateDate< @endTime 
                                AND v.CreateDate>= @startTime
                                )A GROUP BY A.Hours,A.WarehouseSysId ;");
            var vanningData = Context.Database.SqlQuery<WorkDistributionDataDto>(vanningSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(vanningData).ToList();
            return result;
        }

        /// <summary>
        /// 根据仓库获业务类型获取作业时间分布
        /// </summary>
        /// <param name="sysId"></param>
        public List<WorkDistributionDataByWarehouseDto> GetWorkDistributionByWarehouse(Guid sysId)
        {
            var start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            var end = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            var result = new List<WorkDistributionDataByWarehouseDto>();
            ///收货
            var receiptSql = new StringBuilder();
            receiptSql.Append(@" SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times FROM (
                                 SELECT DATE_FORMAT(r1.CreateDate,'%H') AS Hours ,r.WarehouseSysId FROM receipt r
                                 LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId 
                                 WHERE r.Status=40  AND r.UpdateDate< @endTime
                                 AND r.UpdateDate>=  @startTime 
                                 AND r.WarehouseSysId= @warehouseSysId
                                 GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                )A GROUP BY A.Hours;");
            var receiptData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(receiptSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = receiptData, BusinessType = "收货" });

            ///上架
            var shelvesSql = new StringBuilder();
            shelvesSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times FROM (
                                SELECT DATE_FORMAT( r1.UpdateDate,'%H') AS Hours,r.WarehouseSysId FROM  receipt r 
                                LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId  AND r1.Status!=10
                                WHERE r.Status=40 AND r1.UpdateDate< @endTime  
                                AND r1.UpdateDate>=  @startTime 
                                AND r.WarehouseSysId= @warehouseSysId
                                GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                )A GROUP BY A.Hours;");
            var shelvesData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(shelvesSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = shelvesData, BusinessType = "上架" });

            ///分配
            var distribSql = new StringBuilder();
            distribSql.Append(@"  SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times FROM (
                                  SELECT DATE_FORMAT(p.CreateDate,'%H') AS Hours,p.WareHouseSysId 
                                  FROM pickdetail p WHERE p.Status!= -999 AND p.CreateDate< @endTime 
                                  AND p.CreateDate>=  @startTime
                                  AND p.WarehouseSysId= @warehouseSysId
                                )A GROUP BY A.Hours;");
            var distribData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(distribSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = distribData, BusinessType = "分配" });

            ///拣货
            var pickSql = new StringBuilder();
            pickSql.Append(@"  SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times  FROM (
                               SELECT DATE_FORMAT(p.UpdateDate,'%H') AS Hours,p.WareHouseSysId 
                               FROM pickdetail p  WHERE p.Status=50  and p.UpdateDate< @endTime 
                               AND p.UpdateDate>=  @startTime 
                               AND p.WarehouseSysId= @warehouseSysId
                            )A GROUP BY A.Hours;");
            var pickData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(pickSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = pickData, BusinessType = "拣货" });

            ///复核
            var reviewSql = new StringBuilder();
            reviewSql.Append(@" SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times  FROM (
                                SELECT DATE_FORMAT(o.ReviewDate,'%H') AS Hours,o.WareHouseSysId 
                                FROM outboundtransferorder o WHERE o.Status!=-999 AND o.ReviewDate IS  NOT NULL
                                AND o.ReviewDate< @endTime AND o.ReviewDate>=  @startTime 
                                AND o.WarehouseSysId= @warehouseSysId
                            )A GROUP BY A.Hours;");
            var reviewData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(reviewSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = reviewData, BusinessType = "复核" });

            ///出库
            var outboundSql = new StringBuilder();
            outboundSql.Append(@" SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times  FROM (
                                  SELECT DATE_FORMAT(o.ActualShipDate,'%H') AS Hours,o.WareHouseSysId FROM outbound o
                                  WHERE o.Status=70  AND o.ActualShipDate< @endTime 
                                  AND o.ActualShipDate>=  @startTime    
                                  AND o.WarehouseSysId= @warehouseSysId
                                )A GROUP BY A.Hours;");
            var outboundData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(outboundSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = outboundData, BusinessType = "出库" });

            ///装箱
            var vanningSql = new StringBuilder();
            vanningSql.Append(@"SELECT  CAST(A.Hours AS SIGNED) AS Hours,COUNT(1) AS Times  FROM (
                                SELECT  DATE_FORMAT(v.CreateDate,'%H') AS Hours,v1.WarehouseSysId 
                                FROM vanningdetail v
                                LEFT JOIN vanning v1 ON v.VanningSysId=v1.SysId
                                WHERE v.Status!=-999 AND v.CreateDate< @endTime 
                                AND v.CreateDate>=  @startTime
                                AND v1.WarehouseSysId= @warehouseSysId
                                )A GROUP BY A.Hours;");
            var vanningData = Context.Database.SqlQuery<WorkTimeDataByWarehouseDto>(vanningSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end),
                        new MySqlParameter("@warehouseSysId", sysId)).ToList();
            result.Add(new WorkDistributionDataByWarehouseDto() { WarehouseData = vanningData, BusinessType = "装箱" });

            return result;
        }

        /// <summary>
        /// 仓库作业类型占比
        /// </summary>
        /// <returns></returns>
        public List<WorkDistributionPieData> GetWorkDistributionPieData()
        {
            var result = new List<WorkDistributionPieData>();
            var start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            var end = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");

            ///收货
            var receiptSql = new StringBuilder();
            receiptSql.Append(@" SELECT A.WarehouseSysId, COUNT(1) AS Times,'收货' AS TypeName FROM (
                                 SELECT r.WarehouseSysId FROM receipt r
                                 LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId 
                                 WHERE r.Status=40  AND r.UpdateDate<@endTime
                                 AND r.UpdateDate>= @startTime   GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                 )A GROUP BY  A.WarehouseSysId;");
            var receiptData = Context.Database.SqlQuery<WorkDistributionPieData>(receiptSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(receiptData).ToList();

            ///上架
            var shelvesSql = new StringBuilder();
            shelvesSql.Append(@"SELECT  A.WarehouseSysId,COUNT(1) AS Times,'上架' AS TypeName  FROM (
                                SELECT r.WarehouseSysId FROM  receipt r 
                                LEFT JOIN receiptdetail r1 ON r.SysId=r1.ReceiptSysId  AND r1.Status!=10
                                WHERE r.Status=40 AND r1.UpdateDate< @endTime  
                                AND r1.UpdateDate>= @startTime 
                                GROUP BY r1.SkuSysId,r1.ReceiptSysId
                                )A GROUP BY A.WarehouseSysId ;");
            var shelvesData = Context.Database.SqlQuery<WorkDistributionPieData>(shelvesSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(shelvesData).ToList();

            ///分配
            var distribSql = new StringBuilder();
            distribSql.Append(@"  SELECT A.WareHouseSysId,COUNT(1) AS Times,'分配' AS TypeName  FROM (
                                  SELECT p.WareHouseSysId 
                                  FROM pickdetail p WHERE p.Status!= -999 AND p.CreateDate<@endTime 
                                  AND p.CreateDate>= @startTime  
                                  )A GROUP BY A.WarehouseSysId ;");
            var distribData = Context.Database.SqlQuery<WorkDistributionPieData>(distribSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(distribData).ToList();

            ///拣货
            var pickSql = new StringBuilder();
            pickSql.Append(@"  SELECT  A.WarehouseSysId,COUNT(1) AS Times,'拣货' AS TypeName   FROM (
                               SELECT  p.WareHouseSysId 
                               FROM pickdetail p  WHERE p.Status=50  and p.UpdateDate<@endTime 
                               AND  p.UpdateDate>= @startTime  
                               )A GROUP BY A.WarehouseSysId ;");
            var pickData = Context.Database.SqlQuery<WorkDistributionPieData>(pickSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(pickData).ToList();

            ///复核
            var reviewSql = new StringBuilder();
            reviewSql.Append(@" SELECT  A.WarehouseSysId,COUNT(1) AS Times,'复核' AS TypeName FROM (
                                SELECT o.WareHouseSysId 
                                FROM outboundtransferorder o WHERE o.Status!=-999 AND o.ReviewDate IS  NOT NULL
                                AND o.ReviewDate<@endTime AND o.ReviewDate>= @startTime  
                                ) A GROUP BY  A.WarehouseSysId ;");
            var reviewData = Context.Database.SqlQuery<WorkDistributionPieData>(reviewSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(reviewData).ToList();

            ///出库
            var outboundSql = new StringBuilder();
            outboundSql.Append(@" SELECT  A.WarehouseSysId,COUNT(1) AS Times ,'出库' AS TypeName   FROM (
                                  SELECT  o.WareHouseSysId FROM outbound o
                                  WHERE o.Status=70  AND o.ActualShipDate<@endTime 
                                  AND o.ActualShipDate>= @startTime     
                                  ) A GROUP BY A.WarehouseSysId ;");
            var outboundData = Context.Database.SqlQuery<WorkDistributionPieData>(outboundSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(outboundData).ToList();

            ///装箱
            var vanningSql = new StringBuilder();
            vanningSql.Append(@"SELECT   A.WarehouseSysId,COUNT(1) AS Times,'装箱' AS TypeName    FROM (
                                SELECT  v1.WarehouseSysId FROM vanningdetail v
                                LEFT JOIN vanning v1 ON v.VanningSysId=v1.SysId
                                WHERE v.Status!=-999 AND v.CreateDate<@endTime 
                                AND v.CreateDate>= @startTime 
                                )A GROUP BY A.WarehouseSysId ;");
            var vanningData = Context.Database.SqlQuery<WorkDistributionPieData>(vanningSql.ToString(),
                        new MySqlParameter("@startTime", start),
                        new MySqlParameter("@endTime", end)).ToList();
            result = result.Concat(vanningData).ToList();
            return result;
        }
        #endregion
    }
}
