using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System.Linq;
using Castle.Core.Internal;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class LotTemplateAppService : WMSApplicationService, ILotTemplateAppService
    {
        private ICrudRepository _crudRepository = null;
        public LotTemplateAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 删除一条 根据id
        /// </summary>
        /// <param name="sysId"></param>
        public void DeleteLotTemplate(List<Guid> sysId)
        {
            var skuCheck = _crudRepository.GetQuery<sku>(x => sysId.Contains(x.LotTemplateSysId));
            if (skuCheck.Any())
            {
                throw new Exception("模板被SKU占用,无法删除!");
            }

            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteLottemplate", method: MethodType.Post, postData: sysId);
                }).Start();
            }
            
            _crudRepository.Delete<lottemplate>(sysId);
            
        }

        /// <summary>
        /// 获取DTO
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public LotTemplateDto GetLotTemplateDtoById(Guid sysId)
        {
            return _crudRepository.Get<lottemplate>(sysId).TransformTo<LotTemplateDto>();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="lotTemplateQuery"></param>
        /// <returns></returns>
        public Pages<LotTemplateListDto> GetLotTemplateDtoListByPageInfo(LotTemplateQuery lotTemplateQuery)
        {
            #region 拼凑条件
            var lambda = Wheres.Lambda<lottemplate>();
            if (lotTemplateQuery != null)
            {
                if (!lotTemplateQuery.LotCodeSearch.IsNull())
                {
                    lambda = lambda.And(x => x.LotCode == lotTemplateQuery.LotCodeSearch.Trim());
                }
                if (!lotTemplateQuery.DescrSearch.IsNull())
                {
                    var desc = lotTemplateQuery.DescrSearch.Trim();
                    lambda = lambda.And(x => x.Descr.Contains(desc));
                }
            }
            #endregion
            return _crudRepository.GetQueryableByPage<lottemplate, LotTemplateListDto>(lotTemplateQuery, lambda);
        }

        /// <summary>
        /// 写入批次模板
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        public void InsertLotTemplate(LotTemplateDto lotTemplateDto)
        {
            var lotCheck = _crudRepository.GetQuery<lottemplate>(x => x.LotCode == lotTemplateDto.LotCode);
            if (lotCheck.Any())
            {
                throw new Exception("批次模板代码已经存在!");
            }
            var lotTemp = lotTemplateDto.TransformTo<lottemplate>();
            lotTemp.SysId = Guid.NewGuid();
            try
            {
                if (PublicConst.SyncMultiWHSwitch)
                {
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateLottemplate", method: MethodType.Post, postData: lotTemp);
                    }).Start();
                }
                
                _crudRepository.Insert(lotTemp);
                
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        /// <summary>
        /// 更新批次模板
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        public void UpdateLotTemplate(LotTemplateDto lotTemplateDto)
        {
            var lotTemp = lotTemplateDto.TransformTo<lottemplate>();
            try
            {
                if (PublicConst.SyncMultiWHSwitch)
                {
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateLottemplate", method: MethodType.Post, postData: lotTemp);
                    }).Start();
                }
                
                _crudRepository.Update(lotTemp);
                
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        /// <summary>
        /// 返回Select对象
        /// </summary>
        /// <param name="lotCode"></param>
        /// <returns></returns>
        public List<SelectItem> GetSelectLotTemplate(string lotCode = null)
        {
            var lotTemp = _crudRepository.GetAll<lottemplate>();
            if (!string.IsNullOrEmpty(lotCode))
            {
                lotTemp = lotTemp.Where(x => x.LotCode == lotCode);
            }

            var list = new List<SelectItem>();
            lotTemp.ToList().ForEach(info =>
            {
                var item = new SelectItem
                {
                    Text = info.LotCode,
                    Value = info.SysId.ToString()
                };
                list.Add(item);
            });
            return list;
        }
    }
}