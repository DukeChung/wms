using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyWeightSkuQuery : BaseQuery
    {
        public Guid AssemblySysId { get; set; }

        public Guid SkuSysId { get; set; }
    }

    public class AssemblyWeightSkuDto
    {
        public int Index { get; set; }

        public decimal Weight { get; set; }

        public string DisplayWeight
        {
            get { return Weight.ToString("F2"); }
        }

        public DateTime CreateDate { get; set; }

        public string DisplayCreateDate
        {
            get { return CreateDate.ToString(PublicConst.DateFormat); }
        }

        public string CreateUserName { get; set; }
    }

    public class AssemblyWeightSkuRequest : BaseDto
    {
        public Guid SkuSysId { get; set; }

        public Guid AssemblySysId { get; set; }

        public decimal Weight { get; set; }
    }
}
