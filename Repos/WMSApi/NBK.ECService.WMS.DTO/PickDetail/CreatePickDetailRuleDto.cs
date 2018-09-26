using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class CreatePickDetailRuleDto:BaseDto
    {
        public string PickDetailOrder { get; set; }

        /// <summary>
        /// 拣货规则
        /// </summary>
        public string PickRule { get; set; }

        /// <summary>
        /// 拣货方式 按单拣货  批量拣货
        /// </summary>
        public string PickType { get; set; }

        /// <summary>
        /// 拣货的PickSysIds
        /// </summary>
        public string PickSysIds { get; set; }

        public List<string> OutboundSysIdList
        {
            get
            {

                if (!string.IsNullOrEmpty(PickSysIds))
                {
                    var ids = PickSysIds.Substring(0, PickSysIds.Length - 1);

                    var list = ids.Split(',');
                    return new List<string>(list);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}