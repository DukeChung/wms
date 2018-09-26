using System;
using Common.Logging;
using Quartz;
using NBK.WMSJob.DAL;
using System.Text;
using System.Data;
using System.Configuration;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;

namespace NBK.WMSJob
{
    public class InventoryJob : IJob
    {
        public static string mailTo = ConfigurationManager.AppSettings["InvSkuReportMailTo"].ToString();

        public InventoryJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}/库存汇总报告开始", DateTime.Now);

                var attachments = new List<Attachment>();

                //组织数据
                var ds = StockDAL.GetInvSkuReport();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var workbook = new HSSFWorkbook();
                    var table = workbook.CreateSheet("invSkuReport");

                    //设置列宽
                    table.SetColumnWidth(1, 30 * 256);
                    table.SetColumnWidth(2, 60 * 256);

                    #region 组织列头
                    var row = table.CreateRow(0);
                    for (var i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        var cell = row.CreateCell(i);
                        cell.SetCellValue(ds.Tables[0].Columns[i].ColumnName);
                    }
                    #endregion

                    #region 组织数据
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var dataRow = table.CreateRow(i + 1);
                        for (var j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            var cell = dataRow.CreateCell(j);
                            cell.SetCellValue(ds.Tables[0].Rows[i][j].ToString());
                        }
                    }
                    #endregion

                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\reports\\invSkuReport.xls";

                    using (var fs = File.OpenWrite(path))
                    {
                        workbook.Write(fs);   
                        LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}/写入库存汇总报告Excel成功", DateTime.Now);
                        fs.Close();
                    }

                    //增加附件
                    attachments.Add(new Attachment(path));
                }

                var result = MailHelper.SendMail("库存汇总报告", "库存汇总报告", mailTo, attachments);
                if (result == "Y")
                {
                    LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}/邮件发送成功", DateTime.Now);
                }
                else
                {
                    LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}/邮件发送失败,失败原因:{1}", DateTime.Now, result);
                }

                LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}/库存汇总报告结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(InventoryJob)).InfoFormat("{0}库存汇总报告出错：{1}", DateTime.Now, ex.Message);
            }
        }
    }
}
