using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceivedAndSendSkuReportQuery : BaseQuery
    {
        public Guid? SearchWarehouseSysId { get; set; }
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
