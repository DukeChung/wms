using System;

namespace NBK.ECService.WMSReport.DTO.Package
{
    public class UOMDto
    {

        public Guid SysId { get; set; }
        public string UOMCode { get; set; }
        public string Descr { get; set; }
        public string UomType { get; set; }

        public string UomTypeName { get; set; }
    }
}