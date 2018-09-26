using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ThirdPartyLoadingSequenceDto
    {
        /// <summary>
        ///  TMS运单号
        /// </summary>
        public string TMSOrder { get; set; }

        /// <summary>
        /// 出库单列表
        /// </summary>
        public List<LoadingSequenceOutboundList> LoadingSequenceOutboundList { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 装车时间
        /// </summary>
        public DateTime? DepartureDate { get; set; }
        public string OtherId { get; set; }
    }

    public class LoadingSequenceOutboundList
    {
        /// <summary>
        /// 外部订单号
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        /// 仓库外部Id
        /// </summary>
        public string OtherId { get; set; }

        /// <summary>
        /// 类型：1：仓库发货，0：服务站发货
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 订单类型，可参考TMSOrderType中的枚举
        /// </summary>
        public int OrderType { get; set; }

    }
}
