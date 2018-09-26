using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient.Query
{
    public class PlatformQuery : CoreQuery
    {
        [Query(Name = "operationUserId")]
        public string OperationUserId { get; set; }

        /// <summary>
        /// 所有用platformQuery发起的请求，都不忽略信封
        /// </summary>
        public override bool IgnoreEnvelope { get { return false; } }
    }
}
