using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFSkuPackDto
    {
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 基本数量
        /// </summary>
        public int FieldValue01 { get; set; }

        /// <summary>
        /// 基本单位
        /// </summary>
        public Guid? FieldUom01 { get; set; }

        /// <summary>
        /// 基本单位UPC
        /// </summary>
        public string UPC01 { get; set; }


        /// <summary>
        /// 单位转化
        /// </summary>
        public bool? InLabelUnit01 { get; set; }


        /// <summary>
        /// 外包装SYSID
        /// </summary>
        public Guid? PackSysId { get; set; }


        /// <summary>
        /// 包装名称
        /// </summary>
        public string PackCode { get; set; }


        /// <summary>
        /// 外包装数量
        /// </summary>
        public int FieldValue03 { get; set; }
        /// <summary>
        /// 外包装单位
        /// </summary>
        public Guid? FieldUom03 { get; set; }
        /// <summary>
        /// 外包UPC
        /// </summary>
        public string UPC03 { get; set; }

        public Guid WareHouseSysId { get; set; }
    }
}
