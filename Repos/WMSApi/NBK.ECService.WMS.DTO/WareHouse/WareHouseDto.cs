using System;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class WareHouseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }

        public string OtherId { get; set; }
    }
}
