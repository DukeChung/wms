using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Extend
{
    public class GenOrderReponseDto
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 单号列表
        /// </summary>
        public List<string> OrderNumberList { get; set; }
    }
}
