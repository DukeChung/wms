using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class WorkUserAppService : WMSApplicationService, IWorkUserAppService
    {
        private ICrudRepository _crudRepository = null;
        private IWorkUserRepository _workUserRepository = null;

        public WorkUserAppService(ICrudRepository crudRepository, IWorkUserRepository workUserRepository)
        {
            this._crudRepository = crudRepository;
            this._workUserRepository = workUserRepository;
        }

        /// <summary>
        /// 获取工单用户列表
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        public Pages<WorkUserDto> GetWorkUserList(WorkUserQuery workUserQuery)
        {
            _crudRepository.ChangeDB(workUserQuery.WarehouseSysId);
            return _workUserRepository.GetWorkUserList(workUserQuery);
        }

        /// <summary>
        /// 新增工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        public void AddWorkUser(WorkUserDto workUserDto)
        {
            _crudRepository.ChangeDB(workUserDto.WarehouseSysId);
            var workuser = _crudRepository.GetQuery<workuser>(p => p.WorkUserCode == workUserDto.WorkUserCode.Trim() && p.WarehouseSysId == workUserDto.WarehouseSysId).FirstOrDefault();
            if (workuser != null)
            {
                throw new Exception("已存在相同的用户编号");
            }
            workUserDto.SysId = Guid.NewGuid();
            workUserDto.WorkUserCode = workUserDto.WorkUserCode.Trim();
            workUserDto.WorkUserName = workUserDto.WorkUserName.Trim();
            workUserDto.CreateDate = DateTime.Now;
            workUserDto.UpdateDate = DateTime.Now;
            //var workuser = workUserDto.TransformTo<workuser>();
            _crudRepository.Insert(workUserDto.TransformTo<workuser>());
        }

        /// <summary>
        /// 根据Id获取工单用户
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSydId"></param>
        /// <returns></returns>
        public WorkUserDto GetWorkUserById(Guid sysId, Guid warehouseSydId)
        {
            _crudRepository.ChangeDB(warehouseSydId);
            var workUser = _crudRepository.GetQuery<workuser>(p => p.SysId == sysId).FirstOrDefault();
            if (workUser == null)
            {
                throw new Exception("获取工单用户信息失败");
            }
            return workUser.TransformTo<WorkUserDto>();
        }

        /// <summary>
        /// 获取所有工单用户
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        public List<WorkUserListDto> GetWorkUsers(WorkUserQuery workUserQuery)
        {
            _crudRepository.ChangeDB(workUserQuery.WarehouseSysId);
            var result = new List<WorkUserListDto>();
            var list = _crudRepository.GetAllList<workuser>(x => x.IsActive == true && x.WarehouseSysId == workUserQuery.WarehouseSysId);
            result = list.TransformTo<WorkUserListDto>();
            return result;
        }

        /// <summary>
        /// 编辑工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        public void EditWorkUser(WorkUserDto workUserDto)
        {
            _crudRepository.ChangeDB(workUserDto.WarehouseSysId);
            var workuser = _crudRepository.GetQuery<workuser>(p => p.WorkUserCode == workUserDto.WorkUserCode.Trim() && p.WarehouseSysId == workUserDto.WarehouseSysId && p.SysId != workUserDto.SysId).FirstOrDefault();
            if (workuser != null)
            {
                throw new Exception("已存在相同的用户编号");
            }
            workuser = _crudRepository.GetQuery<workuser>(p => p.SysId == workUserDto.SysId).FirstOrDefault();
            workuser.WorkUserCode = workUserDto.WorkUserCode.Trim();
            workuser.WorkUserName = workUserDto.WorkUserName.Trim();
            workuser.WorkType = workUserDto.WorkType;
            workuser.WorkStatus = workUserDto.WorkStatus;
            workuser.IsActive = workUserDto.IsActive;
            workuser.UpdateBy = workUserDto.UpdateBy;
            workuser.UpdateDate = DateTime.Now;
            workuser.UpdateUserName = workUserDto.UpdateUserName;
            _crudRepository.Update(workuser);
        }

        /// <summary>
        /// 删除工单用户
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="warehouseSydId"></param>
        public void DeleteWorkUser(List<Guid> sysIds, Guid warehouseSydId)
        {
            _crudRepository.ChangeDB(warehouseSydId);
            var work = _crudRepository.GetQuery<work>(p => sysIds.Contains(p.AppointUserId.Value) && (p.Status == (int)WorkStatus.Hang || p.Status == (int)WorkStatus.Working)).FirstOrDefault();
            if (work != null)
            {
                throw new Exception("存在有效状态工单，不能删除用户");
            }
            _crudRepository.Delete<workuser>(sysIds);
        }
    }
}
