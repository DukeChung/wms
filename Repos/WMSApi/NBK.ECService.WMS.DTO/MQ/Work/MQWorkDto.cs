using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.MQ
{
    [Serializable]
    [DataContract]
    public class MQWorkDto
    {
        [DataMember]
        public int WorkBusinessType { get; set; }

        [DataMember]
        public int WorkType { get; set; }

        [DataMember]
        public int CurrentUserId { get; set; }

        [DataMember]
        public string CurrentDisplayName { get; set; }

        [DataMember]
        public Guid WarehouseSysId { get; set; }

        [DataMember]
        public List<WorkDetailDto> WorkDetailDtoList { get; set; }

        [DataMember]
        public CancelWorkDto CancelWorkDto { get; set; }
    }
}
