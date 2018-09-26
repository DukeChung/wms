using Abp.EntityFramework;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Repository
{
    public class StockTakeRepository : CrudRepository, IStockTakeRepository
    {
        /// <param name="dbContextProvider"></param>
        public StockTakeRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeListDto> GetStockTakeListByPaging(StockTakeQuery stockTakeQuery)
        {
            var query = from s in Context.stocktakes
                        join sd in Context.stocktakedetails on s.SysId equals sd.StockTakeSysId into t1
                        from sdt1 in t1.DefaultIfEmpty()
                        join sk in Context.skus on sdt1.SkuSysId equals sk.SysId into t2
                        from skt2 in t2.DefaultIfEmpty()
                        select new { s, skt2 };

            query = query.Where(x => x.s.WarehouseSysId == stockTakeQuery.WarehouseSysId);

            if (!stockTakeQuery.StockTakeOrderSearch.IsNull())
            {
                stockTakeQuery.StockTakeOrderSearch = stockTakeQuery.StockTakeOrderSearch.Trim();
                query = query.Where(p => p.s.StockTakeOrder == stockTakeQuery.StockTakeOrderSearch);
            }
            if (stockTakeQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.s.Status == stockTakeQuery.StatusSearch.Value);
            }
            if (stockTakeQuery.AssignBySearch.HasValue)
            {
                query = query.Where(p => p.s.AssignBy == stockTakeQuery.AssignBySearch.Value);
            }
            if (!stockTakeQuery.SkuUPCSearch.IsNull())
            {
                stockTakeQuery.SkuUPCSearch = stockTakeQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.skt2.UPC == stockTakeQuery.SkuUPCSearch);
            }
            if (!stockTakeQuery.SkuCodeSearch.IsNull())
            {
                stockTakeQuery.SkuCodeSearch = stockTakeQuery.SkuCodeSearch.Trim();
                query = query.Where(p => p.skt2.SkuCode == stockTakeQuery.SkuCodeSearch);
            }
            if (!stockTakeQuery.SkuNameSearch.IsNull())
            {
                stockTakeQuery.SkuNameSearch = stockTakeQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.skt2.SkuName.Contains(stockTakeQuery.SkuNameSearch));
            }
            if (stockTakeQuery.CreateBySearch.HasValue)
            {
                query = query.Where(p => p.s.CreateBy == stockTakeQuery.CreateBySearch.Value);
            }
            if (stockTakeQuery.StartTimeSearch.HasValue)
            {
                DateTime toDate = stockTakeQuery.StartTimeSearch.Value.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => stockTakeQuery.StartTimeSearch.Value <= p.s.StartTime && p.s.StartTime <= toDate);
            }
            if (stockTakeQuery.EndTimeSearch.HasValue)
            {
                DateTime toDate = stockTakeQuery.EndTimeSearch.Value.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => stockTakeQuery.EndTimeSearch.Value <= p.s.EndTime && p.s.EndTime <= toDate);
            }
            var stockTakes = query.Select(p => new StockTakeListDto
            {
                SysId = p.s.SysId,
                StockTakeOrder = p.s.StockTakeOrder,
                StockTakeType = p.s.StockTakeType,
                Status = p.s.Status,
                StartTime = p.s.StartTime,
                EndTime = p.s.EndTime,
                AssignUserName = p.s.AssignUserName,
                ReplayUserName = p.s.ReplayUserName,
                CreateUserName = p.s.CreateUserName,
                CreateDate = p.s.CreateDate
            }).Distinct();
            stockTakeQuery.iTotalDisplayRecords = stockTakes.Count();
            stockTakes = stockTakes.OrderByDescending(p => p.CreateDate).Skip(stockTakeQuery.iDisplayStart).Take(stockTakeQuery.iDisplayLength);
            return ConvertPages(stockTakes, stockTakeQuery);
        }

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeDetailViewDto> GetStockTakeDetailListByPaging(StockTakeViewQuery stockTakeViewQuery)
        {
            var query = from sd in Context.stocktakedetails
                        join s in Context.skus on sd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t1
                        from p1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t2
                        from ut2 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on p1.FieldUom02 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where sd.StockTakeSysId == stockTakeViewQuery.StockTakeSysIdSearch
                        select new { sd, s, p1, UOMCode1 = ut2.UOMCode, UOMCode2 = ti3.UOMCode };
            if (!stockTakeViewQuery.SkuUPCSearch.IsNull())
            {
                stockTakeViewQuery.SkuUPCSearch = stockTakeViewQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.s.UPC == stockTakeViewQuery.SkuUPCSearch);
            }
            if (!stockTakeViewQuery.SkuCodeSearch.IsNull())
            {
                stockTakeViewQuery.SkuCodeSearch = stockTakeViewQuery.SkuCodeSearch.Trim();
                query = query.Where(p => p.s.SkuCode == stockTakeViewQuery.SkuCodeSearch);
            }
            if (!stockTakeViewQuery.SkuNameSearch.IsNull())
            {
                stockTakeViewQuery.SkuNameSearch = stockTakeViewQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(stockTakeViewQuery.SkuNameSearch));
            }
            var stockTakeDetails = query.Select(p => new StockTakeDetailViewDto
            {
                SysId = p.sd.SysId,
                StockTakeSysId = p.sd.StockTakeSysId,
                SkuSysId = p.sd.SkuSysId,
                Loc = p.sd.Loc,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                SkuUPC = p.s.UPC,
                UOMCode = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                Qty = p.sd.Qty,
                DisplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.sd.Qty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.sd.Qty,
                StockTakeQty = p.sd.StockTakeQty,
                DisplayStockTakeQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.sd.StockTakeQty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.sd.StockTakeQty,
                ReplayQty = p.sd.ReplayQty,

                DisplayReplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                                && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                                ? Math.Round(((p.p1.FieldValue02.Value * (p.sd.ReplayQty.HasValue ? p.sd.ReplayQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.sd.ReplayQty.HasValue ? p.sd.ReplayQty.Value : 0),

                Remark = p.sd.Remark,
                Status = p.sd.Status,
                CreateDate = p.sd.CreateDate
            }).Distinct();
            stockTakeViewQuery.iTotalDisplayRecords = stockTakeDetails.Count();
            stockTakeDetails = stockTakeDetails.OrderByDescending(p => p.CreateDate).Skip(stockTakeViewQuery.iDisplayStart).Take(stockTakeViewQuery.iDisplayLength);
            return ConvertPages(stockTakeDetails, stockTakeViewQuery);
        }

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeDetailViewDto> GetStockTakeDiffListByPaging(StockTakeViewQuery stockTakeViewQuery)
        {
            var query = from sd in Context.stocktakedetails
                        join s in Context.skus on sd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t1
                        from p1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t2
                        from ut2 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on p1.FieldUom02 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where sd.StockTakeSysId == stockTakeViewQuery.StockTakeSysIdSearch
                        && ((sd.Status == (int)StockTakeDetailStatus.StockTake && sd.Qty != sd.StockTakeQty) || (sd.Status == (int)StockTakeDetailStatus.Replay && sd.Qty != sd.ReplayQty))
                        select new { sd, s, p1, UOMCode1 = ut2.UOMCode, UOMCode2 = ti3.UOMCode };
            if (!stockTakeViewQuery.SkuUPCSearch.IsNull())
            {
                stockTakeViewQuery.SkuUPCSearch = stockTakeViewQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.s.UPC == stockTakeViewQuery.SkuUPCSearch);
            }
            if (!stockTakeViewQuery.SkuCodeSearch.IsNull())
            {
                stockTakeViewQuery.SkuCodeSearch = stockTakeViewQuery.SkuCodeSearch.Trim();
                query = query.Where(p => p.s.SkuCode == stockTakeViewQuery.SkuCodeSearch);
            }
            if (!stockTakeViewQuery.SkuNameSearch.IsNull())
            {
                stockTakeViewQuery.SkuNameSearch = stockTakeViewQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(stockTakeViewQuery.SkuNameSearch));
            }
            var stockTakeDetails = query.Select(p => new StockTakeDetailViewDto
            {
                SysId = p.sd.SysId,
                StockTakeSysId = p.sd.StockTakeSysId,
                SkuSysId = p.sd.SkuSysId,
                Loc = p.sd.Loc,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                SkuUPC = p.s.UPC,
                UOMCode = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                Qty = p.sd.Qty,
                DisplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.sd.Qty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.sd.Qty,
                StockTakeQty = p.sd.StockTakeQty,
                DisplayStockTakeQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.sd.StockTakeQty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.sd.StockTakeQty,
                ReplayQty = p.sd.ReplayQty,
                DisplayReplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                                && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                                ? Math.Round(((p.p1.FieldValue02.Value * (p.sd.ReplayQty.HasValue ? p.sd.ReplayQty.Value : 0) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.sd.ReplayQty.HasValue ? p.sd.ReplayQty.Value : 0),
                Remark = p.sd.Remark,
                Status = p.sd.Status,
                CreateDate = p.sd.CreateDate
            }).Distinct();
            stockTakeViewQuery.iTotalDisplayRecords = stockTakeDetails.Count();
            stockTakeDetails = stockTakeDetails.OrderByDescending(p => p.CreateDate).Skip(stockTakeViewQuery.iDisplayStart).Take(stockTakeViewQuery.iDisplayLength);
            return ConvertPages(stockTakeDetails, stockTakeViewQuery);
        }

        public Pages<StockTakeReportListDto> GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery)
        {
            var query = from sd in Context.stocktakedetails
                        join s in Context.stocktakes on sd.StockTakeSysId equals s.SysId
                        join sk in Context.skus on sd.SkuSysId equals sk.SysId
                        join p in Context.packs on sk.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u in Context.uoms on ti1.FieldUom01 equals u.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where stockTakeReportQuery.SysIds.Contains(s.SysId)
                        select new { sd, s, sk, ti1, UOMCode1 = ti2.UOMCode, UOMCode2 = ti3.UOMCode };
            if (!stockTakeReportQuery.SkuUPCSearch.IsNull())
            {
                stockTakeReportQuery.SkuUPCSearch = stockTakeReportQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.sk.UPC == stockTakeReportQuery.SkuUPCSearch);
            }
            if (!stockTakeReportQuery.SkuCodeSearch.IsNull())
            {
                stockTakeReportQuery.SkuCodeSearch = stockTakeReportQuery.SkuCodeSearch.Trim();
                query = query.Where(p => p.sk.SkuCode == stockTakeReportQuery.SkuCodeSearch);
            }
            if (!stockTakeReportQuery.SkuNameSearch.IsNull())
            {
                stockTakeReportQuery.SkuNameSearch = stockTakeReportQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.sk.SkuName.Contains(stockTakeReportQuery.SkuNameSearch));
            }
            if (stockTakeReportQuery.HasDiffSearch.HasValue && stockTakeReportQuery.HasDiffSearch.Value)
            {
                query = query.Where(p => (p.sd.Status == (int)StockTakeDetailStatus.StockTake && p.sd.Qty != p.sd.StockTakeQty)
                || (p.sd.Status == (int)StockTakeDetailStatus.Replay && p.sd.Qty != p.sd.ReplayQty));
            }
            var stockTakeReport = query.Select(p => new StockTakeReportListDto
            {
                SysId = p.sd.SysId,
                Status = p.sd.Status,
                SkuCode = p.sk.SkuCode,
                SkuUPC = p.sk.UPC,
                SkuName = p.sk.SkuName,
                SkuDescr = p.sk.SkuDescr,
                Loc = p.sd.Loc,
                UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
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
            }).Distinct();
            stockTakeReportQuery.iTotalDisplayRecords = stockTakeReport.Count();
            stockTakeReport = stockTakeReport.OrderByDescending(p => p.CreateDate).Skip(stockTakeReportQuery.iDisplayStart).Take(stockTakeReportQuery.iDisplayLength);
            return ConvertPages(stockTakeReport, stockTakeReportQuery);
        }

        /// <summary>
        /// 根据库位获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <param name="locSysIds"></param>
        /// <returns></returns>
        public Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuByLocation(StockTakeSkuQuery stockTakeSkuQuery)
        {
            var query = from s in Context.skus
                        join i in Context.invskulocs on s.SysId equals i.SkuSysId
                        join l in Context.locations on i.Loc equals l.Loc
                        where i.WareHouseSysId == stockTakeSkuQuery.WarehouseSysId
                        select new { s, i, l };

            if (stockTakeSkuQuery.LocSysId.HasValue)
            {
                query = query.Where(p => p.l.SysId == stockTakeSkuQuery.LocSysId.Value);
            }
            else if (stockTakeSkuQuery.ZoneSysId.HasValue)
            {
                query = query.Where(p => p.l.ZoneSysId == stockTakeSkuQuery.ZoneSysId.Value);
            }

            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuUPC = p.s.UPC,
                SkuName = p.s.SkuName,
                Loc = p.i.Loc
            }).Distinct();
            stockTakeSkuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderBy(p => p.SkuUPC).Skip(stockTakeSkuQuery.iDisplayStart).Take(stockTakeSkuQuery.iDisplayLength);
            return ConvertPages(skus, stockTakeSkuQuery);
        }

        /// <summary>
        /// 根据商品信息获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuBySkuInfo(StockTakeSkuQuery stockTakeSkuQuery)
        {
            var query = from s in Context.skus
                        join i in Context.invskulocs on s.SysId equals i.SkuSysId
                        where i.WareHouseSysId == stockTakeSkuQuery.WarehouseSysId
                        select new { s };
            if (!stockTakeSkuQuery.SkuUPC.IsNull())
            {
                stockTakeSkuQuery.SkuUPC = stockTakeSkuQuery.SkuUPC.Trim();
                query = query.Where(p => p.s.UPC == stockTakeSkuQuery.SkuUPC);
            }
            if (stockTakeSkuQuery.SkuClassSysId.HasValue)
            {
                query = query.Where(p => p.s.SkuClassSysId == stockTakeSkuQuery.SkuClassSysId.Value);
            }
            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuUPC = p.s.UPC,
                SkuName = p.s.SkuName
            }).Distinct().OrderBy(p => p.SkuUPC);
            return new Pages<StockTakeSkuListDto> { TableResuls = new TableResults<StockTakeSkuListDto> { aaData = skus.ToList() } };
        }

        /// <summary>
        /// 根据交易信息获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuByInvTrans(StockTakeSkuQuery stockTakeSkuQuery)
        {
            var invTransQuery = from i in Context.invtrans
                                where i.WareHouseSysId == stockTakeSkuQuery.WarehouseSysId && stockTakeSkuQuery.StartDate.Value <= i.CreateDate && i.CreateDate <= stockTakeSkuQuery.EndDate.Value
                                select new { i };
            var invTranSkus = invTransQuery.Select(p => new { SkuSysId = p.i.SkuSysId, Loc = p.i.Loc }).Distinct();

            var query = from i in invTranSkus
                        join s in Context.skus on i.SkuSysId equals s.SysId
                        select new { s, i };

            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuUPC = p.s.UPC,
                SkuName = p.s.SkuName,
                Loc = p.i.Loc
            }).Distinct();
            stockTakeSkuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderBy(p => p.SkuUPC).Skip(stockTakeSkuQuery.iDisplayStart).Take(stockTakeSkuQuery.iDisplayLength);
            return ConvertPages(skus, stockTakeSkuQuery);
        }

        /// <summary>
        /// 根据库位获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        public List<StockTakeSkuListDto> GeStockTakeDetailsByLocation(NewStockTakeDto newStockTakeDto)
        {
            var query = from s in Context.skus
                        join i in Context.invskulocs on s.SysId equals i.SkuSysId
                        join l in Context.locations on i.Loc equals l.Loc
                        where i.WareHouseSysId == newStockTakeDto.WarehouseSysId
                        select new { s, i, l };

            if (newStockTakeDto.LocSysId.HasValue)
            {
                query = query.Where(p => p.l.SysId == newStockTakeDto.LocSysId.Value);
            }
            else if (newStockTakeDto.ZoneSysId.HasValue)
            {
                query = query.Where(p => p.l.ZoneSysId == newStockTakeDto.ZoneSysId.Value);
            }

            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.s.SysId,
                Loc = p.i.Loc
            }).Distinct();
            return skus.ToList();
        }

        /// <summary>
        /// 根据商品获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        public List<StockTakeSkuListDto> GetStockTakeDetailsBySkuInfo(NewStockTakeDto newStockTakeDto)
        {
            var query = from i in Context.invskulocs
                        where i.WareHouseSysId == newStockTakeDto.WarehouseSysId && newStockTakeDto.SkuSysIds.Contains(i.SkuSysId)
                        select new { i };
            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.i.SkuSysId,
                Loc = p.i.Loc
            }).Distinct();
            return skus.ToList();
        }

        /// <summary>
        /// 根据交易获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        public List<StockTakeSkuListDto> GeStockTakeDetailsByInvTrans(NewStockTakeDto newStockTakeDto)
        {
            var invTransQuery = from i in Context.invtrans
                                where i.WareHouseSysId == newStockTakeDto.WarehouseSysId && newStockTakeDto.StartDate.Value <= i.CreateDate && i.CreateDate <= newStockTakeDto.EndDate.Value
                                select new { i };
            var invTranSkus = invTransQuery.Select(p => new { SkuSysId = p.i.SkuSysId, Loc = p.i.Loc }).Distinct();

            var query = from i in invTranSkus
                        join s in Context.skus on i.SkuSysId equals s.SysId
                        select new { s, i };

            var skus = query.Select(p => new StockTakeSkuListDto()
            {
                SysId = p.s.SysId,
                Loc = p.i.Loc
            }).Distinct();
            return skus.ToList();
        }
    }
}
