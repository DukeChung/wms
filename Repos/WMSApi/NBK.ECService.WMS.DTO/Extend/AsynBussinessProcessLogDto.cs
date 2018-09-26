using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AsynBussinessProcessLogDto<T> : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int SystemId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BussinessType { get; set; }

        /// <summary>
        /// 业务类型描述
        /// </summary>
        public string BussinessTypeName { get; set; }

        /// <summary>
        /// 业务类型Id
        /// </summary>
        public Guid BussinessSysId { get; set; }

        /// <summary>
        /// 业务类型单号ID
        /// </summary>
        public string BussinessOrderNumber { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// 业务Dto
        /// </summary>
        public T BussinessDto { get; set; }
    }
}
