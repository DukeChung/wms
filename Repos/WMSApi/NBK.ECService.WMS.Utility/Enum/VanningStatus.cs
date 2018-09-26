﻿using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum VanningStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 装箱中
        /// </summary>
        [Description("装箱中")]
        Vanning = 20,

        /// <summary>
        /// 装箱完成
        /// </summary>
        [Description("装箱完成")]
        Finish = 50,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = -999
    }
}