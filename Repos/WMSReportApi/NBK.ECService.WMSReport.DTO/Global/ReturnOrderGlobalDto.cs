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
    public class ReturnOrderGlobalDto
    {
        public string PurchaseOrder { get; set; }

        public string CreateUserName { get; set; }

        public string OutboundOrder { get; set; }

        public int skuGroupCount { get; set; }

        public int skuTotalCount { get; set; }

        public int? OutboundType { get; set; }

        public DateTime CreateDate { get; set; }

        public string PurchaseWarehouse { get; set; }

        public string OutboundWarehouse { get; set; }

        public string OutboundTypeDisplay
        {
            get
            {
                if (OutboundType == null)
                {
                    return string.Empty;
                }
                return ((OutboundType)OutboundType).ToDescription();
            }
        }

        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.ToString(PublicConst.DateTimeFormat);
            }
        }
    }

    public class ReturnOrderGlobalQuery : BaseQuery
    {
        public Guid SearchWarehouseSysId { get; set; }

        public int? OutboundType { get; set; }

        public string PurchaseOrder { get; set; }

        public string OutboundOrder { get; set; }
    }
}
