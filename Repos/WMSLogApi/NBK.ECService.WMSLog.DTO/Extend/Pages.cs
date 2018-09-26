using FortuneLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class Pages<T> : Page<T>
    {
        public TableResults<T> TableResuls { get; set; }
    }

    public class TableResults<T>
    {
        /// <summary>
        /// DataTable请求服务器端次数
        /// </summary> 
        public int sEcho { get; set; }
        /// <summary>
        /// 返回条数
        /// </summary>
        public int iTotalRecords { get; set; }
        /// <summary>
        /// 当前显示多少条
        /// </summary>
        public int iTotalDisplayRecords { get; set; }
        /// <summary>
        /// 返回结果集
        /// </summary>
        public List<T> aaData { get; set; }
    }
}
