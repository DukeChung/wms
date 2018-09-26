using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 仓库服务站关系图
    /// </summary>
    public class WarehouseStationRelationDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string ConsigneeCity { get; set; }
        /// <summary>
        /// 总发货单数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
