﻿using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace NBK.ECService.WMSLog.DTO
{
    public class ConnectionStringDto
    {
        public Guid SysId { get; set; }

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public string ConnectionStringRead { get; set; }
    }
}
