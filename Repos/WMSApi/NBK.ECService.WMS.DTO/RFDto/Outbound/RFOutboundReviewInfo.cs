using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundReviewInfo
    {
        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public int OutboundQty { get; set; }

        public int ReviewQty { get; set; }

        public decimal DisplayQty { get; set; }

        ///// <summary>
        ///// null: 无变化
        ///// true: 商品多扫描
        ///// false: 商品少扫描
        ///// </summary>
        //public bool? IsSkuAdded { get; set; }

        //public int SkuQty { get; set; }

        ///// <summary>
        ///// SkuQty和出库单明细的差异数量
        ///// </summary>
        //public int DiffQty { get; set; }

        ///// <summary>
        ///// null: 无变化
        ///// true: 数量多扫描
        ///// false: 数量少扫描
        ///// </summary>
        //public bool? IsQtyAdded
        //{
        //    get
        //    {
        //        if (DiffQty > 0)
        //        {
        //            return true;
        //        }
        //        else if (DiffQty < 0)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
    }
}
