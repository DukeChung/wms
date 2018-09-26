using System;

namespace NBK.ECService.WMS.DTO
{
    public class UpdateInventoryDto:BaseDto
    {
        public Guid? InvLotLocLpnSysId { get; set; }

        public Guid? InvLotSysId { get; set; }

        public Guid? InvSkuLocSysId { get; set; }
        /// <summary>
        /// 扣减或者增加的SQL
        /// </summary>
        public int Qty { get; set; }
      
    }
}