using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickDetailByOrderDto
    {
        public Guid SysId { get; set; }
        public string PickDetailOrder { get; set; }
        public string OutboundOrder { get; set; }
        public DateTime? ExternOrderDate { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneePhone { get; set; }

        public string ServiceStationName { get; set; }

        public int SkuCount { get; set; }

        public int SkuQty { get; set; }

        /// <summary>
        /// 打印人
        /// </summary>
        public string PrintBy { get; set; }
        public List<PrintPickDetailDto> PrintPickDetailDtos { get; set; }

        public string ExternOrderDateText
        {
            get
            {
                if (ExternOrderDate.HasValue)
                {
                    return ExternOrderDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 出库单业务类型
        /// </summary>
        public string OutboundChildType { get; set; }
    }
}