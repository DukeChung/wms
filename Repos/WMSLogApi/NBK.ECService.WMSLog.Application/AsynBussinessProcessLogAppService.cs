using Abp.Application.Services;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application
{
    public class AsynBussinessProcessLogAppService : ApplicationService,IAsynBussinessProcessLogAppService
    {
        private ILogCrudRepository _logCrudRepository = null;

        public AsynBussinessProcessLogAppService(ILogCrudRepository logCrudRepository)
        {
            _logCrudRepository = logCrudRepository;
        }

        public void WriteLog(AsynBussinessProcessLogDto request)
        {
            asyn_bussiness_process_log log = request.JTransformTo<asyn_bussiness_process_log>();
            log.SystemId = request.system_id;
            log.UserId = request.CurrentUserId.ToString();
            log.UserName = request.CurrentDisplayName;
            log.CreateDate = DateTime.Now;
            
            _logCrudRepository.Insert(log);
        }
    }
}
