using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Repositories
{
    /// <summary>
    /// 为了方便扩展，不同的Repository框架实现都需要知道ConnectionString
    /// </summary>
    public interface IDbConnProvider
    {
        /// <summary>
        /// 返回数据库ConnectionString 配置项Name或者DbConnectionString
        /// </summary>
        string NameOrConnectionString { get; }
    }
}
