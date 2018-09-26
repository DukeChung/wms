using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class user_proficiency : SysIdEntity
    {
        public Nullable<int> UserId { get; set; }
        public string UserName { get; set; }
        public Nullable<int> Receipt { get; set; }
        public Nullable<int> Shelves { get; set; }
        public Nullable<int> Outbound { get; set; }
        public Nullable<int> PickDetail { get; set; }
        public Nullable<int> Vanning { get; set; }
        public Nullable<int> Deliver { get; set; }
    }
}
