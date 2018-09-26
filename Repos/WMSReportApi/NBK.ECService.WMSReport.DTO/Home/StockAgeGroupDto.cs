using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 仓库出库，库存库龄占比
    /// </summary>
    public class StockAgeGroupDto
    {
        /// <summary>
        /// 0-5天库龄
        /// </summary>
        public int FirstOrder { get; set; }
        /// <summary>
        /// 6-15天库龄
        /// </summary>
        public int SecondOrder { get; set; }
        /// <summary>
        /// 16-30天库龄
        /// </summary>
        public int ThreeOrder { get; set; }
        /// <summary>
        /// 31-50天库龄
        /// </summary>
        public int FourOrder { get; set; }
        /// <summary>
        /// 50天以上库龄
        /// </summary>
        public int FiveOrder { get; set; }
    }
}
