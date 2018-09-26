﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum FrozenStatus
    {
        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Frozen = 10,

        /// <summary>
        /// 解冻
        /// </summary>
        [Description("解冻")]
        UnFrozen = 20
    }
}
