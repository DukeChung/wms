using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundHandleDateStatisticsDto
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
                    var str = PickDate.Value.ToString(PublicConst.DateTimeFormat);
                    if (str != "0001-01-01 00:00:00")
                    {
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
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
    }
}
