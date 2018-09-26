using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class InvTransBySkuReportDto
    {
        public Guid SysId { get; set; }
        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string SkuDescr { get; set; }
        public string SkuCode { get; set; }

        public string TypeDisplay { get; set; }

        public int PackQty { get; set; }

    }


    public class InvTransSkuListReportDto
    {
        public List<InvTransBySkuReportDto> InvTransBySkuReportDto { get; set; }
        public Pages<InvTranDto> InvTranDtoList { get; set; }
    }
}
