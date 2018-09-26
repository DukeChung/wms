using System;

namespace NBK.ECService.WMSReport.DTO.Base
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
