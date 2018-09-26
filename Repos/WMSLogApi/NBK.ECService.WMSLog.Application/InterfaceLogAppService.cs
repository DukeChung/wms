using Abp.Application.Services;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Linq;
using NBK.ECService.WMSLog.DTO;
using System.Collections.Generic;
using NBK.ECService.WMSLog.Utility;
using FortuneLab.WebApiClient;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using FortuneLab.WebApiClient.Query;

namespace NBK.ECService.WMSLog.Application
{
    public class InterfaceLogAppService : ApplicationService, IInterfaceLogAppService
    {
        private ILogCrudRepository _logCrudRepository = null;
        private IInterfaceLogRepository<Guid> _interfaceLogRepository = null;

        public InterfaceLogAppService(ILogCrudRepository logCrudRepository, IInterfaceLogRepository<Guid> interfaceLogRepository)
        {
            _logCrudRepository = logCrudRepository;
            _interfaceLogRepository = interfaceLogRepository;
        }

        public void WriteInterfaceLog(InterfaceLogDto interfaceLogDto)
        {
            interfaceLogDto.create_date = DateTime.Now;
            _logCrudRepository.Insert(interfaceLogDto.JTransformTo<interface_log>());
        }

        public LogStatisticBaseDto GetHomePageInterfaceLogStatistic(int systemId, int latestDays)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime endDate = currentDate.AddDays(1).AddMilliseconds(-1);
            DateTime startDate = currentDate.AddDays(-latestDays + 1);
            LogStatisticBaseDto rsp = _interfaceLogRepository.GetHomePageInterfaceLogStatistic(systemId, startDate, endDate);

            DateTime sparklineStartDate = currentDate.AddDays(-rsp.PastDays + 1);
            DateTime sparklineEndDate = currentDate.AddDays(1).AddMilliseconds(-1);

            IQueryable<interface_log> interfaceLogList = _logCrudRepository.GetAll<interface_log>().Where(p => p.system_id == systemId && sparklineStartDate <= p.create_date && p.create_date <= sparklineEndDate);
            for (int i = 0; i < rsp.PastDays; i++)
            {
                DateTime fromDate = currentDate.AddDays(-i);
                DateTime toDate = currentDate.AddDays(-i + 1).AddMilliseconds(-1);
                rsp.PastDaysCount[i] = interfaceLogList.Where(p => fromDate <= p.create_date && p.create_date <= toDate).Count();
            }
            rsp.PastDaysCount = rsp.PastDaysCount.Reverse().ToArray();
            return rsp;
        }

        public InterfaceLogDto GetInterfaceLogViewDto(Guid sysId)
        {
            return _logCrudRepository.Get<interface_log>(sysId).JTransformTo<InterfaceLogDto>();
        }

        public Pages<InterfaceLogListDto> GetInterfaceLogList(int systemId, InterfaceLogQuery interfaceLogQuery)
        {
            var lambda = Wheres.Lambda<interface_log>();
            lambda = lambda.And(p => p.system_id == systemId);
            if (!interfaceLogQuery.DescrSearch.IsNull())
            {
                lambda = lambda.And(p => p.interface_name.Contains(interfaceLogQuery.DescrSearch));
            }
            if (interfaceLogQuery.FlagSearch.HasValue)
            {
                lambda = lambda.And(p => p.flag == interfaceLogQuery.FlagSearch.Value);
            }
            if (interfaceLogQuery.CreateDateFromSearch.HasValue && interfaceLogQuery.CreateDateToSearch.HasValue)
            {
                interfaceLogQuery.CreateDateToSearch = interfaceLogQuery.CreateDateToSearch.Value.AddDays(1).AddMilliseconds(-1);
                lambda = lambda.And(p => interfaceLogQuery.CreateDateFromSearch.Value <= p.create_date && p.create_date <= interfaceLogQuery.CreateDateToSearch.Value);
            }
            if (!interfaceLogQuery.InterfaceTypeSearch.IsNull())
            {
                lambda = lambda.And(p => p.interface_type == interfaceLogQuery.InterfaceTypeSearch);
            }

            interfaceLogQuery.iTotalDisplayRecords = _logCrudRepository.Count(lambda);
            var logs = _logCrudRepository.GetQuery(lambda).OrderByDescending(p => p.create_date).Skip(interfaceLogQuery.iDisplayStart).Take(interfaceLogQuery.iDisplayLength).Select(p =>
                new InterfaceLogListDto
                {
                    LogType = 600,
                    SysId = p.SysId,
                    InterfaceType = p.interface_type,
                    Descr = p.interface_name,
                    IsSuccess = p.flag,
                    ElapsedTime = p.elapsed_time,
                    CreateDate = p.create_date,
                    UserId = p.user_id,
                    UserName = p.user_name
                });
            return _logCrudRepository.ConvertPages(logs, interfaceLogQuery);
        }

        /// <summary>
        /// 接口日志调用失败重新手动调用
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        public bool InvokeInterfaceLogApi(int systemId, InterfaceLogQuery request)
        {
            var info = _logCrudRepository.GetQuery<interface_log>(x => x.SysId == request.SysId).FirstOrDefault();
            if (info != null)
            {
                if (!string.IsNullOrEmpty(info.descr))  //接口路径存在
                {
                    var list = info.descr.Split('|');   //根据|截取地址：ip地址|路由路径|Post/Get请求
                    var url = PublicConst.GetWebConfigUrl(list[0]);
                    var par = JsonConvert.DeserializeObject(request.RequestJson);
                    var response = ApiClient.Post<ThirdPartyResponse>(url, list[1], new CoreQuery(), postData: par);
                    if (response.Success && response.ResponseResult != null && response.ResponseResult.IsSuccess)
                    {
                        //重调成功
                        info.flag = true;
                        info.request_json = request.RequestJson;
                        info.response_json = JsonConvert.SerializeObject(response);
                        _logCrudRepository.Update(info);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 重新插入出库单
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        public bool InsertOutbound(int systemId, InterfaceLogQuery request)
        {
            var info = _logCrudRepository.GetQuery<interface_log>(x => x.SysId == request.SysId).FirstOrDefault();
            if (info != null)
            {
                var url = PublicConst.GetWebConfigUrl("WMSAPIURL");
                var par = JsonConvert.DeserializeObject(request.RequestJson);
                var response = ApiClient.Post<ThirdPartyResponse>(url, "/ThirdParty/InsertOutbound", new CoreQuery(), postData: par);
                if (response.Success && response.ResponseResult != null && response.ResponseResult.IsSuccess)
                {
                    //重调成功
                    info.flag = true;
                    info.request_json = request.RequestJson;
                    info.response_json = JsonConvert.SerializeObject(response);
                    _logCrudRepository.Update(info);
                    return true;
                }

            }
            return false;
        }
    }
}
