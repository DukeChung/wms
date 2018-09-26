using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Application
{
    public class QualityControlAppService : WMSApplicationService, IQualityControlAppService
    {
        private ICrudRepository _crudRepository = null;
        private IQualityControlRepository _qualityControlRepository = null;
        private IPackageAppService _packageAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;

        public QualityControlAppService(ICrudRepository crudRepository, IQualityControlRepository qualityControlRepository, IPackageAppService packageAppService, IThirdPartyAppService thirdPartyAppService)
        {
            this._crudRepository = crudRepository;
            this._qualityControlRepository = qualityControlRepository;
            this._packageAppService = packageAppService;
            this._thirdPartyAppService = thirdPartyAppService;
        }

        public Pages<QualityControlListDto> GetQualityControlList(QualityControlQuery qualityControlQuery)
        {
            _crudRepository.ChangeDB(qualityControlQuery.WarehouseSysId);
            return _qualityControlRepository.GetQualityControlList(qualityControlQuery);
        }

        public void DeleteQualityControl(List<Guid> sysIds,Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var notNewQCCount = _crudRepository.GetQuery<qualitycontrol>(p => sysIds.Contains(p.SysId) && p.Status != (int)QCStatus.New).Count();
            if (notNewQCCount == 0)
            {
                foreach (var sysId in sysIds)
                {
                    _crudRepository.Delete<qualitycontroldetail>(p => p.QualityControlSysId == sysId);
                    _crudRepository.Delete<qualitycontrol>(sysId);
                }
            }
            else
            {
                throw new Exception("只能删除新建状态的盘点单, 请刷新页面");
            }
        }

        public QualityControlDto GetQualityControlViewDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var qualityControl = _crudRepository.GetQuery<qualitycontrol>(p => p.SysId == sysId).FirstOrDefault();
            if (qualityControl != null)
            {
                return qualityControl.JTransformTo<QualityControlDto>();
            }
            else
            {
                throw new Exception("未找到质检单数据");
            }
        }

        public Pages<DocDetailDto> GetDocDetails(DocDetailQuery docDetailQuery)
        {
            _crudRepository.ChangeDB(docDetailQuery.WarehouseSysId);
            return _qualityControlRepository.GetDocDetails(docDetailQuery);
        }

        public Pages<QualityControlDetailDto> GetQCDetails(QCDetailQuery qcDetailQuery)
        {
            _crudRepository.ChangeDB(qcDetailQuery.WarehouseSysId);
            return _qualityControlRepository.GetQCDetails(qcDetailQuery);
        }

        public void SaveQualityControl(SaveQualityControlDto saveQualityControlDto)
        {
            _crudRepository.ChangeDB(saveQualityControlDto.WarehouseSysId);
            var qualityControl = _crudRepository.GetQuery<qualitycontrol>(p => p.SysId == saveQualityControlDto.QualityControlSysId).FirstOrDefault();
            if (qualityControl == null) throw new Exception("质检单不存在");
            if (qualityControl.Status != (int)QCStatus.New) throw new Exception(string.Format("质检单状态为{0},无法编辑", ((QCStatus)qualityControl.Status).ToDescription()));

            var qcSkuSysIds = saveQualityControlDto.QCDetails != null ? saveQualityControlDto.QCDetails.Select(p => p.SkuSysId).Distinct().ToList() : new List<Guid>();
            var docDetails = _qualityControlRepository.GetDocDetails(qualityControl.DocOrder, qualityControl.QCType, qcSkuSysIds);
            if (docDetails != null && saveQualityControlDto.QCDetails != null)
            {
                foreach (var docDetail in docDetails)
                {
                    var qcDetailSkuTotalQty = saveQualityControlDto.QCDetails.Where(p => p.SkuSysId == docDetail.SkuSysId).Sum(p => p.DisplayQty);
                    if (qcDetailSkuTotalQty > docDetail.DisplayQty)
                    {
                        throw new Exception(string.Format("商品[{0}]质检不合格数量不能大于相关单据商品数量", docDetail.UPC));
                    }
                }
            }

            _crudRepository.Update<qualitycontrol>(saveQualityControlDto.QualityControlSysId, p =>
            {
                p.UpdateBy = saveQualityControlDto.CurrentUserId;
                p.UpdateDate = DateTime.Now;
                p.UpdateUserName = saveQualityControlDto.CurrentDisplayName;
            });
            _crudRepository.Delete<qualitycontroldetail>(p => p.QualityControlSysId == saveQualityControlDto.QualityControlSysId);
            if (saveQualityControlDto.QCDetails != null)
            {
                foreach (var qcDetail in saveQualityControlDto.QCDetails)
                {
                    int transQty = 0;
                    pack transPack = new pack();
                    if (_packageAppService.GetSkuConversiontransQty(qcDetail.SkuSysId, qcDetail.DisplayQty.GetValueOrDefault(), out transQty, ref transPack) == false)
                    {
                        transQty = Convert.ToInt32(qcDetail.DisplayQty);
                    }
                    qualitycontroldetail detail = new qualitycontroldetail
                    {
                        SysId = Guid.NewGuid(),
                        QualityControlSysId = qcDetail.QualityControlSysId,
                        SkuSysId = qcDetail.SkuSysId,
                        CreateBy = saveQualityControlDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = saveQualityControlDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UOMSysId = qcDetail.UOMSysId,
                        PackSysId = qcDetail.PackSysId,
                        Qty = transQty,
                        Descr = qcDetail.Descr
                    };
                    _crudRepository.Insert(detail);
                }
            }
        }

        /// <summary>
        /// 质检完成
        /// </summary>
        /// <param name="sysId"></param>
        public void FinishQualityControl(FinishQualityControlDto finishQualityControlDto)
        {
            _crudRepository.ChangeDB(finishQualityControlDto.WarehouseSysId);
            var qualityControl = _crudRepository.GetQuery<qualitycontrol>(p => p.SysId == finishQualityControlDto.SysId).FirstOrDefault();
            if (qualityControl == null) throw new Exception("质检单不存在");
            if (qualityControl.Status != (int)QCStatus.New) throw new Exception(string.Format("质检单状态为{0},无法质检完成", ((QCStatus)qualityControl.Status).ToDescription()));

            var qcDetails = _crudRepository.GetQuery<qualitycontroldetail>(p => p.QualityControlSysId == finishQualityControlDto.SysId).ToList();
            var qcDetailSysIds = qcDetails.Select(p => p.SkuSysId);
            var skus = _crudRepository.GetQuery<sku>(p => qcDetailSysIds.Contains(p.SysId));

            ThirdPartyFinishQualityControlDto request = new ThirdPartyFinishQualityControlDto { QCProductRecords = new List<ThirdPartyFinishQualityControlDetailDto>() };
            request.OriginalOrderId = Convert.ToInt32(qualityControl.ExternOrderId);
            request.QCDate = DateTime.Now;
            request.Memo = qualityControl.Descr;

            request.CurrentUserId = finishQualityControlDto.CurrentUserId;
            request.CurrentDisplayName = finishQualityControlDto.CurrentDisplayName;
            foreach (var qcDetail in qcDetails)
            {
                ThirdPartyFinishQualityControlDetailDto qcProductRecord = new ThirdPartyFinishQualityControlDetailDto();
                qcProductRecord.ProductCode = Convert.ToInt32(skus.FirstOrDefault(p => p.SysId == qcDetail.SkuSysId).OtherId);
                qcProductRecord.ProductQty = qcDetail.Qty.GetValueOrDefault();
                qcProductRecord.Reason = qcDetail.Descr;
                request.QCProductRecords.Add(qcProductRecord);
            }
            var rsp = _thirdPartyAppService.FinishQualityControl(request, qualityControl);
            if (rsp.IsSuccess)
            {
                _crudRepository.Update<qualitycontrol>(finishQualityControlDto.SysId, p =>
                {
                    p.Status = (int)QCStatus.Finish;
                    p.QCDate = DateTime.Now;
                    p.QCUserName = finishQualityControlDto.CurrentDisplayName;
                    p.UpdateBy = finishQualityControlDto.CurrentUserId;
                    p.UpdateDate = DateTime.Now;
                    p.UpdateUserName = finishQualityControlDto.CurrentDisplayName;
                });
            }
            else
            {
                throw new Exception($"调用ECC接口失败:{rsp.ErrorMessage}");
            }
        }

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        public AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto)
        {
            _crudRepository.ChangeDB(createAdjustmentDto.WarehouseSysId);
            var adjustmentOrder = _crudRepository.GetQuery<adjustment>(p => p.SourceOrder == createAdjustmentDto.SourceOrder && p.Status != (int)AdjustmentStatus.Void).FirstOrDefault();
            if (adjustmentOrder != null)
            {
                throw new Exception("已存在有效状态损益单，请勿重复创建");
            }

            List<qualitycontroldetail> qcDetails = _crudRepository.GetAllList<qualitycontroldetail>(p => createAdjustmentDto.DetailSysIds.Contains(p.SysId));
            List<Guid> skuSysIds = qcDetails.Select(p => p.SkuSysId).ToList();
            List<sku> skuList = _crudRepository.GetAllList<sku>(p => skuSysIds.Contains(p.SysId));

            AdjustmentDto adjustmentDto = new AdjustmentDto { AdjustmentDetailList = new List<AdjustmentDetailDto>() };
            adjustmentDto.WarehouseSysId = createAdjustmentDto.WarehouseSysId;

            adjustmentDto.Type = (int)AdjustmentType.ReturnLoss;
            adjustmentDto.SourceType = PublicConst.AJSourceTypeQC;
            adjustmentDto.SourceOrder = createAdjustmentDto.SourceOrder;
            if (qcDetails != null && qcDetails.Any())
            {
                qcDetails.ForEach(p =>
                {
                    sku sku = skuList.FirstOrDefault(x => x.SysId == p.SkuSysId);
                    if (sku != null)
                    {
                        if (p.Qty.GetValueOrDefault() > 0)
                        {
                            adjustmentDto.AdjustmentDetailList.Add(new AdjustmentDetailDto
                            {
                                SkuSysId = sku.SysId,
                                SkuCode = sku.SkuCode,
                                SkuName = sku.SkuName,
                                SkuDescr = sku.SkuDescr,
                                Loc = p.Loc,
                                Lot = p.Lot,
                                Lpn = p.Lpn,
                                Qty = -p.Qty.GetValueOrDefault(),
                                Remark = p.Descr
                            });
                        }
                    }
                });
            }
            return adjustmentDto;
        }
    }
}
