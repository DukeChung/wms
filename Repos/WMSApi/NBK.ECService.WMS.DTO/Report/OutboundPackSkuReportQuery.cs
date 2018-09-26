using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Report
{
    public class OutboundPackSkuReportQuery : BaseQuery
    {
        public string OutboundOrder { get; set; }

        public string TransferOrder { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }

        public DateTime? ActualShipDateFrom { get; set; }

        public DateTime? ActualShipDateTo { get; set; }
    }


    public class OutboundPackSkuReportDto
    {
        public int Index { get; set; }
        public string WarehouseName { get; set; }

        public string Channel { get; set; }

        public string OutboundOrder { get; set; }

        public string ServiceStationName { get; set; }

        public int packCount { get; set; }

        public int TransferType { get; set; }

        public int OutboundQty { get; set; }

        public DateTime ActualShipDate { get; set; }

        public string CreateUserName { get; set; }

        public string TransferOrder { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int Qty { get; set; }

        public string TransferTypeDisplay { get { return ((OutboundTransferOrderType)TransferType).ToDescription(); } }

        public string ActualShipDateDisplay { get { return ActualShipDate.ToString(PublicConst.DateTimeFormat); } }
    }
}
