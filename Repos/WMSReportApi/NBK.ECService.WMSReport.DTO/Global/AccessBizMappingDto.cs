using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class AccessBizMappingDto
    {
        public Guid SysId { get; set; }

        public string app_controller { get; set; }

        public string app_service { get; set; }

        public string BizName { get; set; }

        public string SecondBizName { get; set; }

        public string FirstBizName { get; set; }

        public int Count { get; set; }

        public string ServiceUrl
        {
            get
            {
                return app_controller + app_service;
            }
        }
    }

    public class AccessBizGlobalQuery : BaseQuery
    {

    }

    public class AccessBizGlobalDto
    {
        public List<AccessBizGlobalNodesDto> nodes { get; set; } = new List<AccessBizGlobalNodesDto>();

        public List<AccessBizGlobalLinksDto> links { get; set; } = new List<AccessBizGlobalLinksDto>();
    }

    public class AccessBizGlobalNodesDto
    {
        public string name { get; set; }
    }

    public class AccessBizGlobalLinksDto
    {
        public string source { get; set; }

        public string target { get; set; }

        public int value { get; set; }
    }
}
