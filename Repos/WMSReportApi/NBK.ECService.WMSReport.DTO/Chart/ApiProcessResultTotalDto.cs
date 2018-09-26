using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class ApiProcessResultTotalDto
    {
        public List<ApiProcessTotalDto> SuccessList { get; set; } = new List<ApiProcessTotalDto>();

        public List<ApiProcessTotalDto> ErrorList { get; set; } = new List<ApiProcessTotalDto>();
    }

    public class ApiProcessTotalDto
    {
        public int minutes { get; set; }

        public string DisplayDate { get; set; }

        public int ProcessCount { get; set; }
    }
}
