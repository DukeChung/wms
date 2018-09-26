using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using FortuneLab.MessageQueue.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.DomainEvent
{
    public abstract class MessageQueueEventHandler<TEventData> : EventHandler, IEventHandler<TEventData>, ITransientDependency
    {
        protected abstract string MessageQueueName { get; }
        public void HandleEvent(TEventData eventData)
        {
            var resultData = BuildMessageQueueData(eventData);
            PublishToQueue(resultData);
        }

        protected abstract object BuildMessageQueueData(TEventData eventData);

        protected virtual void PublishToQueue(object eventData)
        {
            Task.Run(() =>
            {
                try
                {   
                    var channel = ConnectionManager.Instance.GetCurrentModel();
                    channel.QueueDeclare(queue: MessageQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "", routingKey: MessageQueueName, basicProperties: properties, body: MessageHelper.GetMessageBody(eventData));
                }
                catch (Exception)
                {
                    //吃掉异常，后续做日志记录
                }
            });
        }
    }
}
