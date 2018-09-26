using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkQueryDto : BaseQuery
    {
        public string WorkOrder { get; set; }
        public string DocOrder { get; set; }
        public string AppointUserName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? WorkType { get; set; }
    }
}
