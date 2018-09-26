﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Securities
{
    public interface IFortuneLabUser<TKey>
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        TKey UserId { get; }

        /// <summary>
        /// 登录名称
        /// </summary>
        string LoginName { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; }
    }
}
