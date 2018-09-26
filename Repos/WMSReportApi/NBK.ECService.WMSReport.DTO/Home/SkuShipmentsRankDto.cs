using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class SkuShipmentsRankDto
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQty { get; set; }

        public List<SkuShipmentsRankDetailDto> SkuShipmentsRankDetailDto { get; set; }
    }

    public class SkuShipmentsRankDetailDto
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int InvQty { get; set; }

        public DateTime? ZeroDate { get; set; }

        public int ZeroDateDisplay {
            get {

                if (ZeroDate.HasValue)
                {
                    return DateTime.Now.Subtract((DateTime)ZeroDate).Days;
                }
                else {
                    return 0;
                }
            }
        }

}

}
