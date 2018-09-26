using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient.Query
{
    [Obsolete("请使用CoreQuery替换")]
    public class EmptyQuery : CoreQuery
    {
        public static EmptyQuery Instance { get { return new EmptyQuery(); } }
    }
}
