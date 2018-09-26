using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient.Query
{
    public class DfsQuery : SessionQuery
    {
        [Query(Name = "bucket")]
        public string Bucket { get; set; }

        [Query(Name = "path")]
        public string Path { get; set; }
    }
}
