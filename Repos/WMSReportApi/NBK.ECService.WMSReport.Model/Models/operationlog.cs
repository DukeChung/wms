using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class operationlog : SysIdEntity
    { 
        public int Type { get; set; }
        public string ApiController { get; set; }
        public string AppService { get; set; }
        public string Descr { get; set; }
        public string JsonValue { get; set; }
        public string UserName { get; set; }
        public int CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
    }
}
