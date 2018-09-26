using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public class transferinventoryreceiptextend : SysIdEntity
    {
        public Guid SkuSysId { get; set; }

        public Guid PurchaseSysId { get; set; }

        public Guid WarehouseSysId { get; set; }

        public int Qty { get; set; }

        public int ReceivedQty { get; set; }

        public string Lot { get; set; }

        public string LotAttr01 { get; set; }

        public string LotAttr02 { get; set; }

        public string LotAttr03 { get; set; }

        public string LotAttr04 { get; set; }

        public string LotAttr05 { get; set; }

        public string LotAttr06 { get; set; }

        public string LotAttr07 { get; set; }

        public string LotAttr08 { get; set; }

        public string LotAttr09 { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? ProduceDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string ExternalLot { get; set; }
    }
}
