using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Exceptions
{
    /// <summary>
    /// 框架异常
    /// </summary>
    public class FortuneLabException : SystemException
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorCode { get; set; }

        public FortuneLabException()
        {

        }

        public FortuneLabException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
