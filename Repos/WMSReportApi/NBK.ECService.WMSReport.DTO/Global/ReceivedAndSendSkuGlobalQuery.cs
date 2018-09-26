using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReceivedAndSendSkuGlobalQuery: BaseQuery
    {

        public Guid SearchWarehouseSysId { get; set; }
        /// <summary>
        /// 仓库所在区域
        /// </summary>
        public string WareHouseArea { get; set; }
        /// <summary>
        /// 仓库性质
        /// </summary>
        public string WareHouseProperty { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        public List<Guid> SeachWarehouseSysIdList { get; set; }
    }
}
