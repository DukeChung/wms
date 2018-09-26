using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseDetailReturnViewDto : PurchaseDetailViewDto
    {
        /// <summary>
        /// 异常数量
        /// </summary>
        public int? AbnormalQty { get; set; }

        /// <summary>
        /// 异常数量(用于显示)
        /// </summary>
        public string AbnormalQtyText
        {
            get
            {
                if (AbnormalQty != null)
                {
                    return AbnormalQty.ToString();
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// 可二次销售数量(用于显示)
        /// </summary>
        public string CanSaleQtyText
        {
            get
            { 
                if (Qty!=null)
                { 
                    if (AbnormalQty != null)
                    {
                        return (Convert.ToInt32(Qty) - Convert.ToInt32(AbnormalQty)).ToString();
                    }
                    return Qty.ToString();
                }
                else
                {
                    return "0";
                }
            }
        }
    }
}
