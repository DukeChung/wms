using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFPreBulkPackDto
    {
        public RFCommResult RFCommResult { get; set; }

        public List<RFPreBulkPackDetailDto> PreBulkPackNoScan { get; set; }

        public List<RFPreBulkPackDetailDto> PreBulkPackDetails { get; set; }
    }
}
