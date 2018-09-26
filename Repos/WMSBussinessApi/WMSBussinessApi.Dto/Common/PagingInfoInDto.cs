using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Dto.Common
{
    public class PagingInfoInDto
    {
        public int PageSize { get; set; } = 0;
        public int CurrentPage { get; set; } = 1;
    }
}
