using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{

    public class WorkDistributionListData
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }

        public List<WorkDistributionDataDto> WorkDistributionDataDto { get; set; } = new List<WorkDistributionDataDto>();
    }
    public class WorkDistributionDataDto
    {
        /// <summary>
        /// 小时
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseSysId { get; set; }

        /// <summary>
        /// 作业次数
        /// </summary>
        public int Times { get; set; }
    }

}
