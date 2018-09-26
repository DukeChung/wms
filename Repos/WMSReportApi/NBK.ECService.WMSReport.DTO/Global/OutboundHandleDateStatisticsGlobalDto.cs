using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundHandleDateStatisticsGlobalDto
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 出库单类型
        /// </summary>
        public int? OutboundType { get; set; }

        public string OutboundTypeName
        {
            get { return ((OutboundType)OutboundType).ToDescription(); }
        }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        public string CreateDateDisplay
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 拣货时间
        /// </summary>
        public DateTime? PickDate { get; set; }
        public string PickDateDisplay
        {
            get
            {
                if (PickDate.HasValue)
                {
                    return PickDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 装箱时间
        /// </summary>
        public DateTime? VanningDate { get; set; }
        public string VanningDateDisplay
        {
            get
            {
                if (VanningDate.HasValue)
                {
                    return VanningDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? ActualShipDate { get; set; }
        public string ActualShipDateDisplay
        {
            get
            {
                if (ActualShipDate.HasValue)
                {
                    return ActualShipDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        public string StatusName
        {
            get { return ((OutboundStatus)Status).ToDescription(); }
        }

        public string WarehouseName { get; set; }
    }
}
