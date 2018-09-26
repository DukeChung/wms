using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Models
{
    public class Page<T>
    {
        public Page()
        { }

        public Page(List<T> records, Paging paging)
        {
            Records = records;
            Paging = paging;
        }

        public Paging Paging { get; set; }
        public List<T> Records { get; set; }
    }
}
