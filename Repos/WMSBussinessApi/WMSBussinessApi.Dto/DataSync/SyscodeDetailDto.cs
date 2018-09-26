using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Dto.DataSync
{
    public class SyscodeDetailDto
    {
        public Guid SysId { get; set; }
        public System.Guid SysCodeSysId { get; set; }
        public string SeqNo { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }

        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
    }
}
