using System;
using Common.Logging;
using Quartz;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBK.WMSJob
{
    public class UpdateOutboundJob : IJob
    {
        public UpdateOutboundJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(UpdateOutboundJob)).InfoFormat("{0}/异步更新出库单经纬度开始", DateTime.Now);
                var outCount = WMSApiClient.GetInstance().GetOutboundNoLngLatCount();
                if (outCount > 0)
                {
                    LogManager.GetLogger(typeof(UpdateOutboundJob)).InfoFormat("{0}/总共需要更新：{1} 条", DateTime.Now, outCount);

                    var page = Math.Ceiling(outCount / 30m);
                    for (int i = 0; i < page; i++)
                    {
                        try
                        {
                            WMSApiClient.GetInstance().UpdataOutboundLngLat(30);
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetLogger(typeof(UpdateOutboundJob)).InfoFormat("{0}/第{1}次更新经纬度报错：{2}", DateTime.Now, i + 1, ex.Message);
                        }
                    }
                }
                LogManager.GetLogger(typeof(UpdateOutboundJob)).InfoFormat("{0}/更新出库单经纬度结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(UpdateOutboundJob)).InfoFormat("{0} 更新出库单经纬度错误：{1}", DateTime.Now, ex.Message);
            }
        }
    }
}
