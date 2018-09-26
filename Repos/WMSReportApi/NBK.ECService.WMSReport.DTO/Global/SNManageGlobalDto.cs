using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class SNManageGlobalDto
    {
        public Guid SysId { get; set; }

        public string SN { get; set; }

        public string PurchaseOrder { get; set; }

        public string OutboundOrder { get; set; }

        public int Status { get; set; }

        public string DisplayStatus { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string DisplayPurchaseDate { get; set; }

        public DateTime? OutboundDate { get; set; }

        public string DisplayOutboundDate { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneePhone { get; set; }


        public string ConsigneeAddress { get; set; }

        public string WarehouseName { get; set; }
    }
}
