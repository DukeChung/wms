using System;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class prepackrelation : SysIdEntity
    {
        public Guid PrePackDetailSysId { get; set; }
        public Guid PrePackSysId { get; set; }
        public Guid PreBulkPackSysId { get; set; }
        public Guid PreBulkPackDetailSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
    }
}