using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class VanningOperationDto : BaseDto
    {
        /// <summary>
        /// 出库或拣货单号
        /// </summary> 
        public string PickDetailOrder { get; set; }

        /// <summary>
        /// 扫描条码
        /// </summary>
        public string ScanBarCode { get; set; }

        /// <summary>
        /// 单位数量
        /// </summary>
        public int UnitQty { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 总订单数量
        /// </summary>
        public int TotalOrderCount { get; set; }

        /// <summary>
        /// 总Sku数量
        /// </summary>
        public int? TotalSkuCount { get; set; }

        /// <summary>
        /// 扫描Sku数量
        /// </summary>
        public int ScanTotalSkuCount { get; set; }

        /// <summary>
        /// 扫描订单数量
        /// </summary>
        public int ScanTotalOrderCount { get; set; }

        public string ActionType { get; set; }

        public string CurrentUserName { get; set; }

        public int CurrentUserId { get; set; }

        /// <summary>
        /// 出库单业务类型
        /// </summary>
        public string OutboundChildType { get; set; }

        /// <summary>
        /// 拣货明细
        /// </summary>
        public List<PickDetailOperationDto> PickDetailOperationDto { get; set; }

        /// <summary>
        /// 判断操作用
        /// </summary>
        public List<PickDetailSumDto> PickDetailSumOperationDto { get; set; }

        /// <summary>
        /// 展示用
        /// </summary>
        public List<PickDetailSumDto> PickDetailSumDto { get; set; }

        /// <summary>
        /// 展示用
        /// </summary>
        public List<VanningPickDetailOperationDto> VanningPickDetailOperationDto { get; set; }

        /// <summary>
        /// 装箱明细
        /// </summary>
        public List<VanningPickDetailDto> VanningPickDetailDto { get; set;}
        /// <summary>
        /// 装箱 箱子明细  箱子和 装箱明细通过OutBoundSysId+箱号关联
        /// </summary>
        public List<VanningDetailOperationDto> VanningDetailOperationDto { get; set; }

        /// <summary>
        /// 历史装箱 单号 
        /// </summary>
        public List<VanningRecordDto> VanningRecordDto { get; set; }
    }
}