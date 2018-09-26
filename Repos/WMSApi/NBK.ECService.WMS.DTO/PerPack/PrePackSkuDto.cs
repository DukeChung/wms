using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrePackSkuDto: BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid SysId { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// 存储位置
        /// </summary>
        public string StorageLoc { get; set; }

        /// <summary>
        /// 服务站
        /// </summary>
        public string ServiceStationName { get; set; }

        /// <summary>
        /// 入库批号
        /// </summary>
        public string BatchNumber { get; set; }

        /// <summary>
        /// 预包装单号
        /// </summary>
        public string PrePackOrder { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Nullable<int> Status { get; set; }
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
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 出库仓库
        /// </summary>
        public string OutboundSysName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateDate { get; set; }

        public string CreateDateDisplay
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString(PublicConst.DateFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }


        public List<PrePackDetailDto> PrePackSkuListDto { get; set; }
    }
}
