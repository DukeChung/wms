using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PickDetailListDto : PickDetailDto
    {
        //public string StatusText
        //{
        //    get
        //    {
        //        if (Status.HasValue)
        //        {
        //            return ConverStatus.PickDetail(Status.Value);
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        public string StatusText
        {
            get
            {
                if (Status.HasValue)
                {
                    if (Status.Value == (int)PickDetailStatus.New)
                    {
                        if (PickedQty == 0)
                        {
                            return PickDetailStatus.New.ToDescription();
                        }
                        else if (Qty == PickedQty)
                        {
                            return PickDetailStatus.Finish.ToDescription();
                        }
                        else
                        {
                            return PickDetailStatus.PartPick.ToDescription();
                        }
                    }
                    else
                    {
                        return ((PickDetailStatus)Status.Value).ToDescription();
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string UomCode { get; set; }

        public string PickDateText
        {
            get
            {
                if (PickDate.HasValue && PickDate.Value != new DateTime())
                {
                    return PickDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int OutboundCount { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 出库单业务类型
        /// </summary>
        public string OutboundChildType { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrder { get; set; }

        /// <summary>
        /// 服务站名称
        /// </summary>
        public string ServiceStationName { get; set; }
        /// <summary>
        /// 拣货人
        /// </summary>
        public string AppointUserNames { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
