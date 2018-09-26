using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class LockOrderInput
    {
        /// <summary>
        /// 10：冻结，20：解冻
        /// </summary>
        public int FreezeType { get; set; }
        public int WarehouseId { get; set; }
        public int ProductCode { get; set; }
        public int Quantity { get; set; }
        public string CreateUserName { get; set; }
        public int CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string EditUserName { get; set; }
        public long? EditUserId { get; set; }
        public DateTime? EditTime { get; set; }
        public string Memo { get; set; }
        public string ChannelTypeText { get; set; }
    }

    public class LockOrderResponse
    {
        public bool Succeeded { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }
    }

    public class LockOrderReleaseDetailInput
    {
        public int WarehouseId { get; set; }
        public Guid? WarehouseSysId { get; set; }
        public Guid? LendOrderSysId { get; set; }
        public int? LendOrderId { get; set; }
        public Guid? ProductSysId { get; set; }
        public int ProductCode { get; set; }
        public int ReturnQuantity { get; set; }
        public int DamageLevel { get; set; }
        public string DamageReason { get; set; }
        public string CreateUserName { get; set; }
        public long? CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string EditUserName { get; set; }
        public long? EditUserId { get; set; }
        public DateTime? EditTime { get; set; }
        public string Memo { get; set; }
        public string SourceNumber { get; set; }
    }
}
