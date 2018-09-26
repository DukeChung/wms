using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AdjustmentDto:BaseDto
    {
        public Guid? SysId { get; set; }

        public string AdjustmentOrder { get; set; }

        public int Status { get; set; }

        public int Type { get; set; }

        public string SourceType { get; set; }
        public string SourceOrder { get; set; }

        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }

        public List<AdjustmentDetailDto> AdjustmentDetailList { get; set; } 
    }

    public class AdjustmentDetailDto
    {
        public Guid SysId { get; set; }
        public Guid AdjustmentSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string Loc { get; set; } = "";

        public string Lot { get; set; } = "";

        public string Lpn { get; set; } = "";

        public string UPC { get; set; }

        public string UOMCode { get; set; }

        public decimal Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string AdjustlevelCode { get; set; }

        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Remark { get; set; }
        public List<PictureDto> PictureDtoList { get; set; }
    }
}
