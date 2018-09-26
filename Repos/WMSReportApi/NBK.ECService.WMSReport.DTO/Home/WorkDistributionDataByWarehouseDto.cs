using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class WorkDistributionDataByWarehouseDto
    {
        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string BusinessType { get; set; }

        public List<WorkTimeDataByWarehouseDto> WarehouseData { get; set; } = new List<WorkTimeDataByWarehouseDto>();
    }

    public class WorkTimeDataByWarehouseDto
    {

        /// <summary>
        /// 小时
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// 作业次数
        /// </summary>
        public int Times { get; set; }
    }
}
