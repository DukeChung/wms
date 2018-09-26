using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace FortuneLab.MessageQueue.RabbitMq
{
    public class RabbitMqChannelPool
    {
        private readonly ConcurrentQueue<IModel> _idleChannelQueue = null;
        private static readonly RabbitMqChannelPool Instance = new RabbitMqChannelPool();
        private const int MaxThreadQty = 10;

        private readonly Semaphore _semaphore = null;

        private RabbitMqChannelPool()
        {
            _idleChannelQueue = new ConcurrentQueue<IModel>();
            _semaphore = new Semaphore(MaxThreadQty, MaxThreadQty);
        }

        static RabbitMqChannelPool()
        {

        }

        public static void PubMessage(Action<IModel> action)
        {
            Instance.PubMessageInternal(action);
        }

        private void PubMessageInternal(Action<IModel> action)
        {
            _semaphore.WaitOne();
            IModel channel;
            if (!_idleChannelQueue.TryDequeue(out channel))
            {
                channel = ConnectionManager.Instance.GetNewChannel();
                Console.WriteLine("Create a new Channel");
            }
            action(channel);
            _idleChannelQueue.Enqueue(channel);
            _semaphore.Release();
            Console.WriteLine("IdleQueue Qty:" + _idleChannelQueue.Count);
        }

        public static void Dispose()
        {
            IModel channel;
            while (Instance._idleChannelQueue.TryDequeue(out channel))
            {
                channel.Dispose();
                channel.Close();
            }
        }
    }
}
