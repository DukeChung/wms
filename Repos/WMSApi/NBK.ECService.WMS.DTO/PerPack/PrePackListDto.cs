using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrePackListDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid SysId { get; set; }
        /// <summary>
        /// 预包装单号
        /// </summary>
        public string PrePackOrder { get; set; }
        /// <summary>
        /// 存储货位
        /// </summary>
        public string StorageLoc { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int? Status { get; set; }

        public string BatchNumber { get; set; }

        public string ServiceStationName { get; set; }


        /// <summary>
        /// 订单状态
        /// </summary>
        public string StatusName
        {
            get
            {
                if (Status.HasValue)
                {
                    return ConverStatus.GetPrePackStatus((int)Status);
                }
                return string.Empty;
            }
        }
        public DateTime? CreateDate { get; set; }
        public string CreateDateDisplay
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
    }
}
