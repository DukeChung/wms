using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public  class PrePackDetailDto
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? InvSysId { get; set; }
        public string OtherId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }
        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }
        
        /// <summary>
        /// 最大数量
        /// </summary>
        public int? MaxQty { get; set; }
        public int? SumQty { get; set; }


        public int? PreQty { get; set; }
        /// <summary>
        /// 预包装数量
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 差异数量
        /// </summary>
        public int DifferenceQty
        {
            get
            {
                if (PreQty.HasValue && Qty.HasValue)
                {
                    return PreQty.Value - Qty.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string Lot { get; set; }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        public string ProduceDateDisplay { get; set; }
        public string ExpiryDateDisplay { get; set; }

        public DateTime? ProduceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string UomCode { get; set; }
        public string PackCode { get; set; }

    }
}
