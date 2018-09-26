using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Models
{
    /// <summary>
    /// 所有客户端异常类
    /// </summary>
    public class ErrorItem
    {
        public string errorCode { get; set; }

        public string errorMessage { get; set; }
    }
}
