using System;
using Common.Logging;
using Quartz;
using NBK.WMSJob.DAL;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace NBK.WMSJob
{
    public class CheckStockJob : IJob
    {
        public static string mailTo = ConfigurationManager.AppSettings["MailTo"].ToString();

        public CheckStockJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}/检查库存开始", DateTime.Now);

                //组织数据
                StringBuilder strHtml = new StringBuilder();

                #region 3张库存表Qty数量比较
                var dsStock = StockDAL.GetStockDataSet();
                if (dsStock != null && dsStock.Tables.Count > 0 && dsStock.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>3张库存表Qty数量比较：有差异，差异如下：</b></div>"); 
                    strHtml.Append(GenerateHtmlTable(dsStock));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>3张库存表Qty数量比较：无差异</div>");
                }
                #endregion

                #region 3张库存表可用数量比较
                var dsAvailable = StockDAL.GetStockAvailableQty();
                if (dsAvailable != null && dsAvailable.Tables.Count > 0 && dsAvailable.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>3张库存表可用数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsAvailable));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>3张库存表可用数量比较：无差异</div>");
                }
                #endregion

                #region 3张库存表分配数量比较
                var dsAllocated = StockDAL.GetStockAllocatedQty();
                if (dsAllocated != null && dsAllocated.Tables.Count > 0 && dsAllocated.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>3张库存表分配数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsAllocated));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>3张库存表分配数量比较：无差异</div>");
                }
                #endregion

                #region 3张库存表拣货数量比较
                var dsPicked = StockDAL.GetStockPickedQty();
                if (dsPicked != null && dsPicked.Tables.Count > 0 && dsPicked.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>3张库存表拣货数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsPicked));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>3张库存表拣货数量比较：无差异</div>");
                }
                #endregion

                #region invLot和invLotLocLpn表商品相同批次数量差异
                var dsInvLot = StockDAL.GetStockSkuLotQty();
                if (dsInvLot != null && dsInvLot.Tables.Count > 0 && dsInvLot.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>invLot和invLotLocLpn表商品相同批次数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsInvLot));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>invLot和invLotLocLpn表商品相同批次数量比较：无差异</div>");
                }
                #endregion

                #region invSkuLoc和invLotLocLpn表商品相同货位数量差异
                var dsInvLoc = StockDAL.GetStockSkuLocQty();
                if (dsInvLoc != null && dsInvLoc.Tables.Count > 0 && dsInvLoc.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>invSkuLoc和invLotLocLpn表商品相同货位数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsInvLoc));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>invSkuLoc和invLotLocLpn表商品相同货位数量比较：无差异</div>");
                }
                #endregion

                #region 入库=库存+出库差异比较
                var dsDiffRIO = StockDAL.GetDiffReceiptInvOut();
                if (dsDiffRIO != null && dsDiffRIO.Tables.Count > 0 && dsDiffRIO.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>入库=库存+出库差异比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsDiffRIO));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>入库=库存+出库差异比较：无差异</div>");
                }
                #endregion

                #region 库存分配数量和拣货明细分配数量比较
                var dsInvPickDetail = StockDAL.GetDiffInvAndPickDetailAllocatedQty();
                if (dsInvPickDetail != null && dsInvPickDetail.Tables.Count > 0 && dsInvPickDetail.Tables[0].Rows.Count > 0)
                {
                    strHtml.Append("<div style='margin-top:10px;'><b>库存分配数量和拣货明细分配数量比较：有差异，差异如下：</b></div>");
                    strHtml.Append(GenerateHtmlTable(dsInvPickDetail));
                }
                else
                {
                    strHtml.Append("<div style='margin-top:10px;'>库存分配数量和拣货明细分配数量比较：无差异</div>");
                }
                #endregion

                var result = MailHelper.SendMail("WMS数据检测", strHtml.ToString(), mailTo, null);
                if (result == "Y")
                {
                    LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}/邮件发送成功", DateTime.Now);
                }
                else
                {
                    LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}/邮件发送失败,失败原因:{1}", DateTime.Now, result);
                }

                LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}/检查库存结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}检查库存出错：{1}", DateTime.Now, ex.Message);
            }
        }

        //构造HtmlTable
        private string GenerateHtmlTable(DataSet ds)
        {
            var strTable = new StringBuilder();

            if(ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                strTable.Append("<table style='border-collapse:collapse;border:none;'>");

                #region 表头
                strTable.Append("<tr>");
                if(ds.Tables[0].Columns.Count > 0)
                {
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        strTable.AppendFormat("<td style='border:1px solid #000; padding:5px;text-align:center;'>{0}</td>", dc.ColumnName);
                    }
                    strTable.Append("</tr>");
                }
                #endregion

                #region 表数据
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    strTable.Append("<tr>");
                    for (var i = 0;i< ds.Tables[0].Columns.Count; i++)
                    {
                        strTable.AppendFormat("<td style='border:1px solid #000;padding:5px;text-align:center;'>{0}</td>", dr[i].ToString());
                    }
                    strTable.Append("</tr>");
                }
                #endregion

                strTable.Append("</table>");
            }

            return strTable.ToString();
        }
    }
}
