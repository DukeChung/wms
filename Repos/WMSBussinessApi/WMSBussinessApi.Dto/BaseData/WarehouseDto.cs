using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Utility;

namespace WMSBussinessApi.Dto.BaseData
{
    public class WarehouseDto : BaseDto
    {
        public string Name { get; set; }

        /// <summary>
        /// 加密写库
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 加密读库
        /// </summary>
        public string ConnectionStringRead { get; set; }

        /// <summary>
        /// 解密写库
        /// </summary>
        public string DecryptConnectionString {
            get {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    return StringHelper.DecryptDES(ConnectionString);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 解密读库
        /// </summary>
        public string DecryptConnectionStringRead {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionStringRead))
                {
                    return StringHelper.DecryptDES(ConnectionStringRead);
                }
                return string.Empty;
            }
        }
    }
}
