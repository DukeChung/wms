using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundMapData
    {
        public List<BirdsEysSourceDto> CurrentOutbound { get; set; }
        public List<BirdsEysSourceDto> NewOutbound { get; set; }
        public List<BirdsEysSourceDto> BirdsEysSource { get; set; }
    }

    public class BirdsEysSourceDto
    {
        public Guid WareHouseSysId { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string ServiceStationName { get; set; }

    }
}