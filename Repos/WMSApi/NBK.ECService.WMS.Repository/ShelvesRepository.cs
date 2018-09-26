using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMS.Repository
{
    public class ShelvesRepository : CrudRepository, IShelvesRepository
    {
        /// <param name="dbContextProvider"></param>
        public ShelvesRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 获取待上架收货单
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingShelvesListDto> GetWaitingShelvesList(RFShelvesQuery shelvesQuery)
        {
            var query = from r in Context.receipts
                        join rd in Context.receiptdetails on r.SysId equals rd.ReceiptSysId
                        where (rd.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || rd.ShelvesStatus == (int)ShelvesStatus.Shelves)
                        && (rd.Status == (int)ReceiptStatus.Received) && r.Status != (int)ReceiptStatus.Cancel && r.WarehouseSysId == shelvesQuery.WarehouseSysId
                        group new { r, rd } by new { r.SysId, r.ReceiptOrder, rd.SkuSysId } into g
                        select new
                        {
                            SysId = g.Key.SysId,
                            ReceiptOrder = g.Key.ReceiptOrder,
                            SkuSysId = g.Key.SkuSysId,
                            ReceivedQty = g.Sum(x => x.rd.ReceivedQty),
                            ShelvesQty = g.Sum(x => x.rd.ShelvesQty)
                        };

            var convertuomQuery = from q in query
                                  join s in Context.skus on q.SkuSysId equals s.SysId
                                  join p in Context.packs on s.PackSysId equals p.SysId into t2
                                  from p1 in t2.DefaultIfEmpty()
                                  select new
                                  {
                                      SysId = q.SysId,
                                      ReceiptOrder = q.ReceiptOrder,
                                      SkuSysId = q.SkuSysId,
                                      ReceivedQty = q.ReceivedQty,
                                      ShelvesQty = q.ShelvesQty,
                                      DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * (q.ReceivedQty.HasValue ? q.ReceivedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (q.ReceivedQty.HasValue ? q.ReceivedQty.Value : 0),
                                      DisplayShelvesQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * q.ShelvesQty * 1.00m) / p1.FieldValue01.Value), 3) : q.ShelvesQty
                                  };

            var waitingShelvesList = (from q in convertuomQuery
                                      group q by new { q.SysId, q.ReceiptOrder } into g
                                      select new RFWaitingShelvesListDto
                                      {
                                          SysId = g.Key.SysId,
                                          ReceiptOrder = g.Key.ReceiptOrder,
                                          SkuNumber = g.Count(),
                                          SkuQty = g.Sum(x => x.ReceivedQty - x.ShelvesQty),
                                          DisplaySkuQty = g.Sum(x => x.DisplayReceivedQty - x.DisplayShelvesQty)
                                      });

            shelvesQuery.iTotalDisplayRecords = waitingShelvesList.Count();
            waitingShelvesList = waitingShelvesList.OrderByDescending(p => p.ReceiptOrder).Skip(shelvesQuery.iDisplayStart).Take(shelvesQuery.iDisplayLength);
            return ConvertPages(waitingShelvesList, shelvesQuery);
        }

        /// <summary>
        /// 获取某个单据的待上架商品
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public List<RFWaitingShelvesSkuListDto> GetWaitingShelvesSkuList(RFShelvesQuery shelvesQuery)
        {
            var query = from rd in Context.receiptdetails
                        join s in Context.skus on rd.SkuSysId equals s.SysId
                        join r in Context.receipts on rd.ReceiptSysId equals r.SysId
                        where r.ReceiptOrder == shelvesQuery.ReceiptOrder && r.WarehouseSysId == shelvesQuery.WarehouseSysId
                        group rd by new { rd.ReceiptSysId, rd.SkuSysId, s.SkuCode, s.SkuName, s.UPC } into g
                        select new
                        {
                            ReceiptSysId = g.Key.ReceiptSysId,
                            SkuSysId = g.Key.SkuSysId,
                            SkuCode = g.Key.SkuCode,
                            SkuName = g.Key.SkuName,
                            UPC = g.Key.UPC,
                            ReceivedQty = g.Sum(x => x.ReceivedQty),
                            SkuQty = g.Sum(x => x.ReceivedQty - x.ShelvesQty)
                        };

            var convertuomQuery = from q in query
                                  join s in Context.skus on q.SkuSysId equals s.SysId
                                  join p in Context.packs on s.PackSysId equals p.SysId into t2
                                  from p1 in t2.DefaultIfEmpty()
                                  select new RFWaitingShelvesSkuListDto
                                  {
                                      ReceiptSysId = q.ReceiptSysId,
                                      SkuSysId = q.SkuSysId,
                                      SkuCode = q.SkuCode,
                                      SkuName = q.SkuName,
                                      UPC = q.UPC,
                                      ReceivedQty = q.ReceivedQty,
                                      SkuQty = q.SkuQty,
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
                                      DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * (q.ReceivedQty.HasValue ? q.ReceivedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (q.ReceivedQty.HasValue ? q.ReceivedQty.Value : 0),
                                      DisplaySkuQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * (q.SkuQty.HasValue ? q.SkuQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (q.SkuQty.HasValue ? q.SkuQty.Value : 0)
                                  };

            return convertuomQuery.ToList();
        }

        /// <summary>
        /// 获取收货明细推荐货位
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public string GetAdviceToLoc(RFShelvesQuery shelvesQuery)
        {
            var toLoc = "";

            if (shelvesQuery.SkuSysId.HasValue)
            {
                var query = (from rd in Context.receiptdetails
                             join r in Context.receipts on rd.ReceiptSysId equals r.SysId
                             join s in Context.skus on rd.SkuSysId equals s.SysId
                             where r.ReceiptOrder == shelvesQuery.ReceiptOrder
                                && s.SysId == shelvesQuery.SkuSysId.Value
                                && r.WarehouseSysId == shelvesQuery.WarehouseSysId
                             select new { ToLoc = rd.ToLoc }).FirstOrDefault();

                if (query != null)
                {
                    toLoc = query.ToLoc;
                }
            }
            else
            {
                var query = (from rd in Context.receiptdetails
                             join r in Context.receipts on rd.ReceiptSysId equals r.SysId
                             join s in Context.skus on rd.SkuSysId equals s.SysId
                             where r.ReceiptOrder == shelvesQuery.ReceiptOrder && s.UPC == shelvesQuery.UPC && r.WarehouseSysId == shelvesQuery.WarehouseSysId
                             select new { ToLoc = rd.ToLoc }).FirstOrDefault();

                if (query != null)
                {
                    toLoc = query.ToLoc;
                }
            }

            return toLoc;
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        public List<RFInventoryListDto> GetInventoryList(RFInventoryQuery inventoryQuery)
        {
            if (!string.IsNullOrEmpty(inventoryQuery.ReceiptOrder))
            {
                var query = from i in Context.invskulocs
                            join s in Context.skus on i.SkuSysId equals s.SysId
                            join receiptdetail in Context.receiptdetails on s.SysId equals receiptdetail.SkuSysId
                            join receipt in Context.receipts on receiptdetail.ReceiptSysId equals receipt.SysId
                            join p in Context.packs on s.PackSysId equals p.SysId
                            where receipt.ReceiptOrder == inventoryQuery.ReceiptOrder
                            select new { i.Loc, i.Qty, p, s.UPC, i.WareHouseSysId, i.SkuSysId };

                #region 查询条件
                if (inventoryQuery != null)
                {
                    query = query.Where(x => x.WareHouseSysId == inventoryQuery.WarehouseSysId);
                    query = query.Where(x => x.UPC == inventoryQuery.UPC);
                    if (inventoryQuery.SkuSysId.HasValue)
                    {
                        query = query.Where(x => x.SkuSysId == inventoryQuery.SkuSysId.Value);
                    }
                }
                #endregion

                var result = query.Select(x => new RFInventoryListDto
                {
                    Loc = x.Loc,
                    Qty = x.Qty,
                    SkuSysId = x.SkuSysId,
                    DisplayQty = x.p.InLabelUnit01.HasValue && x.p.InLabelUnit01.Value == true
                                && x.p.FieldValue01 > 0 && x.p.FieldValue02 > 0
                                ? Math.Round(((x.p.FieldValue02.Value * x.Qty * 1.00m) / x.p.FieldValue01.Value), 3) : x.Qty
                }).OrderBy(x => x.Loc).Distinct().ToList();

                return result;
            }
            else
            {
                var query = from i in Context.invskulocs
                            join s in Context.skus on i.SkuSysId equals s.SysId
                            join p in Context.packs on s.PackSysId equals p.SysId into t
                            from ti in t.DefaultIfEmpty()
                            select new { i.Loc, i.Qty, ti, s.UPC, i.WareHouseSysId, i.SkuSysId };

                #region 查询条件
                if (inventoryQuery != null)
                {
                    query = query.Where(x => x.WareHouseSysId == inventoryQuery.WarehouseSysId);
                    query = query.Where(x => x.UPC == inventoryQuery.UPC);
                    if (inventoryQuery.SkuSysId.HasValue)
                    {
                        query = query.Where(p => p.SkuSysId == inventoryQuery.SkuSysId.Value);
                    }
                }
                #endregion

                var result = query.Select(x => new RFInventoryListDto
                {
                    Loc = x.Loc,
                    Qty = x.Qty,
                    SkuSysId = x.SkuSysId,
                    DisplayQty = x.ti.InLabelUnit01.HasValue && x.ti.InLabelUnit01.Value == true
                                && x.ti.FieldValue01 > 0 && x.ti.FieldValue02 > 0
                                ? Math.Round(((x.ti.FieldValue02.Value * x.Qty * 1.00m) / x.ti.FieldValue01.Value), 3) : x.Qty
                }).OrderBy(x => x.Loc).Distinct().ToList();

                return result;
            }
        }

        /// <summary>
        /// 获取待上架加工单列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        public Pages<RFAssemblyWaitingShelvesListDto> GetAssemblyWaitingShelvesList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            var query = from a in Context.assemblies
                        where a.Status == (int)AssemblyStatus.Finished
                            && (a.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || a.ShelvesStatus == (int)ShelvesStatus.Shelves)
                            && a.WareHouseSysId == assemblyShelvesQuery.WarehouseSysId
                        select new RFAssemblyWaitingShelvesListDto
                        {
                            SysId = a.SysId,
                            AssemblyOrder = a.AssemblyOrder,
                            SkuNumber = 1,
                            SkuQty = a.ActualQty - a.ShelvesQty
                        };
            assemblyShelvesQuery.iTotalDisplayRecords = query.Count();
            query = query.OrderByDescending(p => p.AssemblyOrder).Skip(assemblyShelvesQuery.iDisplayStart).Take(assemblyShelvesQuery.iDisplayLength);
            return ConvertPages(query, assemblyShelvesQuery);
        }

        /// <summary>
        /// 获取加工单待上架商品列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        public List<RFAssemblyWaitingShelvesSkuListDto> GetAssemblyWaitingShelvesSkuList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            var query = from a in Context.assemblies
                        join s in Context.skus on a.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where a.AssemblyOrder == assemblyShelvesQuery.AssemblyOrder && a.WareHouseSysId == assemblyShelvesQuery.WarehouseSysId
                        select new RFAssemblyWaitingShelvesSkuListDto
                        {
                            AssemblySysId = a.SysId,
                            SkuSysId = a.SkuSysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            UPC = s.UPC,
                            ActualQty = a.ActualQty,
                            SkuQty = a.ActualQty - a.ShelvesQty,
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
        /// 根据商品sysId List  获取 对应商品系统内货位
        /// </summary>
        /// <param name="skuSysIds"></param>
        /// <returns></returns>
        public List<InvSkuLocDto> GetSkuLocBySkuSysIds(List<Guid> skuSysIds, Guid wareHouseSysId, string lotattr01)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT lot.SkuSysId,lpn.Loc,s.SkuName,s.UPC,l.Status as LocationStatus,SUM(lot.Qty) Qty ");
            strSql.Append(" FROM invlot lot  LEFT JOIN  invlotloclpn lpn ON lot.Lot = lpn.Lot AND lot.SkuSysId = lpn.SkuSysId and lot.WareHouseSysId = lpn.WareHouseSysId");
            strSql.Append(" LEFT JOIN  sku s ON s.SysId = lot.SkuSysId LEFT JOIN  location l ON lpn.Loc = l.Loc and lot.WareHouseSysId = l.WarehouseSysId  ");
            strSql.Append(" WHERE lot.WareHouseSysId =@WareHouseSysId ");
            strSql.AppendFormat("  AND lot.SkuSysId IN ({0})  ", skuSysIds.GuidListToIds());
            strSql.Append("   GROUP BY lot.SkuSysId, lpn.Loc, s.SkuName, s.UPC, l.Status ");
            strSql.Append("   Order by Qty desc ");
            return base.Context.Database.SqlQuery<InvSkuLocDto>(strSql.ToString()
                , new MySqlParameter("@WareHouseSysId", wareHouseSysId)).ToList();

        }
    }
}
