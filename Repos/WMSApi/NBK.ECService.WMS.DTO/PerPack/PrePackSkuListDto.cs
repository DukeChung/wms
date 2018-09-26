using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    /// <summary>
    /// 预包装库存列表
    /// </summary>
    public class PrePackSkuListDto
    {
        public Guid SysId { get; set; }

        /// <summary>
        /// 仓库代码
        /// </summary>
        public Guid WareHouseSysId { get; set; }

        /// <summary>
        /// 货品编码
        /// </summary>
        public Guid SkuSysId { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int Qty { get; set; }
     
        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// 货品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 货品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? ReceiptDate { get; set; }
        public string ReceiptDateDisplay
        {
            get
            {
                if (ReceiptDate.HasValue)
                {
                    return ReceiptDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ProduceDate { get; set; }
        public string ProduceDateDisplay
        {
            get
            {
                if (ProduceDate.HasValue)
                {
                    return ProduceDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateDisplay
        {
            get
            {
                if (ExpiryDate.HasValue)
                {
                    return ExpiryDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
    }
}
