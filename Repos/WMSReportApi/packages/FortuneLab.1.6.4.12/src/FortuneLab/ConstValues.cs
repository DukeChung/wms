using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab
{
    public class ApiTypeAttribute : Attribute
    {
        public ApiTypeEnum ApiType { get; set; }
    }

    public enum ApiTypeEnum
    {
        平台 = 1,
        西安 = 2
    }
}
