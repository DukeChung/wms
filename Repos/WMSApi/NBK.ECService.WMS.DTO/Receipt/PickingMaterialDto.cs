using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class PickingMaterialDto : BaseDto
    {
        /// <summary>
        /// 收货单Id
        /// </summary>
        public Guid ReceiptSysId { get; set; }

        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }

        /// <summary>
        /// 领料分拣时间
        /// </summary>
        public DateTime PickingDate { get; set; }

        /// <summary>
        /// 领料分拣人Id
        /// </summary>
        public int? PickingUserId { get; set; }

        /// <summary>
        /// 领料分拣人
        /// </summary>
        public string PickingUserName { get; set; }

        public List<PickingMaterialDetailDto> PickingMaterialDetailListDto { get; set; }
    }
}
