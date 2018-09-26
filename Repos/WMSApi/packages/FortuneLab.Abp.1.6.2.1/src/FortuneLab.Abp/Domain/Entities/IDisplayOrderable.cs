using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// 此接口定义可手动排序的属性DisplayOrder
    /// </summary>
    public interface IDisplayOrderable
    {
        int DisplayOrder { get; set; }
    }
}
