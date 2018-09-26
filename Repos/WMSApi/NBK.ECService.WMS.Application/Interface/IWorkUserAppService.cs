using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IWorkUserAppService : IApplicationService
    {
        /// <summary>
        /// 获取工单用户列表
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        Pages<WorkUserDto> GetWorkUserList(WorkUserQuery workUserQuery);

        /// <summary>
        /// 新增工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        void AddWorkUser(WorkUserDto workUserDto);

        /// <summary>
        /// 根据Id获取工单用户
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSydId"></param>
        /// <returns></returns>
        WorkUserDto GetWorkUserById(Guid sysId, Guid warehouseSydId);

        List<WorkUserListDto> GetWorkUsers(WorkUserQuery workUserQuery);

        /// <summary>
        /// 编辑工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        void EditWorkUser(WorkUserDto workUserDto);

        /// <summary>
        /// 删除工单用户
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="warehouseSydId"></param>
        void DeleteWorkUser(List<Guid> sysIds, Guid warehouseSydId);
    }
}
