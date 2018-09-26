using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class MaxFrequencyDto
    {
        public List<MaxFrequencyLogDto> MaxFrequencyLogDtoList { get; set; }
        public int Inteval { get; set; }
    }

    public class MaxFrequencyLogDto
    {
        public string Descr { get; set; }
        public int TotalCount { get; set; }
        [JsonIgnore]
        public string DetailLastSysIdStr { get; set; }
        [JsonIgnore]
        public string DetailCountStr { get; set; }
        public List<FrequencyDetail> FrequencyDetails { get; set; }
    }

    public class FrequencyDetail
    {
        public Guid SysId { get; set; }
        public int CountInInteval { get; set; }
        public string Duration { get; set; }
    }
}
