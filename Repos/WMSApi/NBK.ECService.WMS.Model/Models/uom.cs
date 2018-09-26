using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class uom : SysIdEntity
    {
        public string UOMCode { get; set; }
        public string Descr { get; set; }
        public string UomType { get; set; }
    }
}
