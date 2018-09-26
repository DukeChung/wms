using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FortuneLab.Caches
{
    public interface ICommonOjbectCacheProvider
    {
        bool KeyExists(string cacheKey);
        T Get<T>(string cacheKey);
        void Set<T>(string cacheKey, T data, TimeSpan expiry);
        void KeyDelete(string cacheKey);
    }

    public interface ICacheDataSerializer
    {
        byte[] Serialize(object o);
        T Deserialize<T>(byte[] stream);
    }

    public class JsonSerializer : ICacheDataSerializer
    {
        public byte[] Serialize(object o)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o));
            //using (var memoryStream = new MemoryStream())
            //{
            //    Serializer.Serialize(memoryStream, o);

            //    return memoryStream.ToArray();
            //}
        }

        public T Deserialize<T>(byte[] stream)
        {
            //var memoryStream = new MemoryStream(stream);

            //return Serializer.Deserialize<T>(memoryStream);

            var bytes = Encoding.UTF8.GetString(stream);

            return JsonConvert.DeserializeObject<T>(bytes);
        }
    }

    public class FixedInterval
    {
        public FixedInterval(int tryTimes, TimeSpan sleepSpan)
        {
            this.TryTimes = tryTimes;
            this.SleepSpan = sleepSpan;
        }

        public int TryTimes { get; private set; }
        public TimeSpan SleepSpan { get; private set; }
    }

    public class RetryPolicy<TTransientErrorDetectionStrategy>
        where TTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy, new()
    {
        private readonly FixedInterval _retryStrategy;

        private readonly ITransientErrorDetectionStrategy _transientErrorDetectionStrategy = null;

        public RetryPolicy(FixedInterval retryStrategy)
        {
            this._retryStrategy = retryStrategy;
            this._transientErrorDetectionStrategy = new TTransientErrorDetectionStrategy();
        }

        public TResult ExecuteAction<TResult>(Func<TResult> action)
        {
            int tryTimes = 0;
            bool isContinue = false;
            do
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    if (_transientErrorDetectionStrategy.IsTransient(ex))
                    {
                        tryTimes += 1;
                        isContinue = true;
                        Thread.Sleep(_retryStrategy.SleepSpan);
                    }
                    else
                    {
                        isContinue = false;
                    }
                }

            } while (isContinue && tryTimes < _retryStrategy.TryTimes);
            return default(TResult);
        }
    }


    public interface ITransientErrorDetectionStrategy
    {
        bool IsTransient(Exception ex);
    }
}
