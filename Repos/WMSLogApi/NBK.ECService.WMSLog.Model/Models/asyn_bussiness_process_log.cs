using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSLog.Model.Models
{
    public class asyn_bussiness_process_log : SysIdEntity
    {
        public asyn_bussiness_process_log()
        {
            
        }

        public Guid BussinessSysId { get; set; }

        public Guid WarehouseSysId { get; set; }
        public string BussinessOrderNumber { get; set; }

        public string BussinessTypeName { get; set; }
        public string BussinessType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Descr { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public bool IsSuccess { get; set; }
        public int SystemId { get; set; }
    }
}