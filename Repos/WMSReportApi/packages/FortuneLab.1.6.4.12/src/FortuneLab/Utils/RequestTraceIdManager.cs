using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Utils
{
    public static class RequestTraceIdManager
    {
        public const string RequestTraceIdKey = "RequestTraceId";
        public static string GetNextRequestTraceId()
        {
            var r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            return $"{DateTime.Now:yyyyMMddHHmmss}{r.Next(100000, 999999)}";
        }
    }
}
