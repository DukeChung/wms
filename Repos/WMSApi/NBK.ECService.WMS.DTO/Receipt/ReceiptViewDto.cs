﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptViewDto : ReceiptDto
    {
        public string ExpectedReceiptDateText { get { return ExpectedReceiptDate.HasValue ? ExpectedReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public string ReceipDateText { get { return ReceipDate.HasValue ? ReceipDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public string ArrivalDateText { get { return ArrivalDate.HasValue ? ArrivalDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public string ReceiptTypeText { get { return ((Utility.Enum.ReceiptType)ReceiptType).ToDescription(); } }

        public string StatusText { get { return Status.HasValue ? ((Utility.Enum.ReceiptStatus)Status.Value).ToDescription() : string.Empty; } }

        /// <summary>
        /// 作业人
        /// </summary>
        public string AppointUserNames { get; set; }

        public List<ReceiptDetailViewDto> ReceiptDetailViewDtoList { get; set; }

        public List<ReceiptPurchaseDto> RelatedReceiptPurchaseDtoList { get; set; }
    }
}
