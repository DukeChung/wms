using System;

namespace NBK.ECService.WMS.DTO
{
    public class MQProcessDto<T> : BaseDto
    {
        public Guid BussinessSysId { get; set; }

        public string BussinessOrderNumber { get; set; }

        public string Descr { get; set; }

        public T BussinessDto { get; set; }

    }
}
