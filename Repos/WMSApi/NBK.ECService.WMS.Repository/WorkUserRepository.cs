using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class WorkUserRepository : CrudRepository, IWorkUserRepository
    {
        /// <param name="dbContextProvider"></param>
        public WorkUserRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取工单用户列表
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        public Pages<WorkUserDto> GetWorkUserList(WorkUserQuery workUserQuery)
        {
            var query = from wu in Context.workusers
                        where wu.WarehouseSysId == workUserQuery.WarehouseSysId
                        select new { wu };
            if (!workUserQuery.WorkUserCodeSearch.IsNull())
            {
                workUserQuery.WorkUserCodeSearch = workUserQuery.WorkUserCodeSearch.Trim();
                query = query.Where(p => p.wu.WorkUserCode.Contains(workUserQuery.WorkUserCodeSearch));
            }
            if (!workUserQuery.WorkUserNameSearch.IsNull())
            {
                workUserQuery.WorkUserNameSearch = workUserQuery.WorkUserNameSearch.Trim();
                query = query.Where(p => p.wu.WorkUserName.Contains(workUserQuery.WorkUserNameSearch));
            }
            if (workUserQuery.IsActiveSearch.HasValue)
            {
                query = query.Where(p => p.wu.IsActive == workUserQuery.IsActiveSearch.Value);
            }
            if (workUserQuery.WorkTypeSearch.HasValue)
            {
                query = query.Where(p => p.wu.WorkType == workUserQuery.WorkTypeSearch.Value);
            }
            if (workUserQuery.WorkStatusSearch.HasValue)
            {
                query = query.Where(p => p.wu.WorkStatus == workUserQuery.WorkStatusSearch.Value);
            }
            var workUsers = query.Select(p => new WorkUserDto
            {
                SysId = p.wu.SysId,
                WorkUserCode = p.wu.WorkUserCode,
                WorkUserName = p.wu.WorkUserName,
                IsActive = p.wu.IsActive,
                WorkType = p.wu.WorkType,
                WorkStatus = p.wu.WorkStatus,
                CreateDate = p.wu.CreateDate
            });
            workUserQuery.iTotalDisplayRecords = workUsers.Count();
            workUsers = workUsers.OrderByDescending(p => p.CreateDate).Skip(workUserQuery.iDisplayStart).Take(workUserQuery.iDisplayLength);
            return ConvertPages(workUsers, workUserQuery);
        }
    }
}
