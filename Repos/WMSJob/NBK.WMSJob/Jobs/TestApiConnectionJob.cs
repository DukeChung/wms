using System;
using Common.Logging;
using Quartz;
using System.Configuration;

namespace NBK.WMSJob
{
    public class TestApiConnectionJob : IJob
    { 
        public static string mailTo = ConfigurationManager.AppSettings["MailTo"].ToString();

        public TestApiConnectionJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(TestApiConnectionJob)).InfoFormat("{0}/测试Api连接开始", DateTime.Now);

                var rsp = WMSApiClient.GetInstance().TestConnection();
                if (!rsp)
                {
                    var result = MailHelper.SendMail("测试Api连接失败", "测试Api连接失败", mailTo, null);
                    if (result == "Y")
                    {
                        LogManager.GetLogger(typeof(TestApiConnectionJob)).InfoFormat("{0}/测试Api连接邮件发送成功", DateTime.Now);
                    }
                    else
                    {
                        LogManager.GetLogger(typeof(TestApiConnectionJob)).InfoFormat("{0}/测试Api连接邮件发送失败,失败原因:{1}", DateTime.Now, result);
                    }
                }

                LogManager.GetLogger(typeof(TestApiConnectionJob)).InfoFormat("{0}/测试Api连接结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(TestApiConnectionJob)).InfoFormat("{0}测试Api连接出错：{1}", DateTime.Now, ex.Message);
            }
        }

    }
}
