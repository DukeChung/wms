using FortuneLab.ErrorCodes;
using FortuneLab.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Exceptions
{
    /// <summary>
    /// 所有的业务异常类
    /// </summary>
    public class BusinessException : FortuneLabException
    {
        public BusinessException()
        {

        }

        public BusinessException(string errorCode, string message)
            : base(errorCode, message)
        {
        }

        public BusinessException(ErrorMessage errorMessage)
            : this(errorMessage.Code, errorMessage.Msg)
        {

        }

        public BusinessException(string errorResourceKey)
            : this(ResourceHelper.GetMessage(errorResourceKey))
        {

        }
    }
}
