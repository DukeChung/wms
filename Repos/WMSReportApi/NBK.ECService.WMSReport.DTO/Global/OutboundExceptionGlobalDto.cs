using NBK.ECService.WMSReport.Utility;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundExceptionGlobalDto
    {

        public Guid SysId { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? ActualShipDate { get; set; }
        public string ActualShipDateDispaly
        {
            get
            {
                if (ActualShipDate.HasValue)
                {
                    return ActualShipDate.Value.ToString(PublicConst.DateFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 州/市
        /// </summary>
        public string ConsigneeCity { get; set; }
        /// <summary>
        /// 区/县
        /// </summary>
        public string ConsigneeArea { get; set; }
        /// <summary>
        /// 乡/镇
        /// </summary>
        public string ConsigneeTown { get; set; }
        /// <summary>
        /// 服务站名称
        /// </summary>
        public string ServiceStationName { get; set; }
        /// <summary>
        /// 服务站编码
        /// </summary>
        public string ServiceStationCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string UOMCode { get; set; }
        /// <summary>
        /// 异常类型
        /// </summary>
        public string ExceptionReason { get; set; }
        /// <summary>
        /// 异常数量
        /// </summary>
        public int ExceptionQty { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public string ExceptionDesc { get; set; }
        /// <summary>
        /// 跟进结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 怎人归属与处理
        /// </summary>
        public string Responsibility { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否解决
        /// </summary>
        public Nullable<bool> IsSettlement { get; set; }
        public string IsSettlementName
        {
            get
            {
                if (IsSettlement.HasValue && IsSettlement.Value)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        public string WarehouseName { get; set; }
    }
}
