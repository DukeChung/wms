using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class SysCodeQuery:BaseQuery
    {
        public string SysCodeTypeSearch { get; set; }

        public string DescrSearch { get; set; }
    }
}