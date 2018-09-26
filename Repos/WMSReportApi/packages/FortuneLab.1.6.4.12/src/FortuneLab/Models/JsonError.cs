using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Models
{
    /// <summary>
    /// UI所有Json Error类， 默认按照集合处理
    /// </summary>
    public class JsonError
    {
        public List<ErrorItem> Errors { get; set; }

        public JsonError()
        {
            this.Errors = new List<ErrorItem>();
        }

        public JsonError(string errorCode, string errorMessage)
            : this()
        {
            this.AddError(errorCode, errorMessage);
        }

        public void AddError(string errorCode, string errorMessage)
        {
            this.AddError(new ErrorItem { errorCode = errorCode, errorMessage = errorMessage });
        }

        public void AddError(ErrorItem errorItem)
        {
            this.Errors.Add(errorItem);
        }
    }
}
