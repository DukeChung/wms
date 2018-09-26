using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using FortuneLab.Models;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System.Data.Linq.SqlClient;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class SysCodeAppService : WMSApplicationService, ISysCodeAppService
    {
        private ICrudRepository _crudRepository = null;

        public SysCodeAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 根据SysCodeId 获取相关数据
        /// </summary>
        /// <param name="sysCodeId"></param>
        /// <returns></returns>
        public SysCodeDto GetSysCodeDtoById(Guid sysCodeId)
        {
            var sysCode = _crudRepository.Get<syscode>(sysCodeId);
            return sysCode.TransformTo<SysCodeDto>();
        }

        /// <summary>
        /// 根据SysId 获取明细记录
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public SysCodeDetailDto GetSysCodeDetailDtoById(Guid sysId)
        {
            var sysCodeDetail = _crudRepository.Get<syscodedetail>(sysId);
            return sysCodeDetail.JTransformTo<SysCodeDetailDto>();
        }

        /// <summary>
        /// 根据SysId 获取明细记录
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public void DeleteSysCodeDetailByIdList(List<Guid> sysIdList)
        {
            if (sysIdList.Any())
            {
                _crudRepository.Delete<syscodedetail>(sysIdList);
            }

        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="sysCodeQuery"></param>
        /// <returns></returns>
        public Pages<SysCodeDto> GetSysCodeDtoListByPageInfo(SysCodeQuery sysCodeQuery)
        {
            #region 拼凑条件
            var lambda = Wheres.Lambda<syscode>();
            if (sysCodeQuery != null)
            {
                if (!sysCodeQuery.SysCodeTypeSearch.IsNull())
                {
                    lambda = lambda.And(x => x.SysCodeType == sysCodeQuery.SysCodeTypeSearch.Trim());
                }
                if (!sysCodeQuery.DescrSearch.IsNull())
                {
                    var desc = sysCodeQuery.DescrSearch.Trim();
                    lambda = lambda.And(x => x.Descr.Contains(desc));
                }
            }
            #endregion
            return _crudRepository.GetQueryableByPage<syscode, SysCodeDto>(sysCodeQuery, lambda);

        }

        /// <summary>
        /// 获取SysCodeDetailDto 数据
        /// </summary>
        /// <param name="sysCodeSysId"></param>
        /// <returns></returns>
        public List<SysCodeDetailDto> GetSysCodeDetailDtoList(Guid sysCodeSysId)
        {
            var sysCodeDetailList = _crudRepository.GetQuery<syscodedetail>(x => x.SysCodeSysId == sysCodeSysId).ToList();
            return sysCodeDetailList.OrderBy(x => x.SeqNo).JTransformTo<SysCodeDetailDto>();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sysCodeDto"></param>
        /// <returns></returns>
        public void UpdateSysCode(SysCodeDto sysCodeDto)
        {
            var sysCode = _crudRepository.GetQuery<syscode>(p => p.SysId == sysCodeDto.SysId).FirstOrDefault();
            sysCode.Descr = sysCodeDto.Descr;
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSyscode", method: MethodType.Post, postData: sysCode);
                }).Start();
            }
            
            _crudRepository.Update(sysCode);
            
        }

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public void UpdateSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            var sysCodeDetail = _crudRepository.Get<syscodedetail>(sysCodeDetailDto.SysId.Value);
            sysCodeDetail.Descr = sysCodeDetailDto.Descr;
            sysCodeDetail.Code = sysCodeDetailDto.Code;
            sysCodeDetail.SeqNo = sysCodeDetailDto.SeqNo;
            sysCodeDetail.IsActive = sysCodeDetailDto.IsActive;
            sysCodeDetail.UpdateDate = DateTime.Now;
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSyscodeDetail", method: MethodType.Post, postData: sysCodeDetail);
                }).Start();
            }
            
            _crudRepository.Update(sysCodeDetail);
            
        }

        /// <summary>
        /// 新增明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public void InsertSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            var sysCodeDetail = sysCodeDetailDto.TransformTo<syscodedetail>();
            sysCodeDetail.CreateDate = DateTime.Now;
            sysCodeDetail.UpdateDate = DateTime.Now;
            sysCodeDetail.SysId = Guid.NewGuid();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateSyscodeDetail", method: MethodType.Post, postData: sysCodeDetail);
                }).Start();
            }
            
            _crudRepository.Insert(sysCodeDetail);
            
        }

        /// <summary>
        /// 获取系统代码明细
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public List<SelectItem> GetSysCodeDetailList(string sysCodeType, bool isActive)
        {
            var sysCode = _crudRepository.GetQuery<syscode>(x => x.SysCodeType == sysCodeType).ToList();
            if (sysCode.Any())
            {
                var detailList = sysCode.FirstOrDefault().syscodedetails.Where(p => p.IsActive == isActive).OrderBy(x => x.SeqNo).ToList();
                var list = new List<SelectItem>();
                detailList.ForEach(item =>
               {
                   list.Add(new SelectItem()
                   {
                       Text = item.Descr,
                       Value = item.Code
                   });
               });
                return list;
            }
            else
            {
                throw new Exception("系统代码不存在");
            }
        }
    }
}