using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IWorkUserRepository : ICrudRepository
    {
        /// <summary>
        /// 获取工单用户列表
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        Pages<WorkUserDto> GetWorkUserList(WorkUserQuery workUserQuery);
    }
}
