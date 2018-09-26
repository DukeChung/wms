using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Mvc.ViewModels
{
    public class AjaxRequestResult<T>
    {
        /// <summary>
        /// 操作代码, 用来在客户端全局处理
        /// </summary>
        public string typeCode { get; set; }
        public T result { get; set; }

        public AjaxRequestResult(string code, T model)
        {
            this.typeCode = code;
            this.result = model;
        }
    }

    public class ErrorInfo
    {
        public string property { get; set; }
        public List<string> errors { get; set; }
    }
}
