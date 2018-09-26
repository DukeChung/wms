using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Exceptions
{
    public class HandleException : Attribute
    {
        public Type ExceptionType { get; private set; }
        public HandleException(Type exceptionType)
        {
            this.ExceptionType = exceptionType;
        }
    }
}
