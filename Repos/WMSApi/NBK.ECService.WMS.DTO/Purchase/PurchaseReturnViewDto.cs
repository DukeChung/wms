using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseReturnViewDto : PurchaseViewDto
    {
        /// <summary>
        /// 顾客名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 退货联系人
        /// </summary>
        public string ReturnContact { get; set; }

        /// <summary>
        /// 申请人发货地址
        /// </summary>
        public string ShippingAddress { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNumber { get; set; }

        /// <summary>
        /// 退货时间
        /// </summary>
        public DateTime? ReturnTime { get; set; }

        /// <summary>
        /// 退货时间（用于显示）
        /// </summary>
        public string ReturnTimeText
        {
            get
            {
                if (ReturnTime != null)
                {
                    return ReturnTime.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return PurchaseDateText;
                }
            }
        }

        /// <summary>
        /// 退货原因
        /// </summary>
        public string ReturnReason { get; set; }

        /// <summary>
        /// 退货凭证图片URL
        /// </summary>
        public List<string> ReturnVoucherURL { get; set; }

        /// <summary>
        /// 品控检验时间
        /// </summary>
        public DateTime? QCTime { get; set; }

        /// <summary>
        /// 品控检验时间（用于显示）
        /// </summary>
        public string QCTimeText
        {
            get
            {
                if (QCTime != null)
                {
                    return QCTime.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrderId { get; set; }

        public Guid? FromWareHouseSysId { get; set; }

        public List<PurchaseDetailReturnViewDto> PurchaseDetailReturnViewDto { get; set; }
    }
}
