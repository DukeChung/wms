using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using MySql.Data.MySqlClient;
using System.Text;

namespace NBK.ECService.WMS.Repository
{
    public class RFStockTakeRepository : CrudRepository, IRFStockTakeRepository
    {
        /// <param name="dbContextProvider"></param>
        public RFStockTakeRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 初盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public RFStockTakeListingDto GetStockTakeFirstList(RFStockTakeQuery stockTakeQuery)
        {
            var result = new RFStockTakeListingDto();

            var query = from sd in Context.stocktakedetails
                        join ss in Context.stocktakes on sd.StockTakeSysId equals ss.SysId
                        join s in Context.skus on sd.SkuSysId equals s.SysId
                        where ss.AssignBy == stockTakeQuery.CurrentUserId && ss.WarehouseSysId == stockTakeQuery.WarehouseSysId
                        && ss.Status != (int)StockTakeStatus.StockTakeFinished
                        group new { sd } by new { sd.SysId, sd.SkuSysId, sd.Loc, s.SkuCode, s.UPC } into g
                        select new StockTakeFirstListDto
                        {
                            SysId = g.Key.SysId,
                            SkuSysId = g.Key.SkuSysId,
                            SkuCode = g.Key.SkuCode,
                            UPC = g.Key.UPC,
                            Loc = g.Key.Loc,
                            StockTakeQty = g.Sum(x => x.sd.StockTakeQty)
                        };

            result.TotalSkuNumber = query.GroupBy(x => x.SkuSysId).Count();
            result.StockTakeFirstListDto = query.ToList();

            return result;
        }

        /// <summary>
        /// 获取待初盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<RFStockTakeListDto> GetStockTakeFirstListByPaging(RFStockTakeQuery stockTakeQuery)
        {
            var query = from st in Context.stocktakes
                        join sdi in (from sd in Context.stocktakedetails
                                     group sd by sd.StockTakeSysId into g
                                     select new
                                     {
                                         StockTakeSysId = g.Key,
                                         SkuCount = g.Count()
                                     }) on st.SysId equals sdi.StockTakeSysId into t
                        from sdii in t.DefaultIfEmpty()
                        where st.WarehouseSysId == stockTakeQuery.WarehouseSysId
                        && (st.Status == (int)StockTakeStatus.Started || st.Status == (int)StockTakeStatus.StockTake)
                        && st.AssignBy == stockTakeQuery.CurrentUserId
                        select new { st, SkuCount = sdii.SkuCount };
            var stockTakes = query.Select(p => new RFStockTakeListDto
            {
                SysId = p.st.SysId,
                SkuCount = p.SkuCount,
                StockTakeOrder = p.st.StockTakeOrder,
                Status = p.st.Status
            });
            stockTakeQuery.iTotalDisplayRecords = stockTakes.Count();
            stockTakes = stockTakes.OrderByDescending(p => p.StockTakeOrder).Skip(stockTakeQuery.iDisplayStart).Take(stockTakeQuery.iDisplayLength);
            return ConvertPages(stockTakes, stockTakeQuery);
        }

        public List<RFStockTakeFirstDetailDto> GetSkuByUPC(string upc)
        {
            var query = from s in Context.skus
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where s.UPC == upc
                        select new RFStockTakeFirstDetailDto
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
        /// 获取初盘明细
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public List<StockTakeFirstListDto> GetStockTakeFirstDetailList(RFStockTakeQuery stockTakeQuery)
        {
            var query = from sd in Context.stocktakedetails
                        join s in Context.stocktakes on sd.StockTakeSysId equals s.SysId
                        join sk in Context.skus on sd.SkuSysId equals sk.SysId
                        join p in Context.packs on sk.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        where s.WarehouseSysId == stockTakeQuery.WarehouseSysId
                        && s.AssignBy == stockTakeQuery.CurrentUserId
                        && s.StockTakeOrder == stockTakeQuery.StockTakeOrder
                        && (sd.Status == (int)StockTakeDetailStatus.New || sd.Status == (int)StockTakeDetailStatus.StockTake)
                        select new StockTakeFirstListDto
                        {
                            SysId = sd.SysId,
                            SkuSysId = sd.SkuSysId,
                            SkuName = sk.SkuName,
                            Loc = sd.Loc,
                            UPC = sk.UPC,
                            StockTakeQty = sd.StockTakeQty,
                            DisplayStockTakeQty = ti1.InLabelUnit01.HasValue && ti1.InLabelUnit01.Value == true
                                            && ti1.FieldValue01 > 0 && ti1.FieldValue02 > 0
                                            ? Math.Round(((ti1.FieldValue02.Value * sd.StockTakeQty * 1.00m) / ti1.FieldValue01.Value), 3) : sd.StockTakeQty
                        };
            return query.OrderBy(p => p.Loc).ToList();
        }

        /// <summary>
        /// 盘点单明细原材料和成品是否混合
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <param name="isMaterial"></param>
        /// <returns></returns>
        public bool GetStockTakeFirstMaterialProduct(Guid stockTakeSysId,bool? isMaterial)
        {
            var query = (from sd in Context.stocktakedetails
                         join s in Context.skus on sd.SkuSysId equals s.SysId
                         where sd.StockTakeSysId == stockTakeSysId
                         select new
                         {
                             SkuSysId = sd.SkuSysId,
                             IsMaterial = s.IsMaterial
                         }).ToList();

            if (query != null && query.Count > 0)
            {
                query = query.FindAll(x => ((isMaterial == false || isMaterial == null) ? (x.IsMaterial == null || x.IsMaterial == false) : x.IsMaterial == isMaterial));
                if(query == null || query.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            return true;
        }

        /// <summary>
        /// 复盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<RFStockTakeListDto> GetStockTakeSecondListByPage(RFStockTakeQuery stockTakeQuery)
        {
            var stockTakeList = from st in Context.stocktakes
                                join std in (from std in Context.stocktakedetails
                                             where std.Status == (int)StockTakeDetailStatus.Replay
                                             group new { std } by new { std.StockTakeSysId } into g
                                             select new
                                             {
                                                 StockTakeSysId = g.Key.StockTakeSysId,
                                                 SkuCount = g.Count()
                                             }) on st.SysId equals std.StockTakeSysId
                                where st.Status == (int)StockTakeStatus.Replay && st.ReplayBy == stockTakeQuery.CurrentUserId
                                && st.WarehouseSysId == stockTakeQuery.WarehouseSysId
                                select new RFStockTakeListDto()
                                {
                                    SysId = st.SysId,
                                    StockTakeOrder = st.StockTakeOrder,
                                    SkuCount = std.SkuCount,
                                    Status = st.Status
                                };

            stockTakeQuery.iTotalDisplayRecords = stockTakeList.Count();
            stockTakeList = stockTakeList.OrderByDescending(p => p.StockTakeOrder).Skip(stockTakeQuery.iDisplayStart).Take(stockTakeQuery.iDisplayLength);
            return ConvertPages(stockTakeList, stockTakeQuery);
        }

        /// <summary>
        /// 复盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public List<StockTakeSecondListDto> GetStockTakeSecondList(RFStockTakeQuery stockTakeQuery)
        {
            var query = from sd in Context.stocktakedetails
                        join ss in Context.stocktakes on sd.StockTakeSysId equals ss.SysId into t1
                        from ss1 in t1.DefaultIfEmpty()
                        join s in Context.skus on sd.SkuSysId equals s.SysId into t3
                        from s1 in t3.DefaultIfEmpty()
                        join p in Context.packs on s1.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        where ss1.StockTakeOrder == stockTakeQuery.StockTakeOrder && ss1.Status == (int)StockTakeStatus.Replay && ss1.ReplayBy == stockTakeQuery.CurrentUserId
                        && sd.Status == (int)StockTakeDetailStatus.Replay && ss1.WarehouseSysId == stockTakeQuery.WarehouseSysId
                        select new StockTakeSecondListDto
                        {
                            SysId = sd.SysId,
                            SkuSysId = sd.SkuSysId,
                            SkuName = s1.SkuName,
                            Loc = sd.Loc,
                            UPC = s1.UPC,
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
                            ReplayQty = sd.ReplayQty,
                            DisplayReplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0 && sd.ReplayQty != null
                                            ? Math.Round(((p1.FieldValue02.Value * (sd.ReplayQty ?? 0) * 1.00m) / p1.FieldValue01.Value), 3) : (decimal?)sd.ReplayQty
                        };

            return query.OrderBy(p => p.Loc).ToList();
        }

        /// <summary>
        /// 查询未盘点库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        public List<RFInventoryListDto> GetInventoryNoStockTakeList(RFInventoryQuery inventoryQuery)
        {
            const string strSql = @"SELECT i.Loc, i.Qty, sd.StockTakeTime FROM
                                  (SELECT i.Loc,i.SkuSysId,i.Qty,stemp.SysId FROM invskuloc i
                                  Left JOIN (SELECT sd.Loc,sd.SkuSysId,sd.SysId from stocktakedetail sd
                                  INNER JOIN (select SysId from stocktake WHERE AssignBy = @AssignBy AND Status != @Status 
                                  order by createdate desc limit 1) st ON sd.StockTakeSysId = st.SysId) stemp
                                  ON i.Loc = stemp.Loc AND i.SkuSysId = stemp.SkuSysId
                                  WHERE stemp.SysId IS NULL AND i.WareHouseSysId = @WareHouseSysId) i
                                  LEFT JOIN 
                                  (SELECT sd.SkuSysId,sd.Loc,
                                  (SELECT StockTakeTime FROM stocktakedetail WHERE Loc = sd.loc and skusysid = sd.skusysid ORDER BY StockTakeTime DESC LIMIT 1) as StockTakeTime
                                  from stocktakedetail sd
                                  INNER JOIN (select SysId from stocktake WHERE AssignBy = @AssignBy AND Status != @Status 
                                  order by createdate desc limit 1) st ON st.SysId != sd.StockTakeSysId
                                  GROUP by sd.SkuSysId,sd.Loc
                                  ORDER by StockTakeTime desc) sd on i.Loc = sd.loc AND i.SkuSysId = sd.SkuSysId
                                  LEFT JOIN sku sku ON sku.SysId = i.SkuSysId
                                  WHERE sku.UPC = @UPC
                                  ORDER BY i.Loc ASC";

            var result = base.Context.Database.SqlQuery<RFInventoryListDto>(strSql
                , new MySqlParameter("@AssignBy", inventoryQuery.UserId)
                , new MySqlParameter("@UPC", inventoryQuery.UPC)
                , new MySqlParameter("@Status", (int)StockTakeStatus.StockTakeFinished)
                , new MySqlParameter("@WareHouseSysId",inventoryQuery.WarehouseSysId)).ToList();

            return result;
        }
    }
}
