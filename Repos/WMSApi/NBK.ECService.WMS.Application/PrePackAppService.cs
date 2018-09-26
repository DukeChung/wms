using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class PrePackAppService : IPrePackAppService
    {
        private ICrudRepository _crudRepository = null;
        private IPrePackCrudRepository _prePackcrudRepository = null;
        private IBaseAppService _baseAppService = null;

        public PrePackAppService(ICrudRepository crudRepository, IPrePackCrudRepository prePackcrudRepository, IBaseAppService baseAppService)
        {
            _crudRepository = crudRepository;
            _prePackcrudRepository = prePackcrudRepository;
            this._baseAppService = baseAppService;
        }

        #region 分页获取与包装单
        /// <summary>
        /// 分页获取与包装单
        /// </summary>
        /// <param name="perPackQuery"></param>
        /// <returns></returns>
        public Pages<PrePackListDto> GetPrePackByPage(PrePackQuery perPackQuery)
        {
            try
            {
                _crudRepository.ChangeDB(perPackQuery.WarehouseSysId);
                return _prePackcrudRepository.GetPrePackByPage(perPackQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 获取预包装库存
        /// <summary>
        /// 获取预包装库存
        /// </summary>
        /// <param name="prePackSkuQuery"></param>
        /// <returns></returns>
        public Pages<PrePackSkuListDto> GetPrePackSkuByPage(PrePackSkuQuery prePackSkuQuery)
        {
            try
            {
                _crudRepository.ChangeDB(prePackSkuQuery.WarehouseSysId);
                return _prePackcrudRepository.GetPrePackSkuByPage(prePackSkuQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 新增预包装

        /// <summary>
        /// 新增预包装明细
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        public bool SavePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            try
            {
                _crudRepository.ChangeDB(prePackSkuDto.WarehouseSysId);
                if (prePackSkuDto != null)
                {
                    #region 组织预包装单
                    var prePack = new prepack()
                    {
                        SysId = Guid.NewGuid(),
                        StorageLoc = prePackSkuDto.StorageLoc,
                        WareHouseSysId = prePackSkuDto.WarehouseSysId,
                        //PrePackOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPrePack),
                        PrePackOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPrePack),
                        CreateBy = prePackSkuDto.CurrentUserId,
                        UpdateBy = prePackSkuDto.CurrentUserId,
                        CreateUserName = prePackSkuDto.CurrentDisplayName,
                        UpdateUserName = prePackSkuDto.CurrentDisplayName,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        BatchNumber = prePackSkuDto.BatchNumber,
                        ServiceStationName = prePackSkuDto.ServiceStationName,
                        Source = "WMS",
                        Status = (int)PrePackStatus.New
                    };
                    _crudRepository.Insert(prePack);
                    #endregion

                    #region 预包装单明细
                    if (prePackSkuDto.PrePackSkuListDto != null && prePackSkuDto.PrePackSkuListDto.Count > 0)
                    {
                        var prepackdetailList = new List<prepackdetail>();
                        var skus = (from p in prePackSkuDto.PrePackSkuListDto group p by p.SkuSysId into g select g.Key).ToList();

                        var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.SysId)).ToList();
                        var packList = _crudRepository.GetAllList<pack>().ToList();
                        var uomList = _crudRepository.GetAllList<uom>().ToList();

                        foreach (var detail in prePackSkuDto.PrePackSkuListDto)
                        {
                            var detailSku = skuList.Find(x => x.SysId == detail.SkuSysId);
                            pack detailPack = null;
                            if (detailSku != null)
                            {
                                detailPack = packList.Find(x => x.SysId == detailSku.PackSysId);
                            }
                            if (detailPack == null)
                            {
                                throw new Exception("商品:" + detailSku.SkuName + ",包装代码不存在");
                            }

                            uom detailUom = null;
                            if (detailPack != null)
                            {
                                detailUom = uomList.Find(x => x.SysId == Guid.Parse(detailPack.FieldUom01.ToString()));
                            }
                            if (detailUom == null)
                            {
                                throw new Exception("包装:" + detailPack.PackCode + ",单位不存在");
                            }
                            var prepackdetail = new prepackdetail()
                            {
                                SysId = Guid.NewGuid(),
                                PrePackSysId = prePack.SysId,
                                SkuSysId = detail.SkuSysId,
                                UOMSysId = detailUom.SysId,
                                PackSysId = detailPack.SysId,
                                Loc = detail.Loc,
                                Lot = detail.Lot,
                                CreateBy = prePackSkuDto.CurrentUserId,
                                UpdateBy = prePackSkuDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                CreateDate = DateTime.Now,
                                PreQty = detail.PreQty,
                                LotAttr01 = detail.LotAttr01,
                                LotAttr02 = detail.LotAttr02,
                                LotAttr03 = detail.LotAttr03,
                                LotAttr04 = detail.LotAttr04,
                                LotAttr05 = detail.LotAttr05,
                                LotAttr06 = detail.LotAttr06,
                                LotAttr07 = detail.LotAttr07,
                                LotAttr08 = detail.LotAttr08,
                                LotAttr09 = detail.LotAttr09,
                                CreateUserName = prePackSkuDto.CurrentDisplayName,
                                UpdateUserName = prePackSkuDto.CurrentDisplayName
                            };
                            if (!string.IsNullOrEmpty(detail.ProduceDateDisplay))
                                prepackdetail.ProduceDate = Convert.ToDateTime(detail.ProduceDateDisplay);
                            if (!string.IsNullOrEmpty(detail.ExpiryDateDisplay))
                                prepackdetail.ExpiryDate = Convert.ToDateTime(detail.ExpiryDateDisplay);
                            prepackdetailList.Add(prepackdetail);
                        }
                        _crudRepository.BatchInsert(prepackdetailList);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception("新增预包装明细失败" + ex.Message);
            }
            return true;
        }
        #endregion

        #region 预报单明细
        /// <summary>
        /// 预报单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PrePackSkuDto GetPrePackBySysId(Guid sysId,Guid warehouseSysId)
        {
            try
            {
                _crudRepository.ChangeDB(warehouseSysId);
                var pp = _prePackcrudRepository.GetPrePackBySysId(sysId);
                PrePackSkuDto response = pp.JTransformTo<PrePackSkuDto>();
                response.PrePackSkuListDto = _prePackcrudRepository.GetPrePackDetailBySysId(sysId, response.BatchNumber);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("获取预包装单失败" + ex.Message);
            }
        }
        #endregion

        #region 更新预包装

        /// <summary>
        /// 更新预包装明细
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        public bool UpdatePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            try
            {
                _crudRepository.ChangeDB(prePackSkuDto.WarehouseSysId);
                if (prePackSkuDto == null || prePackSkuDto.PrePackSkuListDto == null)
                {
                    throw new Exception("传入预包装单无效,缺失主要数据源");
                }
                var prepackSource = _crudRepository.FirstOrDefault<prepack>(p => p.SysId == prePackSkuDto.SysId);
                if (prepackSource == null)
                {
                    throw new Exception("请求的预包装单不存在！");
                }
                prepackSource.UpdateBy = prePackSkuDto.CurrentUserId;
                prepackSource.UpdateDate = DateTime.Now;
                prepackSource.UpdateUserName = prePackSkuDto.CurrentDisplayName;
                prepackSource.StorageLoc = prePackSkuDto.StorageLoc;
                prepackSource.BatchNumber = prePackSkuDto.BatchNumber;
                prepackSource.ServiceStationName = prePackSkuDto.ServiceStationName;
                //更新预包装单
                _crudRepository.Update<prepack>(prepackSource);

                //删除预包装单明细
                _crudRepository.Delete<prepackdetail>(x => x.PrePackSysId == prepackSource.SysId);

                #region 预包装单明细
                if (prePackSkuDto.PrePackSkuListDto != null && prePackSkuDto.PrePackSkuListDto.Count > 0)
                {
                    var prepackdetailList = new List<prepackdetail>();
                    var skus = (from p in prePackSkuDto.PrePackSkuListDto group p by p.SkuSysId into g select g.Key).ToList();

                    var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.SysId)).ToList();
                    var packList = _crudRepository.GetAllList<pack>().ToList();
                    var uomList = _crudRepository.GetAllList<uom>().ToList();
                    foreach (var detail in prePackSkuDto.PrePackSkuListDto)
                    {
                        var detailSku = skuList.Find(x => x.SysId == detail.SkuSysId);
                        pack detailPack = null;
                        if (detailSku != null)
                        {
                            detailPack = packList.Find(x => x.SysId == detailSku.PackSysId);
                        }
                        if (detailPack == null)
                        {
                            throw new Exception("商品:" + detailSku.SkuName + ",包装代码不存在");
                        }

                        uom detailUom = null;
                        if (detailPack != null)
                        {
                            detailUom = uomList.Find(x => x.SysId == Guid.Parse(detailPack.FieldUom01.ToString()));
                        }
                        if (detailUom == null)
                        {
                            throw new Exception("包装:" + detailPack.PackCode + ",单位不存在");
                        }
                        var prepackdetail = new prepackdetail()
                        {
                            SysId = Guid.NewGuid(),
                            PrePackSysId = prepackSource.SysId,
                            SkuSysId = detail.SkuSysId,
                            UOMSysId = detailUom.SysId,
                            PackSysId = detailPack.SysId,
                            Loc = detail.Loc,
                            Lot = detail.Lot,
                            CreateBy = prePackSkuDto.CurrentUserId,
                            CreateDate = DateTime.Now,
                            UpdateBy = prePackSkuDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            PreQty = detail.PreQty,
                            Qty = detail.Qty,
                            LotAttr01 = detail.LotAttr01,
                            LotAttr02 = detail.LotAttr02,
                            LotAttr03 = detail.LotAttr03,
                            LotAttr04 = detail.LotAttr04,
                            LotAttr05 = detail.LotAttr05,
                            LotAttr06 = detail.LotAttr06,
                            LotAttr07 = detail.LotAttr07,
                            LotAttr08 = detail.LotAttr08,
                            LotAttr09 = detail.LotAttr09,
                            UpdateUserName = prePackSkuDto.CurrentDisplayName
                        };
                        if (!string.IsNullOrEmpty(detail.ProduceDateDisplay))
                            prepackdetail.ProduceDate = Convert.ToDateTime(detail.ProduceDateDisplay);
                        if (!string.IsNullOrEmpty(detail.ExpiryDateDisplay))
                            prepackdetail.ExpiryDate = Convert.ToDateTime(detail.ExpiryDateDisplay);
                        prepackdetailList.Add(prepackdetail);
                    }
                    _crudRepository.BatchInsert(prepackdetailList);
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw new Exception("更新预包装明细失败" + ex.Message);
            }
            return true;
        }
        #endregion

        #region 删除与包装

        /// <summary>
        /// 删除预包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool DeletePerPack(List<Guid> sysId,Guid warehouseSysId)
        {
            try
            {
                _crudRepository.ChangeDB(warehouseSysId);
                var list = _crudRepository.GetQuery<prepack>(x => sysId.Contains(x.SysId) && (x.OutboundSysId != null || x.OutboundOrder != null)).ToList();
                if (list != null && list.Count > 0)
                {
                    throw new Exception("已绑定出库单的预包装单不能删除");
                }
                _prePackcrudRepository.DeletePrePack(sysId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
        #endregion

        #region 预包装导入
        /// <summary>
        /// 预包装导入
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        /// <returns></returns>
        public bool ImportPrePack(PrePackSkuDto prePackSkuDto)
        {
            try
            {
                _crudRepository.ChangeDB(prePackSkuDto.WarehouseSysId);
                #region 组织预包装单
                var prePack = new prepack()
                {
                    SysId = Guid.NewGuid(),
                    BatchNumber = prePackSkuDto.BatchNumber,
                    ServiceStationName = prePackSkuDto.ServiceStationName,
                    WareHouseSysId = prePackSkuDto.WarehouseSysId,
                    //PrePackOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPrePack),
                    PrePackOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPrePack),
                    CreateBy = prePackSkuDto.CurrentUserId,
                    UpdateBy = prePackSkuDto.CurrentUserId,
                    CreateUserName = prePackSkuDto.CurrentDisplayName,
                    UpdateUserName = prePackSkuDto.CurrentDisplayName,
                    UpdateDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    Source = "WMS",
                    Status = (int)PrePackStatus.New
                };
                _crudRepository.Insert(prePack);
                #endregion

                #region 预包装单明细
                if (prePackSkuDto.PrePackSkuListDto != null && prePackSkuDto.PrePackSkuListDto.Count > 0)
                {
                    var prepackdetailList = new List<prepackdetail>();
                    var skus = (from p in prePackSkuDto.PrePackSkuListDto group p by p.OtherId into g select g.Key).ToList();

                    var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.OtherId)).ToList();
                    var packList = _crudRepository.GetAllList<pack>().ToList();
                    var uomList = _crudRepository.GetAllList<uom>().ToList();

                    foreach (var detail in prePackSkuDto.PrePackSkuListDto)
                    {
                        var detailSku = skuList.Find(x => x.OtherId == detail.OtherId && x.UPC == detail.UPC);
                        if (detailSku == null)
                        {
                            throw new Exception("商品外部Id:" + detail.OtherId + "或UPC:" + detail.UPC + "不存在");
                        }

                        pack detailPack = null;
                        if (detailSku != null)
                        {
                            detailPack = packList.Find(x => x.SysId == detailSku.PackSysId);
                        }
                        if (detailPack == null)
                        {
                            throw new Exception("商品:" + detailSku.SkuName + ",包装代码不存在");
                        }

                        uom detailUom = null;
                        if (detailPack != null)
                        {
                            detailUom = uomList.Find(x => x.SysId == Guid.Parse(detailPack.FieldUom01.ToString()));
                            if (detailUom == null)
                            {
                                throw new Exception("包装:" + detailPack.PackCode + ",单位不存在");
                            }
                        }

                        var isexist = prepackdetailList.Where(x => x.SkuSysId == detailSku.SysId).ToList();
                        if (isexist != null && isexist.Count > 0)
                        {
                            throw new Exception("外部Id:" + detail.OtherId + "或UPC:" + detail.UPC + "的商品存在重复，请检查");
                        }

                        var prepackdetail = new prepackdetail()
                        {
                            SysId = Guid.NewGuid(),
                            PrePackSysId = prePack.SysId,
                            SkuSysId = detailSku.SysId,
                            UOMSysId = detailUom.SysId,
                            PackSysId = detailPack.SysId,
                            CreateBy = prePackSkuDto.CurrentUserId,
                            UpdateBy = prePackSkuDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            PreQty = detail.PreQty,
                            CreateUserName = prePackSkuDto.CurrentDisplayName,
                            UpdateUserName = prePackSkuDto.CurrentDisplayName
                        };
                        prepackdetailList.Add(prepackdetail);
                    }
                    _crudRepository.BatchInsert(prepackdetailList);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("预包装导入失败" + ex.Message);
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 判断预包装货位是否存在
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool IsStorageLoc(PrePackQuery query)
        {
            var result = false;
            try
            {
                _crudRepository.ChangeDB(query.WarehouseSysId);
                var info = _crudRepository.GetQuery<prepack>(x => x.StorageLoc == query.StorageLoc && x.Status == (int)PrePackStatus.New && x.WareHouseSysId == query.WarehouseSysId).FirstOrDefault();
                if (info != null)
                {
                    if (query.SysId == new Guid())
                    {   //新建
                        result = true;
                    }
                    else
                    {  //编辑
                        if (info.SysId != query.SysId)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("判断预包装货位是否存在失败" + ex.Message);
            }
            return result;
        }



        /// <summary>
        /// 预包装复制
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool CopyPrePack(PrePackCopy query)
        {
            var result = false;
            try
            {
                _crudRepository.ChangeDB(query.WarehouseSysId);
                var info = _crudRepository.GetQuery<prepack>(x => x.SysId == query.SysId).FirstOrDefault();
                if (info != null)
                {
                    var prePackOrders = _baseAppService.GetNumber(PublicConst.GenNextNumberPrePack, query.CopyNumber);

                    for (int i = 0; i < query.CopyNumber; i++)
                    {
                        var model = new prepack();
                        model.SysId = Guid.NewGuid();
                        model.WareHouseSysId = info.WareHouseSysId;
                        model.Status = info.Status;
                        model.CreateBy = query.CurrentUserId;
                        model.CreateDate = DateTime.Now;
                        model.CreateUserName = query.CurrentDisplayName;
                        model.Source = "WMS";
                        model.UpdateBy = query.CurrentUserId;
                        model.UpdateDate = DateTime.Now;
                        model.UpdateUserName = query.CurrentDisplayName;
                        //model.PrePackOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPrePack);
                        model.PrePackOrder = prePackOrders[i];
                        model.prepackdetails = new List<prepackdetail>();
                        foreach (var item in info.prepackdetails)
                        {
                            var detail = new prepackdetail();
                            detail.SysId = Guid.NewGuid();
                            detail.PrePackSysId = model.SysId;
                            detail.CreateBy = query.CurrentUserId;
                            detail.CreateDate = DateTime.Now;
                            detail.CreateUserName = query.CurrentDisplayName;
                            detail.UpdateBy = query.CurrentUserId;
                            detail.UpdateDate = DateTime.Now;
                            detail.UpdateUserName = query.CurrentDisplayName;
                            detail.SkuSysId = item.SkuSysId;
                            detail.UOMSysId = item.UOMSysId;
                            detail.PackSysId = item.PackSysId;
                            detail.Loc = item.Loc;
                            detail.Lot = item.Lot;
                            detail.PreQty = item.PreQty;
                            detail.LotAttr01 = item.LotAttr01;
                            detail.LotAttr02 = item.LotAttr02;
                            detail.LotAttr03 = item.LotAttr03;
                            detail.LotAttr04 = item.LotAttr04;
                            detail.LotAttr05 = item.LotAttr05;
                            detail.LotAttr06 = item.LotAttr06;
                            detail.LotAttr07 = item.LotAttr07;
                            detail.LotAttr08 = item.LotAttr08;
                            detail.LotAttr09 = item.LotAttr09;
                            model.prepackdetails.Add(detail);
                        }
                        _crudRepository.Insert(model);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("预包装复制失败" + ex.Message);
            }
            return result;
        }

    }
}
