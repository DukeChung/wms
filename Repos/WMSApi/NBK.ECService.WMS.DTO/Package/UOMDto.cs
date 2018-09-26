using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class UOMDto
    {
        public Guid SysId { get; set; }
        public string UOMCode { get; set; }
        public string Descr { get; set; }
        public string UomType { get; set; }

        public string UomTypeName { get; set; }
    }
}
