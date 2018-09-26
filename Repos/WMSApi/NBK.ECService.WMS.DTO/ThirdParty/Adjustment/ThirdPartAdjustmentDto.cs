using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartAdjustmentDto
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 损益原因
        /// </summary>
        public string AdjustmentReason { get; set; }

        /// <summary>
        /// WMS损益单号
        /// </summary>
        public string SourceNumber { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public int WarehouseID { get; set; }

        public List<ThirdPartAdjustmentDetailDto> AdjustmentDetailList { get; set; }
    }
}
