using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class ReceiptListDto
    {
        public Guid SysId { get; set; }

        public string ReceiptOrder { get; set; }

        //public string DisplayExternalOrder { get; set; }

        public string VendorName { get; set; }

        public int ReceiptType { get; set; }

        public string ReceiptTypeText { get { return ((Utility.Enum.ReceiptType)ReceiptType).ToDescription(); } }

        public int? Status { get; set; }

        public string StatusText { get { return Status.HasValue ? ((Utility.Enum.ReceiptStatus)Status.Value).ToDescription() : string.Empty; } }

        public DateTime? ExpectedReceiptDate { get; set; }

        public string ExpectedReceiptDateText { get { return ExpectedReceiptDate.HasValue ? ExpectedReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? ReceiptDate { get; set; }

        public string ReceiptDateText { get { return ReceiptDate.HasValue ? ReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public string ExternalOrder { get; set; }

        /// <summary>
        /// 作业人
        /// </summary>
        public string AppointUserNames { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 收货数量
        /// </summary>
        public int? TotalReceivedQty { get; set; }

        /// <summary>
        /// 上架数量
        /// </summary>
        public int? TotalShelvesQty { get; set; }

        /// <summary>
        /// 上架状态
        /// </summary>
        public string ShelvesStatusText
        {
            get
            {
                //未上架
                if (TotalShelvesQty == 0 || !TotalShelvesQty.HasValue || !TotalReceivedQty.HasValue)
                {
                    return ShelvesStatus.NotOnShelves.ToDescription();
                }

                //上架中
                if (TotalReceivedQty > TotalShelvesQty)
                {
                    return ShelvesStatus.Shelves.ToDescription();
                }

                //上架完成   
                if (TotalReceivedQty == TotalShelvesQty)
                {
                    return ShelvesStatus.Finish.ToDescription();
                }
                return string.Empty;
            }
        }
    }
}
