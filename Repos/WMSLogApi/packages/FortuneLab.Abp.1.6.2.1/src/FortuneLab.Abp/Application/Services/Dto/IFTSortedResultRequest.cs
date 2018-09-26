using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// FortuneLab排序接口
    /// </summary>
    public interface IFTSortedResultRequest
    {
        /// <summary>
        /// 排序方向
        /// </summary>
        string SortDirection { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        string SortField { get; set; }
    }
}
