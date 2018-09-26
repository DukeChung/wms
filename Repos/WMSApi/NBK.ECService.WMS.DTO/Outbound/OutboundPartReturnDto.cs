using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundPartReturnDto: BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 选择退货仓库
        /// </summary>
        public Guid SelectWarehouseSysId { get; set; }

        public List<OutboundPartDetail> OutboundPartDetailList { get; set; }
    }

    public class OutboundPartDetail
    {
        
        public Guid SkuSysId { get; set; }

        public decimal ReturnQty { get; set; }
    }
}
