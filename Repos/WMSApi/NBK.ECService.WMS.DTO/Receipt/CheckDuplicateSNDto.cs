using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Receipt
{
    public class CheckDuplicateSNDto : BaseDto
    {
        public List<string> DuplicateList { get; set; } = new List<string>();

        public List<string> NotExistsList { get; set; } = new List<string>();

        public List<string> OutboundList { get; set; } = new List<string>();

        public List<string> NormalList { get; set; } = new List<string>();
    }
}
