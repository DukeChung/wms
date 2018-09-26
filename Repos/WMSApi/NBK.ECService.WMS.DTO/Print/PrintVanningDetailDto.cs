using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintVanningDetailDto
    {
        public Guid OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }

        public string VanningOrder { get; set; }

        public string ContainerNumber { get; set; }

        public string UpdateUserName { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UpdateDateDisplay
        {
            get
            {
                return UpdateDate.HasValue ? UpdateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }

        public decimal Weight { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ConsigneeName { get; set; }

        public string ConsigneeAddress { get; set; }

        public string DetailAddress { get; set; }

        public string ConsigneePhone { get; set; }

        public int TotalQty
        {
            get
            {
                if (PrintVanningDetailSkuDtoList != null && PrintVanningDetailSkuDtoList.Count > 0)
                {
                    return PrintVanningDetailSkuDtoList.Sum(p => p.Qty);
                }

                return 0;
            }
        }

        /// <summary>
        /// 商品总数
        /// </summary>
        public int TotalOrderQty { get; set; }

        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 订单折扣
        /// </summary>
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrder { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 总计金额
        /// </summary>
        public decimal TotalOrderPrice { get; set; }
        /// <summary>
        /// 优惠券价格
        /// </summary>
        public decimal CouponPrice { get; set; }

        public List<PrintVanningDetailSkuDto> PrintVanningDetailSkuDtoList { get; set; }
    }

    public class PrintVanningDetailSkuDto
    {

        public Guid SysId { get; set; }
        public string ContainerNumber { get; set; }

        public string OtherId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UOMCode { get; set; }

        public int Qty { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 是否赠品
        /// </summary>
        public bool IsGift { get; set; }
        public int GiftQty { get; set; }

        public string IsGiftName
        {
            get
            {
                if (IsGift)
                {
                    return "（赠）";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
