using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyAssemblyModifyStatusDto
    {
        /// <summary>
        /// 获取或设置生产加工单的业务主键。
        /// </summary>
        public int RmpOrderId { get; set; }

        /// <summary>
        /// 获取或设置当前用户唯一标识符 id。业务主键。
        /// </summary>
        public int CurrentUserId { get; set; }

        /// <summary>
        /// 获取或设置当前用户名称。
        /// </summary>
        public string CurrentUserName { get; set; }
    }
}
