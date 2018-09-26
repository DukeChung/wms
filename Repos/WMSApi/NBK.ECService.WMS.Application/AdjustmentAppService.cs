using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Utility.RabbitMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class AdjustmentAppService : WMSApplicationService, IAdjustmentAppService
    {
        private IAdjustmentRepository _crudRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IPackageAppService _packageAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IBaseAppService _baseAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;

        public AdjustmentAppService(IAdjustmentRepository crudRepository, IInventoryRepository inventoryRepository, IPackageAppService packageAppService, IThirdPartyAppService thirdPartyAppService, IBaseAppService baseAppService, IWMSSqlRepository wmsSqlRepository)
        {
            this._crudRepository = crudRepository;
            _inventoryRepository = inventoryRepository;
            _packageAppService = packageAppService;
            _thirdPartyAppService = thirdPartyAppService;
            this._baseAppService = baseAppService;
            this._wmsSqlRepository = wmsSqlRepository;
        }

        public Pages<AdjustmentListDto> GetAdjustmentListByPage(AdjustmentQuery query)
        {
            _crudRepository.ChangeDB(query.WarehouseSysId);
            return _crudRepository.GetAdjustmentListByPage(query);
        }

        public AdjustmentViewDto GetAdjustmentBySysId(Guid sysid,Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var adjustment = _crudRepository.GetAdjustmentBySysId(sysid);
            if (adjustment != null)
            {
                adjustment.AdjustmentDetailList = _crudRepository.GetAdjustmentDetails(sysid, warehouseSysId);

                #region 图片文件
                if (adjustment.AdjustmentDetailList != null && adjustment.AdjustmentDetailList.Count > 0)
                {
                    foreach (var item in adjustment.AdjustmentDetailList)
                    {
                        item.PictureDtoList = new List<PictureDto>();
                        var pictures = _crudRepository.GetQuery<picture>(x => x.TableKey == PublicConst.FileAdjustmentDetail && x.TableSysId == item.SysId).ToList();
                        if (pictures != null && pictures.Count > 0)
                        {
                            foreach (var p in pictures)
                            {
                                item.PictureDtoList.Add(
                                    new PictureDto()
                                    {
                                        Name = p.Name,
                                        Url = p.Url,
                                        Size = p.Size,
                                        Suffix = p.Suffix,
                                        ShowUrl = PublicConst.httpAddress + "/" + PublicConst.Adjustment + "/" + p.Url
                                    });
                            }
                        }
                    }
                }
                #endregion
            }

            return adjustment;
        }

        public void AddAdjustment(AdjustmentDto adjustmentDto)
        {
            _crudRepository.ChangeDB(adjustmentDto.WarehouseSysId);
            if (adjustmentDto == null || adjustmentDto.AdjustmentDetailList == null)
            {
                throw new Exception("传入损益单无效,缺失主要数据源");
            }


            var skuList = adjustmentDto.AdjustmentDetailList.Select(p => p.SkuSysId).ToList();

            var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentDto.WarehouseSysId).FirstOrDefault();
            if (frozenSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == frozenSku.SkuSysId).FirstOrDefault();
                throw new Exception($"商品{sku.SkuName}已被冻结，无法创建损益单!");
            }

            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentDto.WarehouseSysId).ToList();
            if (locskuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in adjustmentDto.AdjustmentDetailList
                                        join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{sku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，无法创建损益单!");
                }
            }

            if (adjustmentDto.AdjustmentDetailList != null)
            {
                foreach (var item in adjustmentDto.AdjustmentDetailList)
                {
                    var invlotloclpn = _crudRepository.GetQuery<invlotloclpn>(x => x.WareHouseSysId == adjustmentDto.WarehouseSysId && x.Loc == item.Loc && x.Lot == item.Lot && x.Lpn == item.Lpn && x.SkuSysId == item.SkuSysId).FirstOrDefault();
                    if (invlotloclpn != null)
                    {
                        var currentQty = CommonBussinessMethod.GetAvailableQty(invlotloclpn.Qty, invlotloclpn.AllocatedQty, invlotloclpn.PickedQty, invlotloclpn.FrozenQty);
                        int transQty = 0;
                        pack transPack = new pack();
                        if (_packageAppService.GetSkuConversiontransQty(item.SkuSysId, item.Qty, out transQty, ref transPack) == false)
                        {
                            transQty = Convert.ToInt32(item.Qty);
                        }
                        if (currentQty + transQty < 0)
                        {
                            throw new Exception($"{item.SkuName} 库存不足,无法创建损益单!");
                        }
                    }
                }
            }  

            adjustmentDto.SysId = Guid.NewGuid();
            adjustmentDto.Status = (int)AdjustmentStatus.New;

            adjustmentDto.CreateDate = DateTime.Now;
            adjustmentDto.UpdateDate = DateTime.Now;

            adjustmentDto.AdjustmentDetailList.ForEach(p =>
            {
                //从盘点创建的损益单，不做单位转换（PS: 盘点那边已经做过了）
                if (adjustmentDto.SourceType != PublicConst.AJSourceTypeStockTake)
                {
                    //gavin : 单位转换
                    int transQty = 0;
                    pack transPack = new pack();
                    if (_packageAppService.GetSkuConversiontransQty(p.SkuSysId, p.Qty, out transQty, ref transPack) == false)
                    {
                        transQty = Convert.ToInt32(p.Qty);
                    }
                    p.Qty = transQty;
                }

                if (string.IsNullOrEmpty(p.AdjustlevelCode))
                {
                    p.AdjustlevelCode = PublicConst.AdjustmentLevelCodeDefault;
                }
                p.SysId = Guid.NewGuid();
                p.AdjustmentSysId = adjustmentDto.SysId.Value;
                p.CreateDate = DateTime.Now;
                p.UpdateDate = DateTime.Now;

                #region 图片文件
                if (p.PictureDtoList != null && p.PictureDtoList.Count > 0)
                {
                    foreach (var item in p.PictureDtoList)
                    {
                        var picture = new picture()
                        {
                            SysId = Guid.NewGuid(),
                            TableKey = PublicConst.FileAdjustmentDetail,
                            TableSysId = p.SysId,
                            Name = item.Name,
                            Url = item.Url,
                            Size = item.Size,
                            Suffix = item.Suffix,
                            CreateBy = adjustmentDto.UpdateBy,
                            CreateDate = DateTime.Now,
                            UpdateBy = adjustmentDto.UpdateBy,
                            UpdateDate = DateTime.Now
                        };
                        _crudRepository.Insert(picture);
                    }
                }
                #endregion
            });

            //adjustmentDto.AdjustmentOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberAdjustment);
            adjustmentDto.AdjustmentOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberAdjustment);

            var adjustment = adjustmentDto.JTransformTo<adjustment>();

            adjustment.adjustmentdetails = adjustmentDto.AdjustmentDetailList.JTransformTo<adjustmentdetail>();

            _crudRepository.Insert(adjustment);

        }

        public void UpdateAdjustment(AdjustmentDto adjustmentDto)
        {
            _crudRepository.ChangeDB(adjustmentDto.WarehouseSysId);
            if (adjustmentDto == null || adjustmentDto.AdjustmentDetailList == null)
            {
                throw new Exception("传入损益单无效,缺失主要数据源");
            }

            var adjustmentSource = _crudRepository.FirstOrDefault<adjustment>(p => p.SysId == adjustmentDto.SysId);
            if (adjustmentSource == null)
            {
                throw new Exception("请求的损益单不存在！");
            } 

            var skuList = adjustmentDto.AdjustmentDetailList.Select(p => p.SkuSysId).ToList();

            var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentDto.WarehouseSysId).FirstOrDefault();
            if (frozenSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == frozenSku.SkuSysId).FirstOrDefault();
                throw new Exception($"商品{sku.SkuName}已被冻结，无法保存损益单!");
            }

            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentDto.WarehouseSysId).ToList();
            if (locskuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in adjustmentDto.AdjustmentDetailList
                                        join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{sku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，无法保存损益单!");
                }
            }

            if (adjustmentDto.AdjustmentDetailList!=null)
            {
                foreach (var item in adjustmentDto.AdjustmentDetailList)
                {
                    var invlotloclpn = _crudRepository.GetQuery<invlotloclpn>(x => x.WareHouseSysId == adjustmentDto.WarehouseSysId && x.Loc == item.Loc && x.Lot == item.Lot && x.Lpn == item.Lpn && x.SkuSysId == item.SkuSysId).FirstOrDefault();
                    if (invlotloclpn != null)
                    {
                        var currentQty = CommonBussinessMethod.GetAvailableQty(invlotloclpn.Qty, invlotloclpn.AllocatedQty, invlotloclpn.PickedQty, invlotloclpn.FrozenQty);
                        int transQty = 0;
                        pack transPack = new pack();
                        if (_packageAppService.GetSkuConversiontransQty(item.SkuSysId, item.DisplayQty, out transQty, ref transPack) == false)
                        {
                            transQty = Convert.ToInt32(item.DisplayQty);
                        }
                        if (currentQty + transQty < 0)
                        {
                            throw new Exception($"{item.SkuName} 库存不足,无法保存损益单!");
                        }
                    }
                } 
            } 


            adjustmentSource.UpdateBy = adjustmentDto.UpdateBy;
            adjustmentSource.UpdateDate = DateTime.Now;
            adjustmentSource.UpdateUserName = adjustmentDto.UpdateUserName;

            adjustmentSource.adjustmentdetails.ToList().ForEach(p =>
            {
                var adjustmentDetail = adjustmentDto.AdjustmentDetailList.FirstOrDefault(q =>
                    q.SkuSysId == p.SkuSysId
                    && q.Lot == p.Lot
                    && q.Loc == p.Loc && q.SysId == p.SysId);
                if (adjustmentDetail != null)
                {
                    //gavin : 单位转换
                    int transQty = 0;
                    pack transPack = new pack();
                    if (_packageAppService.GetSkuConversiontransQty(adjustmentDetail.SkuSysId, adjustmentDetail.DisplayQty, out transQty, ref transPack) == false)
                    {
                        transQty = Convert.ToInt32(adjustmentDetail.DisplayQty);
                    }

                    p.Qty = transQty;
                    p.AdjustlevelCode = adjustmentDetail.AdjustlevelCode;
                    p.Remark = adjustmentDetail.Remark;
                    p.UpdateBy = adjustmentDetail.UpdateBy;
                    p.UpdateDate = DateTime.Now;
                    p.UpdateUserName = adjustmentDetail.UpdateUserName;

                    #region 图片文件
                    _crudRepository.Delete<picture>(x => x.TableKey == PublicConst.FileAdjustmentDetail && x.TableSysId == adjustmentDetail.SysId);

                    if (adjustmentDetail.PictureDtoList != null && adjustmentDetail.PictureDtoList.Count > 0)
                    {
                        foreach (var item in adjustmentDetail.PictureDtoList)
                        {
                            var picture = new picture()
                            {
                                SysId = Guid.NewGuid(),
                                TableKey = PublicConst.FileAdjustmentDetail,
                                TableSysId = adjustmentDetail.SysId,
                                Name = item.Name,
                                Url = item.Url,
                                Size = item.Size,
                                Suffix = item.Suffix,
                                CreateBy = adjustmentDto.UpdateBy,
                                CreateDate = DateTime.Now,
                                UpdateBy = adjustmentDto.UpdateBy,
                                UpdateDate = DateTime.Now
                            };
                            _crudRepository.Insert(picture);
                        }
                    }
                    #endregion
                }
            });

            _crudRepository.Update(adjustmentSource);

        }

        public void DeleteAjustmentSkus(List<Guid> request,Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            _crudRepository.Delete<adjustmentdetail>(request);
        }

        public Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery)
        {
            _crudRepository.ChangeDB(skuQuery.WarehouseSysId);
            return _crudRepository.GetSkuInventoryList(skuQuery);
        }

        public void Audit(AdjustmentAuditDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var businessLogDto = new BusinessLogDto();
            try
            {
                businessLogDto.doc_sysId = dto.SysId;
                businessLogDto.doc_order = "损益单";
                businessLogDto.access_log_sysId = this.AccessLogSysId.HasValue ? this.AccessLogSysId.Value : Guid.NewGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(dto);
                businessLogDto.user_id = dto.CurrentUserId.ToString();
                businessLogDto.user_name = dto.CurrentDisplayName;
                businessLogDto.business_name = BusinessName.Losses.ToDescription();
                businessLogDto.business_type = BusinessType.Adjustment.ToDescription();
                businessLogDto.business_operation = PublicConst.AuditAdjustment;
                businessLogDto.descr = "[old_json记录 损益记录 , new_json记录 交接数据 记录]";
                businessLogDto.flag = true;

                var adjustmentSource = _crudRepository.FirstOrDefault<adjustment>(p => p.SysId == dto.SysId);
                if (adjustmentSource == null)
                {
                    throw new Exception("请求的损益单不存在！");
                }
                businessLogDto.doc_order += adjustmentSource.AdjustmentOrder;
                if (adjustmentSource.Status != (int)AdjustmentStatus.New)
                {
                    throw new Exception("只有新建的损益单可以审核，请检查！");
                }

                //提前校验损益  冻结
                //货位级别
                var locs = adjustmentSource.adjustmentdetails.Select(p => p.Loc).ToList();
                var locations = _crudRepository.GetQuery<location>(p => locs.Contains(p.Loc) && p.Status == (int)LocationStatus.Frozen && p.WarehouseSysId == adjustmentSource.WareHouseSysId).ToList();

                if (locations.Count() > 0)
                {
                    throw new Exception($"货位{locations.First().Loc}已被冻结，不能做损益!");
                }

                //商品级别
                var skuList = adjustmentSource.adjustmentdetails.Select(p => p.SkuSysId.Value).ToList();
                var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value) 
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentSource.WareHouseSysId);
                if (frozenSkuList.Count() > 0)
                {
                    var skuSysId = frozenSkuList.First().SkuSysId;
                    var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能做损益!");
                }

                //货位商品级别
                var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == adjustmentSource.WareHouseSysId).ToList();

                if (locskuList.Count > 0)
                {
                    var locskuFrozenQuery = from T1 in adjustmentSource.adjustmentdetails
                                            join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { T2.SkuSysId, T2.Loc }
                                            select T2;

                    if (locskuFrozenQuery.Count() > 0)
                    {
                        var firstFrozenLocsku = locskuFrozenQuery.First();
                        var skuSysId = firstFrozenLocsku.SkuSysId;
                        var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                        throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能做损益!");
                    }
                }
                

                adjustmentSource.Status = (int)AdjustmentStatus.Audit;
                adjustmentSource.AuditingBy = dto.AuditingBy.ToString();
                adjustmentSource.AuditingDate = DateTime.Now;
                adjustmentSource.AuditingName = dto.AuditingName;
                adjustmentSource.UpdateBy = dto.AuditingBy;
                adjustmentSource.UpdateDate = DateTime.Now;
                adjustmentSource.UpdateUserName = dto.AuditingName;

                var list = new List<ThirdPartAdjustmentDetailListDto>();
                var updateInventoryList = new List<UpdateInventoryDto>();
                var invtranList = new List<invtran>();

                foreach (var adjustmentdetail in adjustmentSource.adjustmentdetails)
                {
                    int updateQty = adjustmentdetail.Qty;

                    if (adjustmentSource.SourceType == PublicConst.AJSourceTypeStockTake || adjustmentSource.SourceType == PublicConst.AJSourceTypeQC)
                    {
                        var outList = new List<ThirdPartAdjustmentDetailListDto>();
                        //处理盘点创建的损益单库存
                        AuditStockTakeAdjustmentInventory(adjustmentSource, adjustmentdetail, dto, updateInventoryList, invtranList, out outList);
                         
                        //盘点损益
                        list.AddRange(outList) ;
                    }
                    else
                    {
                        //处理正常PC端直接创建的损益单库存
                        AuditNormalAdjustmentInventory(adjustmentSource, adjustmentdetail, dto, updateInventoryList, invtranList);
                    }
                }

                _wmsSqlRepository.UpdateInventoryAdjustmentAudit(updateInventoryList);
                _wmsSqlRepository.BatchInsertInvTrans(invtranList);

                list = list.GroupBy(x => new { x.Lot, x.SkuSysId }).Select(p => new ThirdPartAdjustmentDetailListDto { Lot = p.Key.Lot, SkuSysId = p.Key.SkuSysId, Qty = p.Sum(i => i.Qty) }).ToList();
                businessLogDto.old_json = JsonConvert.SerializeObject(adjustmentSource.adjustmentdetails);
                _crudRepository.Update(adjustmentSource);

                var infoList = new List<adjustmentdetail>();
                //重新组织损益单明显数据
                foreach (var item in adjustmentSource.adjustmentdetails)
                {
                    if (string.IsNullOrEmpty(item.Lot))
                    {
                        var lot = list.Where(x => x.SkuSysId == item.SkuSysId).ToList();
                        if (lot != null)
                        {
                            foreach (var lotitem in lot)
                            {
                                item.Qty = lotitem.Qty;
                                item.Lot = lotitem.Lot;
                                infoList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        infoList.Add(item);
                    }
                }
                //重新组织损益单明细数据
                adjustmentSource.adjustmentdetails = infoList;

                var rsp = _thirdPartyAppService.InsertAdjustment(adjustmentSource);

                foreach (var item in adjustmentSource.adjustmentdetails)
                {
                    if (adjustmentSource.SourceType == PublicConst.AJSourceTypeStockTake || adjustmentSource.SourceType == PublicConst.AJSourceTypeQC)
                    {
                        item.Lot = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {
                //发送MQ
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }
        }

        /// <summary>
        /// 处理正常PC端直接创建的损益单
        /// </summary>
        /// <param name="adjustmentdetail"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="updateQty"></param>
        public void AuditNormalAdjustmentInventory(adjustment adjustmentSource, adjustmentdetail adjustmentdetail, AdjustmentAuditDto request, List<UpdateInventoryDto> updateInventoryList, List<invtran> invtranList)
        {
            int updateQty = adjustmentdetail.Qty;
            int auditingBy = request.AuditingBy;
            string auditingName = request.AuditingName;

            //var loDto = _inventoryRepository.GetlotloclpnDto(adjustmentdetail.SkuSysId.Value, adjustmentdetail.Lot, adjustmentdetail.Loc, adjustmentdetail.Lpn, adjustmentSource.WareHouseSysId).FirstOrDefault();

            //if (loDto == null)
            //{
            //    throw new Exception("有商品库存信息不存在(库位批次)，请检查！");
            //}

            //var skuInvlotloclpn = _crudRepository.Get<invlotloclpn>(loDto.InvLotLocLpnSysId);

            //if (skuInvlotloclpn == null)
            //{
            //    throw new Exception("有商品库存信息不存在(库位批次)，请检查！");
            //}

            //if ((CommonBussinessMethod.GetAvailableQty(skuInvlotloclpn.Qty, skuInvlotloclpn.AllocatedQty, skuInvlotloclpn.PickedQty, skuInvlotloclpn.FrozenQty) + updateQty) < 0)
            //{
            //    throw new Exception("库位批次库存数据修改后不能为负数，请检查！");
            //}

            //skuInvlotloclpn.Qty += updateQty;
            //_crudRepository.Update(skuInvlotloclpn);

            //var skuInvlot = _crudRepository.Get<invlot>(loDto.InvLotSysId);

            //if (skuInvlot == null)
            //{
            //    throw new Exception("有商品库存信息不存在(批次)，请检查！");
            //}

            //if ((CommonBussinessMethod.GetAvailableQty(skuInvlot.Qty, skuInvlot.AllocatedQty, skuInvlot.PickedQty, skuInvlot.FrozenQty) + updateQty) < 0)
            //{
            //    throw new Exception("批次库存修改后不能为负数，请检查！");
            //}

            //skuInvlot.Qty += updateQty;
            //_crudRepository.Update(skuInvlot);

            //var skuinvskuloc = _crudRepository.Get<invskuloc>(loDto.InvSkuLocSysId);

            //if (skuinvskuloc == null)
            //{
            //    throw new Exception("有商品库存信息不存在(库位)，请检查！");
            //}

            //if ((CommonBussinessMethod.GetAvailableQty(skuinvskuloc.Qty, skuinvskuloc.AllocatedQty, skuinvskuloc.PickedQty, skuinvskuloc.FrozenQty) + updateQty) < 0)
            //{
            //    throw new Exception("库位库存修改后不能为负数，请检查！");
            //}

            //skuinvskuloc.Qty += updateQty;
            //_crudRepository.Update(skuinvskuloc);

            sku skuInfo = _crudRepository.FirstOrDefault<sku>(p => p.SysId == adjustmentdetail.SkuSysId.Value);

            var invLotLocLpn = _crudRepository.GetQuery<invlotloclpn>(p => p.SkuSysId == adjustmentdetail.SkuSysId.Value 
                && p.WareHouseSysId == adjustmentSource.WareHouseSysId && p.Lot == adjustmentdetail.Lot 
                && p.Loc == adjustmentdetail.Loc && p.Lpn == adjustmentdetail.Lpn).FirstOrDefault();
            if (invLotLocLpn == null)
            {
                throw new Exception(string.Format("商品 [{0}] 在InvLotLocLpn没有库存信息", skuInfo.SkuName));
            }
            int invLotLocLpnAvailableQty = CommonBussinessMethod.GetAvailableQty(invLotLocLpn.Qty, invLotLocLpn.AllocatedQty, invLotLocLpn.PickedQty, invLotLocLpn.FrozenQty);
            if (invLotLocLpnAvailableQty + updateQty < 0)
            {
                throw new Exception("库位批次库存数据修改后不能为负数，请检查！");
            }
            updateInventoryList.Add(new UpdateInventoryDto
            {
                InvLotLocLpnSysId = invLotLocLpn.SysId,
                InvLotSysId = null,
                InvSkuLocSysId = null,
                Qty = updateQty,
                CurrentUserId = auditingBy,
                CurrentDisplayName = auditingName,
                WarehouseSysId = adjustmentSource.WareHouseSysId
            });

            var invLot = _crudRepository.GetQuery<invlot>(p => p.SkuSysId == adjustmentdetail.SkuSysId.Value
                && p.WareHouseSysId == adjustmentSource.WareHouseSysId && p.Lot == adjustmentdetail.Lot).FirstOrDefault();
            if (invLot == null)
            {
                throw new Exception(string.Format("商品 [{0}] 在InvLot没有库存信息", skuInfo.SkuName));
            }
            int invLotAvailableQty = CommonBussinessMethod.GetAvailableQty(invLot.Qty, invLot.AllocatedQty, invLot.PickedQty, invLot.FrozenQty);
            if (invLotAvailableQty + updateQty < 0)
            {
                throw new Exception("批次库存修改后不能为负数，请检查！");
            }
            updateInventoryList.Add(new UpdateInventoryDto
            {
                InvLotLocLpnSysId = null,
                InvLotSysId = invLot.SysId,
                InvSkuLocSysId = null,
                Qty = updateQty,
                CurrentUserId = auditingBy,
                CurrentDisplayName = auditingName,
                WarehouseSysId = adjustmentSource.WareHouseSysId
            });

            var invLoc = _crudRepository.GetQuery<invskuloc>(p => p.SkuSysId == adjustmentdetail.SkuSysId.Value
                && p.WareHouseSysId == adjustmentSource.WareHouseSysId && p.Loc == adjustmentdetail.Loc).FirstOrDefault();
            if (invLoc == null)
            {
                throw new Exception(string.Format("商品 [{0}] 在InvLoc没有库存信息", skuInfo.SkuName));
            }
            int invLocAvailableQty = CommonBussinessMethod.GetAvailableQty(invLoc.Qty, invLoc.AllocatedQty, invLoc.PickedQty, invLoc.FrozenQty);
            if (invLocAvailableQty + updateQty < 0)
            {
                throw new Exception("库位库存修改后不能为负数，请检查！");
            }
            updateInventoryList.Add(new UpdateInventoryDto
            {
                InvLotLocLpnSysId = null,
                InvLotSysId = null,
                InvSkuLocSysId = invLoc.SysId,
                Qty = updateQty,
                CurrentUserId = auditingBy,
                CurrentDisplayName = auditingName,
                WarehouseSysId = adjustmentSource.WareHouseSysId
            });

            invtran invtranInfo = new invtran()
            {
                SysId = Guid.NewGuid(),
                DocOrder = adjustmentSource.AdjustmentOrder,
                DocSysId = adjustmentSource.SysId,
                DocDetailSysId = adjustmentdetail.SysId,
                WareHouseSysId = adjustmentSource.WareHouseSysId,
                SkuSysId = adjustmentdetail.SkuSysId.Value,
                SkuCode = skuInfo.SkuCode,
                TransType = InvTransType.Adjustment,
                SourceTransType = InvSourceTransType.Losses,
                Qty = updateQty,
                Loc = adjustmentdetail.Loc,
                Lot = adjustmentdetail.Lot,
                Lpn = adjustmentdetail.Lpn,
                Status = InvTransStatus.Ok,
                CreateBy = auditingBy,
                CreateDate = DateTime.Now,
                UpdateBy = auditingBy,
                UpdateDate = DateTime.Now
            };
            invtranList.Add(invtranInfo);
            //_crudRepository.Insert(invtranInfo);
        }

        public void AuditStockTakeAdjustmentInventory(adjustment adjustmentSource, adjustmentdetail adjustmentdetail, AdjustmentAuditDto request, List<UpdateInventoryDto> updateInventoryList, List<invtran> invtranList, out List<ThirdPartAdjustmentDetailListDto> outList)
        {
            int updateQty = adjustmentdetail.Qty;
            int auditingBy = request.AuditingBy;
            string auditingName = request.AuditingName;

            List<Guid> invlotloclpnSysIdList = null;
            if (adjustmentSource.SourceType == PublicConst.AJSourceTypeStockTake)
            {
                invlotloclpnSysIdList = _inventoryRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == adjustmentdetail.SkuSysId.Value && x.Loc == adjustmentdetail.Loc && x.WareHouseSysId == adjustmentSource.WareHouseSysId).Select(x => x.SysId).ToList();
            }
            else
            {
                invlotloclpnSysIdList = _inventoryRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == adjustmentdetail.SkuSysId.Value && x.WareHouseSysId == adjustmentSource.WareHouseSysId).Select(x => x.SysId).ToList();
                if (invlotloclpnSysIdList == null || invlotloclpnSysIdList.Count == 0)
                {
                    throw new Exception("库存不足！");
                }
            }

            sku skuInfo = _crudRepository.FirstOrDefault<sku>(p => p.SysId == adjustmentdetail.SkuSysId.Value);

            outList = new List<ThirdPartAdjustmentDetailListDto>();

            if (invlotloclpnSysIdList != null && invlotloclpnSysIdList.Count > 0)
            {
                foreach (var invlotloclpnSysId in invlotloclpnSysIdList)
                {
                    int tempQty = 0;
                    var skuInvlotloclpn = _crudRepository.Get<invlotloclpn>(invlotloclpnSysId);
                    adjustmentdetail.Lot = skuInvlotloclpn.Lot;

                    //由于盘点损益是没有批次的，因此需要循环每一个批次货位的库存去看库存数量是否能消耗点盘点损益的数量
                    //增加库存
                    if (updateQty > 0)
                    {
                        tempQty = updateQty;
                    }
                    //扣减库存
                    else
                    {
                        int availableQty = CommonBussinessMethod.GetAvailableQty(skuInvlotloclpn.Qty, skuInvlotloclpn.AllocatedQty, skuInvlotloclpn.PickedQty, skuInvlotloclpn.FrozenQty);
                        if (availableQty == 0 || updateQty == 0) continue;
                        tempQty = (availableQty) >= Math.Abs(updateQty) ? updateQty : -availableQty;
                    }
                    updateQty = updateQty - tempQty;

                    var outInfo = new ThirdPartAdjustmentDetailListDto();
                    outInfo.Qty = tempQty;
                    outInfo.Lot = skuInvlotloclpn.Lot;
                    outInfo.SkuSysId = skuInvlotloclpn.SkuSysId;
                    outList.Add(outInfo);

                    updateInventoryList.Add(new UpdateInventoryDto
                    {
                        InvLotLocLpnSysId = invlotloclpnSysId,
                        InvLotSysId = null,
                        InvSkuLocSysId = null,
                        Qty = tempQty,
                        CurrentUserId = auditingBy,
                        CurrentDisplayName = auditingName,
                        WarehouseSysId = adjustmentSource.WareHouseSysId
                    });

                    //skuInvlotloclpn.Qty += tempQty;
                    //_crudRepository.Update(skuInvlotloclpn);

                    //var lotSysId = _inventoryRepository.Getinvlot(adjustmentdetail.SkuSysId.Value, adjustmentSource.WareHouseSysId, skuInvlotloclpn.Lot);
                    //var skuInvlot = _crudRepository.Get<invlot>(lotSysId);
                    //if ((CommonBussinessMethod.GetAvailableQty(skuInvlot.Qty, skuInvlot.AllocatedQty, skuInvlot.PickedQty, skuInvlot.FrozenQty) + tempQty) < 0)
                    //{
                    //    throw new Exception("批次库存修改后不能为负数，请检查！");
                    //}
                    //skuInvlot.Qty += tempQty;
                    //_crudRepository.Update(skuInvlot);
                    var skuInvlot = _crudRepository.GetQuery<invlot>(p => p.SkuSysId == adjustmentdetail.SkuSysId.Value && p.WareHouseSysId == adjustmentSource.WareHouseSysId && p.Lot == skuInvlotloclpn.Lot).FirstOrDefault();
                    if (skuInvlot == null)
                    {
                        throw new Exception(string.Format("商品 [{0}] 在InvLot没有库存信息", skuInfo.SkuName));
                    }
                    int invLotAvailableQty = CommonBussinessMethod.GetAvailableQty(skuInvlot.Qty, skuInvlot.AllocatedQty, skuInvlot.PickedQty, skuInvlot.FrozenQty);
                    if (invLotAvailableQty + tempQty < 0)
                    {
                        throw new Exception("批次库存修改后不能为负数，请检查！");
                    }
                    updateInventoryList.Add(new UpdateInventoryDto
                    {
                        InvLotLocLpnSysId = null,
                        InvLotSysId = skuInvlot.SysId,
                        InvSkuLocSysId = null,
                        Qty = tempQty,
                        CurrentUserId = auditingBy,
                        CurrentDisplayName = auditingName,
                        WarehouseSysId = adjustmentSource.WareHouseSysId
                    });


                    //var locSysId = _inventoryRepository.Getinvskuloc(adjustmentdetail.SkuSysId.Value, adjustmentSource.WareHouseSysId, adjustmentdetail.Loc);
                    //var locSysId = _inventoryRepository.Getinvskuloc(adjustmentdetail.SkuSysId.Value, adjustmentSource.WareHouseSysId, skuInvlotloclpn.Loc);
                    //var skuInvloc = _crudRepository.Get<invskuloc>(locSysId);
                    //if ((CommonBussinessMethod.GetAvailableQty(skuInvloc.Qty, skuInvloc.AllocatedQty, skuInvloc.PickedQty, skuInvloc.FrozenQty) + tempQty) < 0)
                    //{
                    //    throw new Exception("库位库存修改后不能为负数，请检查！");
                    //}
                    //skuInvloc.Qty += tempQty;
                    //_crudRepository.Update(skuInvloc);
                    var skuInvloc = _crudRepository.GetQuery<invskuloc>(p => p.SkuSysId == adjustmentdetail.SkuSysId.Value && p.WareHouseSysId == adjustmentSource.WareHouseSysId && p.Loc == skuInvlotloclpn.Loc).FirstOrDefault();
                    if (skuInvloc == null)
                    {
                        throw new Exception(string.Format("商品 [{0}] 在InvSkuLoc没有库存信息", skuInfo.SkuName));
                    }
                    int invLocAvailableQty = CommonBussinessMethod.GetAvailableQty(skuInvloc.Qty, skuInvloc.AllocatedQty, skuInvloc.PickedQty, skuInvloc.FrozenQty);
                    if (invLocAvailableQty + tempQty < 0)
                    {
                        throw new Exception("库位库存修改后不能为负数，请检查！");
                    }
                    updateInventoryList.Add(new UpdateInventoryDto
                    {
                        InvLotLocLpnSysId = null,
                        InvLotSysId = null,
                        InvSkuLocSysId = skuInvloc.SysId,
                        Qty = tempQty,
                        CurrentUserId = auditingBy,
                        CurrentDisplayName = auditingName,
                        WarehouseSysId = adjustmentSource.WareHouseSysId
                    });

                    invtran invtranInfo = new invtran()
                    {
                        SysId = Guid.NewGuid(),
                        DocOrder = adjustmentSource.AdjustmentOrder,
                        DocSysId = adjustmentSource.SysId,
                        DocDetailSysId = adjustmentdetail.SysId,
                        WareHouseSysId = adjustmentSource.WareHouseSysId,
                        SkuSysId = adjustmentdetail.SkuSysId.Value,
                        SkuCode = skuInfo.SkuCode,
                        TransType = InvTransType.Adjustment,
                        SourceTransType = InvSourceTransType.Losses,
                        Qty = tempQty,
                        //Loc = adjustmentdetail.Loc,
                        //Lot = adjustmentdetail.Lot,
                        Loc = skuInvlotloclpn.Loc,
                        Lot = skuInvlotloclpn.Lot,
                        Lpn = string.Empty,
                        Status = InvTransStatus.Ok,
                        CreateBy = auditingBy,
                        CreateDate = DateTime.Now,
                        UpdateBy = auditingBy,
                        UpdateDate = DateTime.Now
                    };
                    invtranList.Add(invtranInfo);
                    //_crudRepository.Insert(invtranInfo);

                    if (updateQty == 0)
                    {
                        //库存全部消耗完成，退出方法
                        return;
                    }
                }

                if (updateQty != 0)
                {
                    //循环结束，损益库存仍未被扣减完，说明库存不足
                    throw new Exception($" '{skuInfo.SkuName}' 在 {adjustmentdetail.Loc} 上库存不足，请检查！");
                }

            }
            else
            {
                #region 此分支盘点业务暂时不会进入
                var lot = _inventoryRepository.Getinvlotlist(adjustmentdetail.SkuSysId.Value, adjustmentSource.WareHouseSysId).FirstOrDefault();
                if (lot != null)
                {
                    //回写损益单批次
                    adjustmentdetail.Lot = lot.Lot;

                    var invlotloclpn = new invlotloclpn()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = adjustmentSource.WareHouseSysId,
                        SkuSysId = adjustmentdetail.SkuSysId.Value,
                        Loc = adjustmentdetail.Loc,
                        Lot = lot.Lot,
                        Lpn = string.Empty,
                        Qty = updateQty,
                        AllocatedQty = 0,
                        PickedQty = 0,
                        Status = 1,
                        CreateBy = 999,
                        CreateDate = DateTime.Now,
                        CreateUserName = "System",
                        UpdateBy = 999,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = "System"
                    };
                    _crudRepository.Insert(invlotloclpn);

                    lot.Qty += updateQty;
                    _crudRepository.Update(lot);

                    var invskuloc = new invskuloc()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = adjustmentSource.WareHouseSysId,
                        SkuSysId = adjustmentdetail.SkuSysId.Value,
                        Loc = adjustmentdetail.Loc,
                        Qty = updateQty,
                        AllocatedQty = 0,
                        PickedQty = 0,
                        CreateBy = 999,
                        CreateDate = DateTime.Now,
                        CreateUserName = "System",
                        UpdateBy = 999,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = "System"
                    };

                    _crudRepository.Insert(invskuloc);

                    invtran invtranInfo = new invtran()
                    {
                        SysId = Guid.NewGuid(),
                        DocOrder = adjustmentSource.AdjustmentOrder,
                        DocSysId = adjustmentSource.SysId,
                        DocDetailSysId = adjustmentdetail.SysId,
                        WareHouseSysId = adjustmentSource.WareHouseSysId,
                        SkuSysId = adjustmentdetail.SkuSysId.Value,
                        SkuCode = skuInfo.SkuCode,
                        TransType = InvTransType.Adjustment,
                        SourceTransType = InvSourceTransType.Losses,
                        Qty = updateQty,
                        Loc = adjustmentdetail.Loc,
                        Lot = adjustmentdetail.Lot,
                        Lpn = string.Empty,
                        Status = InvTransStatus.Ok,
                        CreateBy = auditingBy,
                        CreateDate = DateTime.Now,
                        UpdateBy = auditingBy,
                        UpdateDate = DateTime.Now
                    };

                    _crudRepository.Insert(invtranInfo);
                }
                #endregion
            }
        }

        public void Void(AdjustmentAuditDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var adjustmentSource = _crudRepository.FirstOrDefault<adjustment>(p => p.SysId == dto.SysId);
            if (adjustmentSource == null)
            {
                throw new Exception("请求的损益单不存在！");
            }

            if (adjustmentSource.Status != (int)AdjustmentStatus.New)
            {
                throw new Exception("只有新建的损益单可以作废，请检查！");
            }

            adjustmentSource.Status = (int)AdjustmentStatus.Void;
            adjustmentSource.AuditingBy = dto.AuditingBy.ToString();
            adjustmentSource.AuditingDate = DateTime.Now;
            adjustmentSource.AuditingName = dto.AuditingName;
            adjustmentSource.UpdateBy = dto.AuditingBy;
            adjustmentSource.UpdateDate = DateTime.Now;
            adjustmentSource.UpdateUserName = dto.AuditingName;

            _crudRepository.Update(adjustmentSource);
        }
    }
}
