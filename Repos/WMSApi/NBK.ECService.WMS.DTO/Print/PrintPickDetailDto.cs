using System;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickDetailDto : BaseDto
    {
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Lpn { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UomName { get; set; }

        public string UomNameDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(SkuDescr))
                {
                    var index = SkuDescr.LastIndexOf("单位");
                    if (index != -1)
                    {
                        var name = SkuDescr.Substring(index, SkuDescr.Length - index);
                        if (!string.IsNullOrEmpty(name))
                        {
                            return name;
                        }
                        else
                        {
                            return "缺省";
                        }
                    }
                    else
                    {
                        return "缺省";
                    }
                }
                else
                {
                    return "缺省";
                }
            }

        }

        /// <summary>
        /// 出库单Id
        /// </summary>
        public Guid? OutboundSysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        public string PackFactor { get; set; }

        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 出库单业务类型
        /// </summary>
        public string OutboundChildType { get; set; }
    }
}