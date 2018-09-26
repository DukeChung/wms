using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenRequestDto: BaseDto
    {
        public int Type { get; set; }

        public Guid ZoneSysId { get; set; }

        public string Loc { get; set; }

        public int Status { get; set; }

        public string Memo { get; set; }

        public Guid? SysId { get; set; }

        public string skus { get; set; }

        public List<Guid> SkuList { get; set; }

        /// <summary>
        /// 货位商品 类型 冻结专用集合
        /// </summary>
        public List<FrozenLocSkuDto> LocSkuList { get; set; }
    }
}
