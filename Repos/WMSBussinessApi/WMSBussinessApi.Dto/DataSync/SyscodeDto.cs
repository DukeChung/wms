using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Dto.DataSync
{
    public class SyscodeDto
    {
        public Guid SysId { get; set; }
        public string SysCodeType { get; set; }
        public string Descr { get; set; }
    }
}
