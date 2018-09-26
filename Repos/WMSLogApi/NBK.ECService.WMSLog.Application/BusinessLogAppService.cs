using Abp.Application.Services;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;

namespace NBK.ECService.WMSLog.Application
{
    public class BusinessLogAppService : ApplicationService, IBusinessLogAppService
    {
        private ILogCrudRepository _logCrudRepository = null;

        public BusinessLogAppService(ILogCrudRepository logCrudRepository)
        {
            _logCrudRepository = logCrudRepository;
        }

        public void WriteBusinessLog(BusinessLogDto businessLogDto)
        {
            businessLogDto.create_date = DateTime.Now;
            _logCrudRepository.Insert(businessLogDto.JTransformTo<business_log>());
        }

        public InboundBizLogDto GetInboundBizLogByDays(int systemId, int days)
        {
            InboundBizLogDto response = new InboundBizLogDto();

            DateTime startDate = DateTime.Now.Date.AddDays(-days);
            DateTime endDate = DateTime.Now.Date.AddDays(1);

            response.InboundPorcessTotalCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Inbound && p.create_date > startDate && p.create_date <= endDate).Count();
            response.InboundPorcessSuccessCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Inbound && p.create_date > startDate && p.create_date <= endDate && p.flag == true).Count();
            response.InboundPorcessFailCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Inbound && p.create_date > startDate && p.create_date <= endDate && p.flag == false).Count();

            response.InboundBizApiDisplayDtoList = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Inbound && p.create_date > startDate && p.create_date <= endDate)
                .GroupBy(p => new { p.business_operation }).Select(p => new BizApiDisplayDto()
                {
                    BussinessName = p.Key.business_operation,
                    ProcessCount = p.Count()
                }).OrderByDescending(x => x.ProcessCount)
                .ToList();

            if (response.InboundBizApiDisplayDtoList.Count > 3)
                response.InboundBizApiDisplayDtoList = response.InboundBizApiDisplayDtoList.Take(3).ToList();
            return response;
        }

        public OutboundBizLogDto GetOutboundBizLogByDays(int systemId, int days)
        {
            OutboundBizLogDto response = new OutboundBizLogDto();

            DateTime startDate = DateTime.Now.Date.AddDays(-days);
            DateTime endDate = DateTime.Now.Date.AddDays(1);

            response.OutboundPorcessTotalCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Outbound && p.create_date > startDate && p.create_date <= endDate).Count();
            response.OutboundPorcessSuccessCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Outbound && p.create_date > startDate && p.create_date <= endDate && p.flag == true).Count();
            response.OutboundPorcessFailCount = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Outbound && p.create_date > startDate && p.create_date <= endDate && p.flag == false).Count();

            response.OutboundBizApiDisplayDtoList = _logCrudRepository.GetAllList<business_log>(p => p.system_id == systemId && p.business_type == PublicConst.BussinessType_Outbound && p.create_date > startDate && p.create_date <= endDate)
                .GroupBy(p => new { p.business_operation }).Select(p => new BizApiDisplayDto()
                {
                    BussinessName = p.Key.business_operation,
                    ProcessCount = p.Count()
                }).OrderByDescending(x => x.ProcessCount)
                .ToList();

            if (response.OutboundBizApiDisplayDtoList.Count > 3)
                response.OutboundBizApiDisplayDtoList = response.OutboundBizApiDisplayDtoList.Take(3).ToList();
            return response;
        }

        public ApiProcessResultTotalDto GetApiProcessResult(int systemId)
        {
            ApiProcessResultTotalDto response = new ApiProcessResultTotalDto();

            int totalMinutes = 10;
            DateTime startTime;
            DateTime endTime;
            DateTime now = DateTime.Now;
            int successCount = 0;
            int errorCount = 0;
            var time1 = DateTime.Parse((now.AddMinutes(-10).GetDateTimeFormats('g')[0]));
            var time2 = DateTime.Parse((now.GetDateTimeFormats('g')[0]));

            var access_logLsit = _logCrudRepository.GetQuery<access_log>(p => p.system_id == systemId && p.create_date > time1 && p.create_date <= time2).ToList();
            var business_logList = _logCrudRepository.GetQuery<business_log>(p => p.system_id == systemId && p.create_date > time1 && p.create_date <= time2).ToList();
            var interface_logList = _logCrudRepository.GetQuery<interface_log>(p => p.system_id == systemId && p.create_date > time1 && p.create_date <= time2).ToList();



            for (int minute = 1; minute <= totalMinutes; minute++)
            {
                startTime = DateTime.Parse((now.AddMinutes(-minute).GetDateTimeFormats('g')[0]));
                endTime = DateTime.Parse((now.AddMinutes(-minute + 1).GetDateTimeFormats('g')[0]));

                successCount = access_logLsit.Where(p => p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                successCount += business_logList.Where(p => p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                successCount += interface_logList.Where(p => p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();

                errorCount = access_logLsit.Where(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                errorCount += business_logList.Where(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                errorCount += interface_logList.Where(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();

                #region 注释原有查询
                //successCount = _logCrudRepository.GetQuery<access_log>(p => p.system_id == systemId && p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                //successCount += _logCrudRepository.GetQuery<business_log>(p => p.system_id == systemId && p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                //successCount += _logCrudRepository.GetQuery<interface_log>(p => p.system_id == systemId && p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();

                //errorCount = _logCrudRepository.GetQuery<access_log>(p => p.system_id == systemId && p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                //errorCount += _logCrudRepository.GetQuery<business_log>(p => p.system_id == systemId && p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                //errorCount += _logCrudRepository.GetQuery<interface_log>(p => p.system_id == systemId && p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                #endregion

                response.SuccessList.Add(new ApiProcessTotalDto() { minutes = (totalMinutes + 1 - minute), DisplayDate = startTime.ToString("HH:mm"), ProcessCount = successCount });
                response.ErrorList.Add(new ApiProcessTotalDto() { minutes = (totalMinutes + 1 - minute), DisplayDate = startTime.ToString("HH:mm"), ProcessCount = errorCount });
            }
            return response;
        }

        public ApiProcessResultTotalDto GetApiProcessResult(int systemId, int secondInterval)
        {
            ApiProcessResultTotalDto response = new ApiProcessResultTotalDto();

            int totalInterval = 10;
            DateTime startTime;
            DateTime endTime;
            DateTime now = DateTime.Now;
            int successCount = 0;
            int errorCount = 0;

            for (int interval = 0; interval < totalInterval; interval++)
            {
                startTime = DateTime.Parse((now.AddSeconds(-(interval + 1) * secondInterval).GetDateTimeFormats('s')[0]));
                endTime = DateTime.Parse((now.AddSeconds(-secondInterval * interval).GetDateTimeFormats('s')[0]));

                successCount = _logCrudRepository.GetQuery<access_log>(p => p.system_id == systemId && p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                //successCount += _logCrudRepository.GetQuery<business_log>(p => p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();
                //successCount += _logCrudRepository.GetQuery<interface_log>(p => p.flag == true && p.create_date > startTime && p.create_date <= endTime).Count();

                //errorCount = _logCrudRepository.GetQuery<access_log>(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                //errorCount += _logCrudRepository.GetQuery<business_log>(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();
                //errorCount += _logCrudRepository.GetQuery<interface_log>(p => p.flag == false && p.create_date > startTime && p.create_date <= endTime).Count();

                response.SuccessList.Add(new ApiProcessTotalDto() { minutes = (totalInterval - interval), DisplayDate = startTime.ToString("HH:mm:ss"), ProcessCount = successCount });
                //response.ErrorList.Add(new ApiProcessTotalDto() { minutes = (totalInterval - interval), DisplayDate = startTime.ToString("HH:mm:ss"), ProcessCount = errorCount });
            }
            return response;
        }

        public BusinessLogDto GetBusinessLogViewDto(Guid sysId)
        {
            return _logCrudRepository.Get<business_log>(sysId).JTransformTo<BusinessLogDto>();
        }

        public Pages<BusinessLogListDto> GetBusinessLogList(int systemId, BusinessLogQuery businessLogQuery)
        {
            var lambda = Wheres.Lambda<business_log>();
            lambda = lambda.And(p => p.system_id == systemId);
            if (!businessLogQuery.DescrSearch.IsNull())
            {
                lambda = lambda.And(p => p.descr.Contains(businessLogQuery.DescrSearch));
            }
            if (businessLogQuery.FlagSearch.HasValue)
            {
                lambda = lambda.And(p => p.flag == businessLogQuery.FlagSearch.Value);
            }
            if (businessLogQuery.CreateDateFromSearch.HasValue && businessLogQuery.CreateDateToSearch.HasValue)
            {
                businessLogQuery.CreateDateToSearch = businessLogQuery.CreateDateToSearch.Value.AddDays(1).AddMilliseconds(-1);
                lambda = lambda.And(p => businessLogQuery.CreateDateFromSearch.Value <= p.create_date && p.create_date <= businessLogQuery.CreateDateToSearch.Value);
            }

            businessLogQuery.iTotalDisplayRecords = _logCrudRepository.Count(lambda);
            var logs = _logCrudRepository.GetQuery(lambda).OrderByDescending(p => p.create_date).Skip(businessLogQuery.iDisplayStart).Take(businessLogQuery.iDisplayLength).Select(p =>
                new BusinessLogListDto
                {
                    LogType = 500,
                    SysId = p.SysId,
                    Descr = p.descr,
                    IsSuccess = p.flag,
                    CreateDate = p.create_date,
                    UserId = p.user_id,
                    UserName = p.user_name
                });
            return _logCrudRepository.ConvertPages(logs, businessLogQuery);
        }
    }
}
