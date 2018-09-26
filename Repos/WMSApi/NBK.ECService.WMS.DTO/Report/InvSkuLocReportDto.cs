using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class InvSkuLocReportDto
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string Loc { get; set; }

        public int Qty { get; set; }

        /// <summary>
        /// 外部Id
        /// </summary>
        public string OtherId { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        public int AllocatedQty { get; set; }

        public int PickedQty { get; set; }

        public int FrozenQty { get; set; }

        public decimal DisplayQty { get; set; }

        public decimal DisplayAllocatedQty { get; set; }

        public decimal DisplayPickedQty { get; set; }

        public decimal DisplayFrozenQty { get; set; }

        public DateTime? UpdateDate { get; set; }
    }

    public class InvSkuLocReportQuery : BaseQuery
    {
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string Loc { get; set; }
        /// <summary>
        /// 外部Id
        /// </summary>
        public string OtherId { get; set; }
        /// <summary>
        /// 是否原材料
        /// </summary>
        public bool? IsMaterial { get; set; }
        /// <summary>
        /// 是否库存为零
        /// </summary>
        public bool? IsStoreZero { get; set; }
    }

}
