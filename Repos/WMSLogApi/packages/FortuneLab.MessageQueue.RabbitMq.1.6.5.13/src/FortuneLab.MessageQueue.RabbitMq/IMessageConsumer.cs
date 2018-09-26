using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.MessageQueue.RabbitMq
{
    public interface IMessageConsumer
    {
        string SessionName { get; }
        string ExchangeName { get; }
        string QueueName { get; }
        bool IsStoped { get; }
        void StartConsume(string sessionName);
        void StopConsume();
    }
}
