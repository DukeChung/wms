using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AdjustmentListDto
    {
        public Guid SysId { get; set; }

        public string AdjustmentOrder { get; set; }

        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return ((AdjustmentStatus)Status).ToDescription();
            }
        }

        public int Type { get; set; }

        public string TypeName
        {
            get
            {
                return ((AdjustmentType)Type).ToDescription();
            }
        }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        /// <summary>
        /// 来源单号
        /// </summary>
        public string SourceOrder { get; set; }
        public string CreateInfoDisplay
        {
            get
            {
                return $"{CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - {CreateUserName}";
            }
        }
    }
}
