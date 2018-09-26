using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Dto.XXX
{
    public class Report1OutDto
    {
        public List<Report1Item> ReportData { get; set; }
        public int ResultTotal { get; set; }
    }

    public class Report1Item
    {
        public string Date { get; set; }
        public string XingMing { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Zip { get; set; }
    }
}
