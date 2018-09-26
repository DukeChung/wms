using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PickingOperationDto : BaseDto
    {
        public List<PickingOperationDetail> PickingOperationDetails { get; set; }
    }

    public class PickingOperationDetail
    {
        public Guid SysId { get; set; }

        public string PickDetailOrder { get; set; }

        public string OutboundOrder { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string SkuDescr { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public int PickedQty { get; set; }

        public decimal DisplayPickedQty { get; set; }
    }
}
