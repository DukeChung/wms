using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// FortuneLab 分页请求接口
    /// </summary>
    public interface IFTPagedResultRequest
    {
        /// <summary>
        /// Skip count (beginning of the page).
        /// </summary>
        int SkipCount { get; set; }
        
        /// <summary>
        /// 页码
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 每页数据量
        /// </summary>
        int PageSize { get; set; }

    }
}
