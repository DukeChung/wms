using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class InvTranDto
    {
        public Guid SysId { get; set; }

        public string DocOrder { get; set; }

        public Guid DocSysId { get; set; }

        public string TransType { get; set; }

        public string TransTypeText
        {
            get
            {
                string val = string.Empty;
                switch (TransType)
                {
                    case InvTransType.Inbound:
                        val = "入库";
                        break;
                    case InvTransType.Outbound:
                        val = "出库";
                        break;
                    case InvTransType.Adjustment:
                        val = "调整";
                        break;
                    case InvTransType.Assembly:
                        val = "加工";
                        break;
                    default:
                        break;
                }
                return val;
            }
        }

        public int Qty { get; set; }

        public int AvailableQty { get; set; }

        public int? LockedQty { get; set; }

        public int? OccupiedQty { get; set; }

        public decimal DisplayQty { get; set; }

        public decimal DisplayAvailableQty { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string LotAttr01 { get; set; }

        public string SkuName { get; set; }
        public Guid SkuSysId { get; set; }

        public string CreateDateText { get { return CreateDate.HasValue ? CreateDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }
    }
}
