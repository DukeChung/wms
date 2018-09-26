using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartAdjustmentDetailDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductCode { get; set; }

        /// <summary>
        /// 损益级别
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 来源单号
        /// </summary>
        public string SourceOrderId { get; set; }

        public List<AdjustOrderProductPicture> AdjustOrderProductPicture { get; set; }
    }

    public class AdjustOrderProductPicture
    {
        /// <summary>
        /// 图片显示名称
        /// </summary>
        public string PictureName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string PictureUrl { get; set; }
    }

    public class ThirdPartAdjustmentDetailListDto
    {
        public Guid? SkuSysId { get; set; }

        public string Lot { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

    }
}
