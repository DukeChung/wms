using FortuneLab.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Securities
{
    /// <summary>
    /// 权限检查配置项
    /// </summary>
    public class PermissionCheckConfig
    {
        /// <summary>
        /// 权限检查是否已开启
        /// </summary>
        public bool PermissionCheckEnable { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        PermissionCheckConfig()
        {
            bool permissionCheckEnable = false;
            if (!bool.TryParse(ConfigurationManager.AppSettings["api:PermissionCheckEnable"] ?? "false", out permissionCheckEnable))
            {
                PermissionCheckEnable = false;
            }
        }

        public static PermissionCheckConfig Instance
        {
            get
            {
                if (Singleton<PermissionCheckConfig>.Instance == null)
                {
                    Singleton<PermissionCheckConfig>.Instance = new PermissionCheckConfig();
                }
                return Singleton<PermissionCheckConfig>.Instance;
            }
        }
    }
}
