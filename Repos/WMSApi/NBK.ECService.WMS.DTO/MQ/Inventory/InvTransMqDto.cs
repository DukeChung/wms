using System;
using System.Runtime.Serialization;

namespace NBK.ECService.WMS.DTO.MQ.Inventory
{
    [Serializable]
    [DataContract]
    public class InvTransMqDto
    {
        [DataMember]
        public Guid InvTransSysId { get; set; }
        [DataMember]
        public Guid WareHouseSysId { get; set; }
        [DataMember]
        public string DocOrder { get; set; }
        [DataMember]
        public Guid DocSysId { get; set; }
        [DataMember]
        public Guid DocDetailSysId { get; set; }
        [DataMember]
        public Guid SkuSysId { get; set; }
        [DataMember]
        public string TransType { get; set; }
        [DataMember]
        public string SourceTransType { get; set; }
        [DataMember]
        public int Qty { get; set; }
        [DataMember]
        public string Loc { get; set; }
        [DataMember]
        public string Lot { get; set; }
        [DataMember]
        public string Lpn { get; set; }
        [DataMember]
        public string ToLoc { get; set; }
        [DataMember]
        public string ToLot { get; set; }
        [DataMember]
        public string ToLpn { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public long CreateBy { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
        [DataMember]
        public string UpdateUserName { get; set; }
        [DataMember]
        public string CreateUserName { get; set; }
    }
}