using Abp.Application.Services;
using FortuneLab.WebApiClient;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.Securities;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class ContainerAppService : WMSApplicationService, IContainerAppService
    {
        private ICrudRepository _crudRepository = null;
        public ContainerAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        public Pages<ContainerDto> GetContainerList(ContainerQuery query)
        {
            #region 拼凑条件
            var lambda = Wheres.Lambda<container>();
            if (query != null)
            {
                if (!query.ContainerName.IsNull())
                {
                    var name = query.ContainerName.Trim();
                    lambda = lambda.And(x => x.ContainerName.Contains(name));
                }
            }

            #endregion

            return _crudRepository.GetQueryableByPage<container, ContainerDto>(query, lambda);
        }

        public void DeleteContainer(string sysIdList, Guid warehouseSysId)
        {
            //foreach (var id in sysIdList.Trim(',').Split(','))
            //{
            //    Guid sysId = new Guid(id);
            //    //后期根据业务扩展补充删除的验证条件

            //    _crudRepository.Delete<container>(sysId);
            //}
            var sysIds = sysIdList.ToGuidList();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteContainer", method: MethodType.Post, postData: sysIds);
                }).Start();
            }
            
            _crudRepository.Delete<container>(sysIds);
            
        }

        public void AddContainer(ContainerDto containerDto)
        {
            containerDto.SysId = Guid.NewGuid();
            var container = containerDto.TransformTo<container>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateContainer", method: MethodType.Post, postData: container);
                }).Start();
            }
            
            _crudRepository.InsertAndGetId(containerDto.TransformTo<container>());
            
        }

        public ContainerDto GetContainerBySysId(Guid sysId, Guid warehouseSysId)
        {
            var container = _crudRepository.Get<container>(sysId);
            return container.TransformTo<ContainerDto>();
        }

        public void UpdateContainer(ContainerDto containerDto)
        {
            var container = containerDto.TransformTo<container>();
            container.UpdateBy = 1;
            container.UpdateDate = DateTime.Now;
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateContainer", method: MethodType.Post, postData: container);
                }).Start();
            }
            
            _crudRepository.Update(container);
            
        }

        /// <summary>
        /// 获取全部有效箱子
        /// </summary>
        /// <returns></returns>
        public List<ContainerDto> GetContainerListByIsActive(Guid warehouseSysId)
        {
            var container = _crudRepository.GetQuery<container>(x => x.IsActive);
            return container.JTransformTo<ContainerDto>();
        }
    }
}
