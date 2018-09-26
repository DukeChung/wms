using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AdjustmentViewDto
    {
        public Guid SysId { get; set; }

        public string AdjustmentOrder { get; set; }

        public Guid WareHouseSysId { get; set; }

        public string WareHouseName { get; set; }


        /// <summary>
        /// 来源单号
        /// </summary>
        public string SourceOrder { get; set; }
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

        public string CreateInfoDisplay
        {
            get
            {
                return $"{CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - {CreateUserName}";
            }
        }

        public List<AdjustmentDetailViewDto> AdjustmentDetailList { get; set; } = new List<AdjustmentDetailViewDto>();
    }

    public class AdjustmentDetailViewDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string UOMCode { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string AdjustlevelCode { get; set; }

        public string AdjustlevelDisplay { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public string Channel { get; set; }
        public string Remark { get; set; }
        public List<PictureDto> PictureDtoList { get; set; }
    }
}
