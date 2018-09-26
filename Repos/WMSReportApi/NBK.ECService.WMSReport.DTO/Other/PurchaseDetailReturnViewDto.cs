using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
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
                    return string.Empty;
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
                if (AbnormalQty != null && Qty != null)
                {
                    return (Convert.ToInt32(Qty) - Convert.ToInt32(AbnormalQty)).ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
