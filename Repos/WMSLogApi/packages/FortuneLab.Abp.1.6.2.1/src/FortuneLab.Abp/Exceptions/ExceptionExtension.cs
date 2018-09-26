using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ExceptionExtension
    {
        public static string FullMessage(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            if (ex.InnerException == null)
                return ex.Message;
            else
                return string.Format("{0} -> {1}", ex.Message, ex.InnerException.FullMessage());
        }

        public static string FullStackTrace(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            if (ex.InnerException == null)
                return ex.StackTrace;
            else
                return string.Format("{0} -> {1}", ex.StackTrace, ex.InnerException.FullStackTrace());
        }
    }
}
