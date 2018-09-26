using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WMSBussinessApi.Service;
using WMSBussinessApi.Dto.DataSync;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WMSBussinessApi.Website.Controllers
{
    [Route("api/[controller]")]
    public class DataSyncController : Controller
    {
        private IDataSyncSvc _dataSyncSvc;
        public DataSyncController(IDataSyncSvc dataSyncSvc)
        {
            _dataSyncSvc = dataSyncSvc;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Test")]
        public string Test()
        {
            return _dataSyncSvc.Test();
        }

        /// <summary>
        /// 多仓同步创建商品
        /// </summary>
        /// <param name="sku"></param>
        [HttpPost("SyncCreateSku")]
        public void SyncCreateSku([FromBody]SkuDto sku)
        {
            _dataSyncSvc.SyncCreateSku(sku);
        }

        /// <summary>
        /// 多仓同步更新商品
        /// </summary>
        /// <param name="sku"></param>
        [HttpPost("SyncUpdateSku")]
        public void SyncUpdateSku([FromBody]SkuDto sku)
        {
            _dataSyncSvc.SyncUpdateSku(sku);
        }

        /// <summary>
        /// 多仓同步创建包装
        /// </summary>
        /// <param name="pack"></param>
        [HttpPost("SyncCreatePack")]
        public void SyncCreatePack([FromBody]PackDto pack)
        {
            _dataSyncSvc.SyncCreatePack(pack);
        }

        /// <summary>
        /// 多仓同步更新包装
        /// </summary>
        /// <param name="pack"></param>
        [HttpPost("SyncUpdatePack")]
        public void SyncUpdatePack([FromBody]PackDto pack)
        {
            _dataSyncSvc.SyncUpdatePack(pack);
        }

        /// <summary>
        /// 多仓同步删除包装
        /// </summary>
        /// <param name="sysIdList"></param>
        [HttpPost("SyncDeletePack")]
        public void SyncDeletePack([FromBody]List<Guid> sysIdList)
        {
            _dataSyncSvc.SyncDeletePack(sysIdList);
        }

        /// <summary>
        /// 多仓同步创建系统代码
        /// </summary>
        /// <param name="syscode"></param>
        [HttpPost("SyncCreateSyscode")]
        public void SyncCreateSyscode([FromBody]SyscodeDto syscode)
        {
            _dataSyncSvc.SyncCreateSyscode(syscode);
        }

        /// <summary>
        /// 多仓同步创建系统代码明细
        /// </summary>
        /// <param name="syscodedetail"></param>
        [HttpPost("SyncCreateSyscodeDetail")]
        public void SyncCreateSyscodeDetail([FromBody]SyscodeDetailDto syscodedetail)
        {
            _dataSyncSvc.SyncCreateSyscodeDetail(syscodedetail);
        }
    }
}
