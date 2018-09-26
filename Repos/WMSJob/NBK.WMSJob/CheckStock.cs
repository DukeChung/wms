using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Common.Logging;
using System.Threading;
using System.Configuration;

namespace NBK.WMSJob
{
    partial class CheckStock : ServiceBase
    {
        public static string stockCheckTimer = ConfigurationManager.AppSettings["StockCheckTimer"];
        public static string InvSkuReportTimer = ConfigurationManager.AppSettings["InvSkuReportTimer"];
        public static string DailyShippedSkuTimer = ConfigurationManager.AppSettings["DailyShippedSkuTimer"];
        public static string TestConnectionTimer = ConfigurationManager.AppSettings["TestConnectionTimer"];
        public static string UpdateReportRedis = ConfigurationManager.AppSettings["UpdateReportRedis"];
        public static string UpdateOutboundLngLat = ConfigurationManager.AppSettings["UpdateOutboundLngLat"];

        IScheduler sched;

        public CheckStock()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                ISchedulerFactory sf = new StdSchedulerFactory();
                sched = sf.GetScheduler();

                #region Check job
                IJobDetail job = new JobDetailImpl("CheckStockJob", "CheckStockGroup", typeof(CheckStockJob));
                string cronExpr = stockCheckTimer;
                ITrigger trigger = new CronTriggerImpl("trigger1", "CheckStockGroup", "CheckStockJob", "CheckStockGroup", cronExpr);
                sched.AddJob(job, true);
                sched.ScheduleJob(trigger);
                #endregion

                #region 库存
                IJobDetail job1 = new JobDetailImpl("InventoryJob", "InventoryJobGroup", typeof(InventoryJob));
                string cronExpr1 = InvSkuReportTimer;
                ITrigger trigger1 = new CronTriggerImpl("trigger2", "InventoryJobGroup", "InventoryJob", "InventoryJobGroup", cronExpr1);
                sched.AddJob(job1, true);
                sched.ScheduleJob(trigger1);
                #endregion

                #region 每日发货明细
                IJobDetail job2 = new JobDetailImpl("DailyShippedSkuJob", "DailyShippedSkuJobGroup", typeof(DailyShippedSkuJob));
                string cronExpr2 = DailyShippedSkuTimer;
                ITrigger trigger2 = new CronTriggerImpl("trigger3", "DailyShippedSkuJobGroup", "DailyShippedSkuJob", "DailyShippedSkuJobGroup", cronExpr2);
                sched.AddJob(job2, true);
                sched.ScheduleJob(trigger2);
                #endregion

                #region 测试Api连接
                IJobDetail job3 = new JobDetailImpl("TestApiConnectionJob", "TestApiConnectionJobGroup", typeof(TestApiConnectionJob));
                string cronExpr3 = TestConnectionTimer;
                ITrigger trigger3 = new CronTriggerImpl("trigger4", "TestApiConnectionJobGroup", "TestApiConnectionJob", "TestApiConnectionJobGroup", cronExpr3);
                sched.AddJob(job3, true);
                sched.ScheduleJob(trigger3);
                #endregion


                #region 更新报表缓存数据
                IJobDetail job4 = new JobDetailImpl("UpdateReportRedisJob", "UpdateReportRedisJobGroup", typeof(UpdateReportRedisJob));
                string cronExpr4 = UpdateReportRedis;
                ITrigger trigger4 = new CronTriggerImpl("trigger5", "UpdateReportRedisJobGroup", "UpdateReportRedisJob", "UpdateReportRedisJobGroup", cronExpr4);
                sched.AddJob(job4, true);
                sched.ScheduleJob(trigger4);
                #endregion


                #region 更新出库单经纬度
                IJobDetail job5 = new JobDetailImpl("UpdateOutboundJob", "UpdateOutboundJobGroup", typeof(UpdateOutboundJob));
                string cronExpr5 = UpdateOutboundLngLat;
                ITrigger trigger5 = new CronTriggerImpl("trigger6", "UpdateOutboundJobGroup", "UpdateOutboundJob", "UpdateOutboundJobGroup", cronExpr5);
                sched.AddJob(job5, true);
                sched.ScheduleJob(trigger5);
                #endregion


                sched.Start();
                LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}服务启动成功", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(CheckStockJob)).InfoFormat("{0}服务启动出错：{1}", DateTime.Now, ex.Message);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        protected override void OnStop()
        {
            sched.Shutdown(true);
        }
    }
}
