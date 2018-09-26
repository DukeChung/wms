using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class SummaryLogDto
    {
        public string LogName { get; set; }
        public int LogType { get; set; }
        public Guid SysId { get; set; }
        public string Descr { get; set; }
        public bool IsSuccess { get; set; }
        public decimal? ElapsedTime { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
