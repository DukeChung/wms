using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundPackGlobalQuery : BaseQuery
    {
        public string OutboundOrder { get; set; }

        public DateTime? ActualShipDateFrom { get; set; }

        public DateTime? ActualShipDateTo { get; set; }

        public Guid SearchWarehouseSysId { get; set; }
    }

    public class OutboundPackGlobalDto
    {
        public string WarehouseName { get; set; }

        public string Channel { get; set; }

        public string OutboundOrder { get; set; }

        public string ServiceStationName { get; set; }

        public int packCount { get; set; }

        public int TransferType { get; set; }

        public int OutboundQty { get; set; }

        public DateTime ActualShipDate { get; set; } 


        public string ActualShipDateDisplay
        {
            get
            {   
                return ActualShipDate.ToString(PublicConst.DateFormat);
            }
        } 

        public string CreateUserName { get; set; }

        public string TransferOrder { get; set; }  

        public string TransferTypeDisplay { get { return ((OutboundTransferOrderType)TransferType).ToDescription(); } }

    }
}
