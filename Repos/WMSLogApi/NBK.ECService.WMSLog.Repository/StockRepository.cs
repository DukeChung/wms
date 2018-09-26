using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository
{
    public class StockRepository : CrudRepository, IStockRepository
    {
        public StockRepository(IDbContextProvider<NBK_WMS_CheckStore> dbContextProvider) : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockDataSet(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" select il.SkuSysId,s.SkuCode ,s.SkuName  ,IFNULL(il.qty,0) as InvLotQty, ");
            sqlStr.Append(" IFNULL(isl.qty, 0) as InvSkuLocQty, IFNULL(illl.qty, 0) as InvLotLocLpnQty from ");
            sqlStr.Append(" (select skusysid, sum(qty) as qty from invLot ");
            sqlStr.Append("  group by skusysid) il ");
            sqlStr.Append(" left join (select skusysid, sum(qty) as qty from invskuLoc ");
            sqlStr.Append(" group by skusysid) isl on il.skusysid = isl.skusysid ");
            sqlStr.Append(" left join (select skusysid, sum(qty) as qty from invlotloclpn ");
            sqlStr.Append(" group by skusysid) illl on il.skusysid = illl.skusysid ");
            sqlStr.Append(" left join sku s on il.skusysid = s.sysid ");
            sqlStr.Append(" where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty; ");
            var queryList = base.Context.Database.SqlQuery<InvSkuLotLocLpnQty>(sqlStr.ToString()).AsQueryable();
            var list = queryList.ToList();
            return list;
        }

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockAvailableQty(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@" select il.SkuSysId,s.SkuCode ,s.SkuName ,IFNULL(il.qty,0) as InvLotQty,
                            IFNULL(isl.qty,0) as InvSkuLocQty,IFNULL(illl.qty,0) as InvLotLocLpnQty from
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invLot
                          group by skusysid) il
                          left join 
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join 
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty;");
            var queryList = base.Context.Database.SqlQuery<InvSkuLotLocLpnQty>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockAllocatedQty(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@"select il.SkuSysId,s.SkuCode ,s.SkuName ,IFNULL(il.qty,0) as InvLotQty,
                          IFNULL(isl.qty, 0) as InvSkuLocQty, IFNULL(illl.qty, 0) as InvLotLocLpnQty from
                          (select skusysid, sum(AllocatedQty) as qty from invLot
                          group by skusysid) il
                          left join
                          (select skusysid, sum(AllocatedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join
                          (select skusysid, sum(AllocatedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty;");
            var queryList = base.Context.Database.SqlQuery<InvSkuLotLocLpnQty>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockPickedQty(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@" select il.SkuSysId,s.SkuCode ,s.SkuName ,IFNULL(il.qty,0) as InvLotQty,
                          IFNULL(isl.qty, 0) as InvSkuLocQty, IFNULL(illl.qty, 0) as InvLotLocLpnQty   from
                          (select skusysid, sum(PickedQty) as qty from invLot
                          group by skusysid) il
                          left join
                          (select skusysid, sum(PickedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join
                          (select skusysid, sum(PickedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty;");
            var queryList = base.Context.Database.SqlQuery<InvSkuLotLocLpnQty>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }

        /// <summary>
        ///  invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <returns></returns>
        public List<InvLotAndInvLotLocLpn> GetStockSkuLotQty(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@" select il.SkuSysId,s.SkuCode ,s.SkuName ,il.Lot ,IFNULL(il.qty,0) as InvLotQty,
                              IFNULL(illl.qty,0) as InvLotLocLpnQty from
                              (select skusysid,Lot,sum(qty) as qty from invLot
                              group by SkuSysId,lot) il
                              left join 
                              (select skusysid,Lot,sum(qty) as qty from invlotloclpn
                              group by SkuSysId,lot) illl on il.skusysid = illl.SkuSysId AND il.lot = illl.lot
                              left join sku s on il.skusysid = s.sysid
                              where il.qty != illl.qty; ");
            var queryList = base.Context.Database.SqlQuery<InvLotAndInvLotLocLpn>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }


        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLocAndInvLotLocLpn> GetStockSkuLocQty(int type)
        {

            var sqlStr = new StringBuilder();
            sqlStr.Append(@"select illl.SkuSysId,s.SkuCode ,s.SkuName ,illl.Loc ,IFNULL(illl.qty,0) as InvLotQty,
                              IFNULL(isl.qty,0) as InvSkuLocQty from
                              (select skusysid,loc,sum(qty) as qty from invlotloclpn
                              group by skusysid,loc) illl
                              left join
                              (select skusysid,loc,sum(qty) as qty from invskuLoc
                              group by skusysid,loc) isl on isl.skusysid = illl.skusysid and isl.loc = illl.loc
                              left join sku s on illl.skusysid = s.sysid
                              where isl.qty != illl.qty;");
            var queryList = base.Context.Database.SqlQuery<InvSkuLocAndInvLotLocLpn>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }

        /// <summary>
        ///  查询入库，库存，出库的差异
        /// </summary>
        /// <returns></returns>
        public List<DiffReceiptInvOut> GetDiffReceiptInvOut(int type)
        {
            var sqlStr = new StringBuilder();
            //sqlStr.Append(@"SELECT r.SkuSysId,s.SkuCode ,s.SkuName ,IFNULL(SUM(r.qty),0) AS InvQty,
            //              IFNULL(SUM(i.qty),0) AS StockQty,IFNULL(SUM(o.qty),0) AS OutboundQty,
            //              IFNULL(SUM(r.qty - i.qty - o.qty),0) AS DifferentQty  from
            //                (select SkuSysId,sum(qty) as qty from (
            //              select skusysid,sum(ShelvesQty) AS qty from assembly 
            //              group by SkuSysId
            //              union all
            //              select skusysid,sum(ShelvesQty) AS qty from receiptdetail
            //              GROUP BY SkuSysId) t group by SkuSysId) r
            //                left join 
            //                (SELECT skusysid,sum(qty) AS qty from invlotloclpn
            //                group by skusysid) i ON i.skusysid = r.SkuSysId
            //                left join 
            //                (SELECT skusysid,sum(ShippedQty) as qty from outbounddetail
            //                  GROUP BY skusysid) o ON o.SkuSysId = r.SkuSysId
            //                LEFT JOIN sku s ON s.SysId = r.skusysid
            //                GROUP BY r.SkuSysId,s.SkuCode,s.SkuName
            //                HAVING SUM(r.qty) <> SUM(i.qty) + SUM(o.qty);");


            sqlStr.Append(@"SELECT r.SkuSysId,s.SkuCode,s.SkuName,SUM(IFNULL(r.qty,0)) AS InvQty,SUM(IFNULL(i.qty,0)) AS StockQty,SUM(IFNULL(o.qty,0)) AS OutboundQty,SUM(IFNULL(r.qty,0) - IFNULL(i.qty,0) - IFNULL(o.qty,0)) AS DifferentQty from
                        (
                          select SkuSysId,sum(IFNULL(qty,0)) as qty from (
                            select skusysid,sum(IFNULL(ShelvesQty,0)) AS qty from assembly 
                            group by SkuSysId
                            union all
                            select skusysid,sum(IFNULL(ShelvesQty,0)) AS qty from receiptdetail
                            GROUP BY SkuSysId
                            UNION ALL
                            SELECT ad.SkuSysId,SUM(IFNULL(Qty,0)) AS qty FROM adjustmentdetail ad
                            INNER JOIN adjustment a ON ad.AdjustmentSysId = a.SysId
                            WHERE a.Status = 20 AND Qty > 0
                            GROUP by ad.SkuSysId
                            UNION ALL
                            SELECT SkusysId,SUM(IFNULL(ReturnQty,0)) AS qty FROM skuborrowdetail WHERE status in (20,30)
                            GROUP BY SkuSysId
                            ) t group by t.SkuSysId
                          ) r
                          left join 
                            (SELECT skusysid,sum(IFNULL(Qty,0)) AS qty from invlotloclpn
                            group by skusysid) i ON i.skusysid = r.SkuSysId
                          left join 
                          (
                              SELECT SkuSysId,SUM(IFNULL(Qty,0)) AS qty FROM (
                              SELECT skusysid,sum(IFNULL(ShippedQty,0)) as qty from outbounddetail
                              GROUP BY SkuSysId
                              UNION ALL
                              SELECT ad.SkuSysId,SUM(IFNULL(-Qty,0)) AS qty FROM adjustmentdetail ad
                              INNER JOIN adjustment a ON ad.AdjustmentSysId = a.SysId
                              WHERE a.Status = 20 AND Qty < 0
                              GROUP by ad.SkuSysId
                              UNION ALL 
                              SELECT SkuSysId,SUM(IFNULL(Qty,0)) AS qty FROM assemblydetail WHERE Status = 50
                              GROUP by SkuSysId
                              UNION ALL
                              SELECT SkusysId,SUM(IFNULL(Qty,0)) AS qty FROM skuborrowdetail WHERE status in (20,30)
                              GROUP BY SkuSysId
                              ) t1 GROUP by t1.SkuSysId
                            ) o ON o.SkuSysId = r.SkuSysId
                          LEFT JOIN sku s ON s.SysId = r.skusysid
                          GROUP BY r.SkuSysId,s.SkuCode,s.SkuName
                          HAVING SUM(IFNULL(r.qty,0)) <> SUM(IFNULL(i.qty,0)) + SUM(IFNULL(o.qty,0)); ");

            var queryList = base.Context.Database.SqlQuery<DiffReceiptInvOut>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;

        }

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <returns></returns>
        public List<InvAndPickDetailAllocatedQty> GetDiffInvAndPickDetailAllocatedQty(int type)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(@" select illl.SkuSysId,s.SkuCode,s.SkuName ,IFNULL(illl.qty,0) as InventoryAllocation,
                            IFNULL(pd.qty,0) as PickingDetails from
                          (select skusysid,sum(AllocatedQty) as qty from invLot
                          group by skusysid) illl
                          left join 
                          (select skusysid,sum(qty) as qty from pickdetail where status = 10
                          group by skusysid) pd on illl.skusysid = pd.skusysid
                          left join sku s on illl.skusysid = s.sysid
                          where illl.qty <> pd.qty;");
            var queryList = base.Context.Database.SqlQuery<InvAndPickDetailAllocatedQty>(sqlStr.ToString()).AsQueryable();

            var list = queryList.ToList();
            return list;
        }
    }
}
