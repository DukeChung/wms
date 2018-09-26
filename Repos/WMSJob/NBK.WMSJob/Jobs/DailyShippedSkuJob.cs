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
    public class DailyShippedSkuJob : IJob
    {
        public static string mailTo = ConfigurationManager.AppSettings["DailyShippedSkuMailTo"].ToString();

        public DailyShippedSkuJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}/每日发货明细开始", DateTime.Now);

                var attachments = new List<Attachment>();

                //组织数据
                var ds = StockDAL.GetDailyShippedSku();
                if (ds != null && ds.Tables.Count > 0)
                {
                    var workbook = new HSSFWorkbook();
                    var table = workbook.CreateSheet("dailyShippedSku");

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
                    var r = 1;
                    for (var i = 0; i < ds.Tables.Count; i++)
                    {
                        for (var j = 0; j < ds.Tables[i].Rows.Count; j++)
                        {
                            var dataRow = table.CreateRow(r);
                            for (var k = 0; k < ds.Tables[i].Columns.Count; k++)
                            {
                                var cell = dataRow.CreateCell(k);
                                cell.SetCellValue(ds.Tables[i].Rows[j][k].ToString());
                            }
                            r++;
                        }
                    }
                    #endregion

                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\reports\\dailyShippedSku.xls";
                    using (var fs = File.OpenWrite(path))
                    {
                        workbook.Write(fs);
                        LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}/写入每日发货明细Excel成功", DateTime.Now);
                        fs.Close();
                    }

                    //增加附件
                    attachments.Add(new Attachment(path));
                }

                var daily = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                var result = MailHelper.SendMail(daily + "发货明细", daily + "发货明细", mailTo, attachments);
                if (result == "Y")
                {
                    LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}/邮件发送成功", DateTime.Now);
                }
                else
                {
                    LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}/邮件发送失败,失败原因:{1}", DateTime.Now, result);
                }

                LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}/每日发货明细结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(DailyShippedSkuJob)).InfoFormat("{0}每日发货明细出错：{1}", DateTime.Now, ex.Message);
            }
        }
    }
}
