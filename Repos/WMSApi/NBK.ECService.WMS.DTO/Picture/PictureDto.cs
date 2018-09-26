using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PictureDto : BaseDto
    {
        public Guid SysId { get; set; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 图片后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 图片名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Url { get; set; }

        public string ShowUrl { get; set; }
    }
}
