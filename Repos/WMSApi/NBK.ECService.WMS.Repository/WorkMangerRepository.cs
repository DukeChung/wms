using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class WorkMangerRepository : CrudRepository, IWorkMangerRepository
    {
        public WorkMangerRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 分页获取工单
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        public Pages<WorkListDto> GetWorkByPage(WorkQueryDto request)
        {
            var query = from w in Context.works
                        where w.WarehouseSysId == request.WarehouseSysId
                        select new WorkListDto
                        {
                            SysId = w.SysId,
                            WorkOrder = w.WorkOrder,
                            Source = w.Source,
                            Status = w.Status,
                            WorkType = w.WorkType,
                            AppointUserName = w.AppointUserName,
                            DocOrder = w.DocOrder,
                            CreateDate = w.CreateDate
                        };
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.WorkOrder))
                {
                    request.WorkOrder = request.WorkOrder.Trim();
                    query = query.Where(x => x.WorkOrder == request.WorkOrder);
                }
                if (!string.IsNullOrEmpty(request.DocOrder))
                {
                    request.DocOrder = request.DocOrder.Trim();
                    query = query.Where(x => x.DocOrder == request.DocOrder);
                }
                if (!string.IsNullOrEmpty(request.AppointUserName))
                {
                    request.AppointUserName = request.AppointUserName.Trim();
                    query = query.Where(x => x.AppointUserName == request.AppointUserName);
                }
                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }
                if (request.WorkType.HasValue)
                {
                    query = query.Where(x => x.WorkType == request.WorkType.Value);
                }
            }
            request.iTotalDisplayRecords = query.Count();
            query = query.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(query, request);
        }

    }
}
