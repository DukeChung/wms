using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    /// <summary>
    /// 打印箱贴Dto
    /// </summary>
    public class PrintVanningDetailStickDto
    {
        /// <summary>
        /// 装箱单主键
        /// </summary>
        public Guid? VanningSysId { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ConsigneePhone { get; set; } 

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 出库单外部单号
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 箱号+容器编号
        /// </summary>
        public string VanningOrderNumber { get; set; }

        /// <summary>
        /// 容器序号
        /// </summary>
        public string ContainerNumber { get; set; }

        /// <summary>
        /// 包裹重量
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 包裹数量
        /// </summary>
        public string ParcelNumber { get; set; }

        /// <summary>
        /// 发货人
        /// </summary>
        public string Contacts { get; set; }

        /// <summary>
        /// 发货人电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 发货人地址
        /// </summary>
        public string Address { get; set; }

        public string PrintMallName { get; set; }

        public string PrintMallHttpUrl { get; set; }

        public string PrintMallPhone { get; set; }

        public int SkuCount { get; set; }

        public int SkuQty { get; set; }

        public List<PrintVanningDetailStickSkuDto> PrintVanningDetailStickSkuDto { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string CarrierNumber { get; set; }

        /// <summary>
        /// 大头笔
        /// </summary>
        public string Marke { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
    }
}
