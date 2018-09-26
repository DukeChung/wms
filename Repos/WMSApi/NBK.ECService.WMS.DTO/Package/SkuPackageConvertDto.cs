using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuPackageConvertDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string PackCode { get; set; }
        public Nullable<bool> InLabelUnit01 { get; set; }
        public Nullable<int> FieldValue01 { get; set; }
        public Nullable<int> FieldValue02 { get; set; }

        /// <summary>
        /// 基础Qty（原材料最小单位）
        /// </summary>
        public int BaseQty { get; set; }

        /// <summary>
        /// 单位Qty（成品单位）
        /// </summary>
        public decimal UnitQty { get; set; }

        /// <summary>
        /// 原材料-->成品：false
        /// 成品-->原材料：true
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 是否做转化：true转化，false不转化
        /// </summary>
        public bool result { get; set; }

    }
}
