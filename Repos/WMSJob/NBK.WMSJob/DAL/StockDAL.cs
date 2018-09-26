using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace NBK.WMSJob.DAL
{
    public class StockDAL
    {
        private static string connString = ConfigurationManager.ConnectionStrings["nbk_wmsContext"].ToString();
        private static string connString1 = ConfigurationManager.ConnectionStrings["nbk_wmsContext_1"].ToString();
        private static string connString2 = ConfigurationManager.ConnectionStrings["nbk_wmsContext_2"].ToString();

        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockDataSet()
        {
            var strSql = @"select il.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,il.qty as invLot数量,isl.qty as invskuLoc数量,illl.qty as invLotLocLpn数量 from
                          (select skusysid,sum(qty) as qty from invLot
                          group by skusysid) il
                          left join 
                          (select skusysid,sum(qty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join 
                          (select skusysid,sum(qty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 查询有差异的库存可用数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockAvailableQty()
        {
            var strSql = @"select il.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,il.qty as invLot可用数量,isl.qty as invskuLoc可用数量,illl.qty as invLotLocLpn可用数量 from
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invLot
                          group by skusysid) il
                          left join 
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join 
                          (select skusysid,sum(qty-AllocatedQty-PickedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockAllocatedQty()
        {
            var strSql = @"select il.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,il.qty as invLot分配数量,isl.qty as invskuLoc分配数量,illl.qty as invLotLocLpn分配数量 from
                          (select skusysid,sum(AllocatedQty) as qty from invLot
                          group by skusysid) il
                          left join 
                          (select skusysid,sum(AllocatedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join 
                          (select skusysid,sum(AllocatedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockPickedQty()
        {
            var strSql = @"select il.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,il.qty as invLot拣货数量,isl.qty as invskuLoc拣货数量,illl.qty as invLotLocLpn拣货数量 from
                          (select skusysid,sum(PickedQty) as qty from invLot
                          group by skusysid) il
                          left join 
                          (select skusysid,sum(PickedQty) as qty from invskuLoc
                          group by skusysid) isl on il.skusysid = isl.skusysid
                          left join 
                          (select skusysid,sum(PickedQty) as qty from invlotloclpn
                          group by skusysid) illl on il.skusysid = illl.skusysid
                          left join sku s on il.skusysid = s.sysid
                          where il.qty != isl.qty or il.qty != illl.qty or isl.qty != illl.qty";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockSkuLotQty()
        {
            var strSql = @"select il.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,il.Lot AS 批次号,il.qty as invLot数量,illl.qty as invLotLocLpn数量 from
                              (select skusysid,Lot,sum(qty) as qty from invLot
                              group by SkuSysId,lot) il
                              left join 
                              (select skusysid,Lot,sum(qty) as qty from invlotloclpn
                              group by SkuSysId,lot) illl on il.skusysid = illl.SkuSysId AND il.lot = illl.lot
                              left join sku s on il.skusysid = s.sysid
                              where il.qty != illl.qty";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockSkuLocQty()
        {
            var strSql = @"select illl.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,illl.loc as 货位,illl.qty as invLot数量,isl.qty as invskuLoc数量 from
                              (select skusysid,loc,sum(qty) as qty from invlotloclpn
                              group by skusysid,loc) illl
                              left join
                              (select skusysid,loc,sum(qty) as qty from invskuLoc
                              group by skusysid,loc) isl on isl.skusysid = illl.skusysid and isl.loc = illl.loc
                              left join sku s on illl.skusysid = s.sysid
                              where isl.qty != illl.qty ";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 查询入库，库存，出库的差异
        /// </summary>
        /// <returns></returns>
        public static DataSet GetDiffReceiptInvOut()
        {
            var strSql = @"SELECT r.SkuSysId,s.SkuCode as 商品编号,s.SkuName as 商品名称,SUM(IFNULL(r.qty,0)) AS 入库数量,SUM(IFNULL(i.qty,0)) AS 库存数量,
                        SUM(IFNULL(o.qty,0)) AS 出库数量,SUM(IFNULL(r.qty,0) - IFNULL(i.qty,0) - IFNULL(o.qty,0)) AS 差异数量 from
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
                          HAVING SUM(IFNULL(r.qty,0)) <> SUM(IFNULL(i.qty,0)) + SUM(IFNULL(o.qty,0));";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <returns></returns>
        public static DataSet GetDiffInvAndPickDetailAllocatedQty()
        {
            var strSql = @"select illl.SkuSysId,s.skucode as 商品编号,s.SkuName as 商品名称,illl.qty as 库存分配数量,pd.qty as 拣货明细分配数量 from
                          (select skusysid,sum(AllocatedQty) as qty from invLot
                          group by skusysid) illl
                          left join 
                          (select skusysid,sum(qty) as qty from pickdetail where status = 10
                          group by skusysid) pd on illl.skusysid = pd.skusysid
                          left join sku s on illl.skusysid = s.sysid
                          where illl.qty <> pd.qty ";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 库存汇总报告
        /// </summary>
        /// <returns></returns>
        public static DataSet GetInvSkuReport()
        {
            var strSql = @"SELECT s.otherId AS 商品外部Id,s.upc AS 商品条码,s.SkuName AS 商品名称,w.Name as 仓库,
                               SUM(CASE WHEN ((((p.InLabelUnit01 IS NOT NULL) 
                                             AND ((1 = p.InLabelUnit01)
                                             AND (p.InLabelUnit01 IS NOT NULL)))
                                             AND (p.FieldValue01 > 0))
                                             AND (p.FieldValue02 > 0)) THEN (ROUND(((p.FieldValue02 * inv.Qty) * 1.00) / (p.FieldValue01), 2))
                                             ELSE (inv.Qty)
                                             END) AS 数量
                              FROM invLot inv  
                              inner join sku s on inv.SkuSysId = s.SysId 
                              left join warehouse w  on inv.warehouseSysId = w.SysId 
                              left join pack p on p.sysid = s.packsysid
                              GROUP by s.otherId,s.upc,s.SkuName,w.Name";

            return DbHelperDataSet(strSql);
        }

        /// <summary>
        /// 每日发货明细
        /// </summary>
        /// <returns></returns>
        public static DataSet GetDailyShippedSku()
        {
            DataSet ds = new DataSet();
            var daily = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            var beginDate = daily + " 00:00:00";
            var endDate = daily + " 23:59:59";

            var strSql = string.Format(@"SELECT w.Name as 仓库,o.ServiceStationCode AS 服务站编码, o.ServiceStationName as 服务站名称, o.OutboundOrder as 出库单号,s.UPC as 商品条码,s.SkuName as 商品名称,od.ShippedQty as 发货数量 FROM outbound o
                                LEFT JOIN outbounddetail od ON od.OutboundSysId = o.SysId
                                LEFT JOIN warehouse w ON w.SysId = o.WareHouseSysId
                                LEFT JOIN sku s ON s.SysId = od.SkuSysId
                                WHERE o.status = 70 and o.ActualShipDate >= '{0}' AND o.ActualShipDate <= '{1}'
                                ORDER by w.Name,o.OutboundOrder;", beginDate, endDate);

            MySqlConnection myConnection = new MySqlConnection(connString);
            MySqlCommand mySqlCommand = new MySqlCommand(strSql, myConnection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            mySqlDataAdapter.Fill(ds, "Table0");

            MySqlConnection myConnection1 = new MySqlConnection(connString1);
            MySqlCommand mySqlCommand1 = new MySqlCommand(strSql, myConnection1);
            MySqlDataAdapter mySqlDataAdapter1 = new MySqlDataAdapter(mySqlCommand1);
            mySqlDataAdapter1.Fill(ds, "Table1");

            MySqlConnection myConnection2 = new MySqlConnection(connString2);
            MySqlCommand mySqlCommand2 = new MySqlCommand(strSql, myConnection2);
            MySqlDataAdapter mySqlDataAdapter2 = new MySqlDataAdapter(mySqlCommand2);
            mySqlDataAdapter2.Fill(ds, "Table2");

            return ds;
        }

        /// <summary>
        /// 传入sql语句，返回DataSet
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private static DataSet DbHelperDataSet(string strSql)
        {
            MySqlConnection myConnection = new MySqlConnection(connString);
            MySqlCommand mySqlCommand = new MySqlCommand(strSql, myConnection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);

            DataSet ds = new DataSet();
            mySqlDataAdapter.Fill(ds, "Table0");

            return ds;
        }
    }
}
