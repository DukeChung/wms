using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    /// <summary>
    /// 获取预包装库存参数
    /// </summary>
    public class PrePackSkuQuery:BaseQuery
    {
        public string SkuCodeSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public string UPCSearch { get; set; }
    }
}
