using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenListDto : BaseDto
    {
        public Guid SysId { get; set; }

        public string ZoneName { get; set; }

        public string Loc { get; set; }

        public int Type { get; set; }

        public string TypeDisplay
        {
            get
            {
                return ((FrozenType)Type).ToDescription();
            }
        }

        public int Status { get; set; }

        public string StatusDisplay
        {
            get
            {
                return ((FrozenStatus)Status).ToDescription();
            }
        }

        public string CreateUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.ToString(PublicConst.DateTimeFormat);
            }
        }

        public string Memo { get; set; }

        public Guid? SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int FrozenSource { get; set; }

        public string FrozenSourceDisplay
        {
            get
            {
                return ((FrozenSource)FrozenSource).ToDescription();
            }
        }
    }
}
