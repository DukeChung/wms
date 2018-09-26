using System;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundOperateDto : BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 选择退货仓库
        /// </summary>
        public Guid SelectWarehouseSysId { get; set; }
    }
}
