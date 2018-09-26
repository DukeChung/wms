using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class CoordinateDto
    {
        public int Status { get; set; }
        public Result Result { get; set; }
    }


    public class Result
    {
        public locationDto location { get; set; }

        public string precise { get; set; }

        public string confidence { get; set; }

        public string level { get; set; }
    }

    public class locationDto
    {
        public string lng { get; set; }

        public string lat { get; set; }
    }

}
