using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFContainerPickingDetailListDto
    {
        /// <summary>
        /// 拣货单主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 出库单主键
        /// </summary>
        public Guid? OutboundSysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// 容器及数量
        /// </summary>
        public List<ContainerInfo> ContainerInfos { get; set; } = new List<ContainerInfo>();

        /// <summary>
        /// 分配数量
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 拣货数量
        /// </summary>
        public int PickedQty { get; set; }

        /// <summary>
        /// 本次拣货数量
        /// </summary>
        public int CurrentPickedQty { get; set; }

        public DateTime PickDate { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }

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
                            return string.Empty;
                        }
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
    }

    public class ContainerInfo
    {
        public string StorageCase { get; set; }

        public int ContainerQty { get; set; }
    }
}