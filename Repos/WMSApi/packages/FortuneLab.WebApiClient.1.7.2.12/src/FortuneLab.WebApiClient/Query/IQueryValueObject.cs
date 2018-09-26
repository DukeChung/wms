using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient.Query
{
    public interface IQueryValueObject
    {
        /// <summary>
        /// 获取查询对象的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        object GetValue();
    }
}
