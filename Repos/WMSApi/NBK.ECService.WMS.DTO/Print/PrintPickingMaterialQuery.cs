using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickingMaterialQuery : BaseQuery
    {
        /// <summary>
        /// 收货单ID
        /// </summary>
        public Guid ReceiptSysId { get; set; }

        /// <summary>
        /// 领料人ID
        /// </summary>
        public Guid PickingUserId { get; set; }

        /// <summary>
        /// 领料人
        /// </summary>
        public string PickingUserName { get; set; }

        public List<string> SysIds { get; set; }
    }
}
