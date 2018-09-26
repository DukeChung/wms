using Abp.Web.WebApi.Controllers.Filters;
using FortuneLab.ECService.Securities.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FortuneLab.ECService.Web.WebApi.Controllers
{
    [RoutePrefix("api/framework")]
    public class FrameworkController : FortuneLabApiController
    {
        [HttpGet]
        public void FrameworkAPI()
        {

        }

        /// <summary>
        /// 保持API活动
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("keepalive"), Public, AllowAnonymous]
        public string KeepAlive()
        {
            Stopwatch stopwatch = Request.Properties[DataMonitorAttribute.StopwatchKey] as Stopwatch;
            if (null != stopwatch)
                return stopwatch.ElapsedMilliseconds.ToString();
            else
                return DateTime.Now.ToString();
        }

        /// <summary>
        /// 获取组件信息列表
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [Route("assemblies/getList"), Public, AllowAnonymous]
        public List<object> GetAssemblies(string filters = "")
        {
            var filterList = string.IsNullOrWhiteSpace(filters) ? new List<string>() : filters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<object> assemblyList = new List<object>();
            StringBuilder sbAssemblyList = new StringBuilder();
            if (filterList.Any())
            {
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies().Where(asem => !asem.IsDynamic && filterList.Any(filter => asem.FullName.Contains(filter))))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(item.Location);
                    assemblyList.Add(new { item.FullName, fvi.ProductVersion, fvi.FileVersion });
                }
            }
            else
            {
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies().Where(asem => !asem.IsDynamic))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(item.Location);
                    assemblyList.Add(new { item.FullName, fvi.ProductVersion, fvi.FileVersion });
                }
            }
            return assemblyList;
        }
    }
}
