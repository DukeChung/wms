using System.Collections.Generic;

namespace FortuneLab.WebClient.Models
{
    public class ListResultInfo<T>
        where T : class, new()
    {
        public ListResultInfo()
        {
            Paging = new PagingInfo();
            Records = new List<T>();
        }

        public PagingInfo Paging { get; set; }
        public List<T> Records { get; set; }
    }
}