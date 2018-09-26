using FortuneLab.WebApiClient.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Models
{
    public class ListQueryModel
    {
        public string Keywords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public string GroupProperty { get; set; }

        [Obsolete("Use SessionQuery instead")]
        public ListQuery ToListQuery(LoginQuery loginQuery)
        {
            return new ListQuery(loginQuery, PageIndex, PageSize, Keywords) { SortField = SortField, SortDirection = SortDirection };
        }

        public ListQuery ToListQuery(SessionQuery sessionQuery)
        {
            return new ListQuery(sessionQuery, PageIndex, PageSize, Keywords) { SortField = SortField, SortDirection = SortDirection };
        }
    }
}
