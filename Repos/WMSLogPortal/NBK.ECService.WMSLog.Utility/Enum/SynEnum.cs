using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Utility.Enum
{
    public enum SynEnum
    {
        /// <summary>
        /// 同步SKU包含分类
        /// </summary>
        [Description("同步SKU包含分类全部")]
        SynchroAll = 1,

        /// <summary>
        /// 同步菜单
        /// </summary>
        [Description("同步菜单")]
        SynchroMenu = 2,

        /// <summary>
        /// 同步登陆信息
        /// </summary>
        [Description("同步登陆信息")]
        CleanUserLoginRedis = 3,

        /// <summary>
        /// 同步SKU包含分类
        /// </summary>
        [Description("同步SKU包含分类")]
        SynchroSku = 4,

        /// <summary>
        /// 同步包装包含单位
        /// </summary>
        [Description("同步包装包含单位")]
        SynchroPack = 5,

        /// <summary>
        /// 同步批次模板
        /// </summary>
        [Description("同步批次模板")]
        SynchroLottemplate = 6,

        /// <summary>
        /// 同步仓库
        /// </summary>
        [Description("同步仓库")]
        SynchroWarehouse = 7,

    }
}
