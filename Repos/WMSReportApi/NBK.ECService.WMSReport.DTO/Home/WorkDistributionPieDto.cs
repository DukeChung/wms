using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class WorkDistributionPieDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 总作业数次数
        /// </summary>
        public int AllTims { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<WorkDistributionPieData> WorkPieData { get; set; } = new List<WorkDistributionPieData>();
    }

    public class WorkDistributionPieData
    {
        /// <summary>
        /// 作业类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 作业次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public Guid WarehouseSysId { get; set; }

    }
}
