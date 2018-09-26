using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFPrePackDto : BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 预包装单号
        /// </summary>
        public string PrePackOrder { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string StorageLoc { get; set; }

        public Guid? SkuSysId { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
    }
}
