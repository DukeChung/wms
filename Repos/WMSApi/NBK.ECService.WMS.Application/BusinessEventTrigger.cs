using System;
using FortuneLab.MessageQueue.MessageModels;
using FortuneLab.Runtime.Session;

namespace NBK.ECService.WMS.Application
{
    public static class BusinessEventTrigger
    {
        public const string ExchangeName = "BizSys_LD_EX_Business";

        [Obsolete("使用IMqEventBus替换")]
        public static void Publish<T>(string routingKey, BusinessEventData<T> eventData, bool useThreadPool = true) where T : class
        {
            //var currentUnitOfWork = IocManager.Instance.IocContainer.Resolve<IUnitOfWorkManager>().Current;
            //currentUnitOfWork.Completed += (o, s) =>
            //{
            FortuneLab.ECService.Application.Services.BusinessEventTrigger.Publish<T>(ExchangeName, routingKey, eventData, useThreadPool);
            //};
        }
    }

    public interface IMqEventBus
    {
        void Publish<T>(string routingKey, BusinessEventData<T> eventData, bool useThreadPool = true) where T : class;
    }

    public class MqEventBus : IMqEventBus
    {
        public const string ExchangeName = "BizSys_LD_EX_Business";
        private readonly IFortuneLabSession _fortuneLabSession;

        public MqEventBus(IFortuneLabSession fortuneLabSession)
        {
            this._fortuneLabSession = fortuneLabSession;
        }

        public void Publish<T>(string routingKey, BusinessEventData<T> eventData, bool useThreadPool = true) where T : class
        {
            eventData.UserId = _fortuneLabSession.Identity.UserId;
            eventData.UserName = _fortuneLabSession.Identity.Name;
            eventData.SessionKey = _fortuneLabSession.Identity.SessionKey;
            FortuneLab.ECService.Application.Services.BusinessEventTrigger.Publish<T>(ExchangeName, routingKey, eventData, useThreadPool);
        }
    }
}