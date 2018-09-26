using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyTransferInventoryOutBackDto
    {
        /// <summary>
        /// 移仓单ID。
        /// </summary>
        public int ShiftOrderID { get; set; }

        /// <summary>
        /// 更新用户ID
        /// </summary>
        public long UpdateUserID { get; set; }

        /// <summary>
        /// 更新用户名
        /// </summary>
        public string UpdateUserName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
