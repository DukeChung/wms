using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Securities.Entities
{
    /// <summary>
    /// 用户登录信息实体
    /// </summary>
    public class UserDevice : SysIdEntity
    {
        public int UserSysId { get; set; }
        public int DeviceType { get; set; }
        public string SessionKey { get; set; }

        /// <summary>
        /// 最后一次激活时间(ValidateSession更新时间)
        /// </summary>
        public DateTime ActiveTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiredTime { get; set; }

        public string DeviceId { get; set; }

        public string ClientId { get; set; }

        public string UserAgent { get; set; }
    }
}
