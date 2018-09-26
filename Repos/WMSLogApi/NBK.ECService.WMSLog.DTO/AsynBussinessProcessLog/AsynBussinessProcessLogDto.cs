using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class AsynBussinessProcessLogDto : BaseDto
    {
        public string BussinessType { get; set; }

        public string BussinessTypeName { get; set; }

        public Guid BussinessSysId { get; set; }

        public string BussinessOrderNumber { get; set; }

        public bool IsSuccess { get; set; }

        public string RequestJson { get; set; }

        public string ResponseJson { get; set; }

        public string Descr { get; set; }
    }
}
