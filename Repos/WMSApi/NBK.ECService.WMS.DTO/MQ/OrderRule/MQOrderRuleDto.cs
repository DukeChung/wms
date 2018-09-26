using System;
using System.Runtime.Serialization;

namespace NBK.ECService.WMS.DTO.MQ.OrderRule
{
    [Serializable]
    [DataContract]
    public class MQOrderRuleDto 
    {
        [DataMember]
        public Guid OrderSysId { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public int CurrentUserId { get; set; }
        [DataMember]
        public string CurrentDisplayName { get; set; }
        [DataMember]
        public Guid WarehouseSysId { get; set; }
    }
}