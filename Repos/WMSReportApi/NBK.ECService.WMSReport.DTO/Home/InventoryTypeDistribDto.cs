using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{

    public class InventoryTypeDistribDto
    {
        public List<string> SkuClassName { get; set; }
        public List<InventoryTypeDistribListDto> InventoryTypeDistribListDto { get; set; }
    }

    public class InventoryTypeDistribListDto
    {
        public string TypeName { get; set; }
        public List<InventoryTypeDistrib> InventoryTypeDistrib { get; set; }
    }

    public class InventoryTypeDistrib
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string SkuClassName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
    }
}
