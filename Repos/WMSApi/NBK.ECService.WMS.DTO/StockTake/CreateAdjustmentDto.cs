using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CreateAdjustmentDto
    {
        public string SourceOrder { get; set; }

        public Guid WarehouseSysId { get; set; }

        public List<Guid> DetailSysIds { get; set; }

        /// <summary>
        /// 损益类型
        /// </summary>
        public int Type { get; set; }
    }
}
