using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum PickDetailSourceType
    {
        /// <summary>
        /// 出库单拣货明细
        /// </summary>
        [Description("出库单拣货明细")]
        Outbound = 10,

        /// <summary>
        /// 加工单拣货明细
        /// </summary>
        [Description("加工单拣货明细")]
        Assembly = 20,
    }
}
