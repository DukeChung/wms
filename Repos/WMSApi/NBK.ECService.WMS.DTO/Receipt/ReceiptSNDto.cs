using System;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptSNDto
    {
        public Guid ReceiptSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SN { get; set; }

        /// <summary>
        /// 页面判断是否已经使用
        /// </summary>
        public string IsAction { get; set; }
        /// <summary>
        /// 页面判断是否已经打印
        /// </summary>
        public string IsPrint { get; set; }

    }
}