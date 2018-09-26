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
    public class SkuBorrowAppService : WMSApplicationService, ISkuBorrowAppService
    {
        private ICrudRepository _crudRepository = null;
        private ISkuBorrowRepository _skuborrowcrudRepository = null;
        private IBaseAppService _baseAppService = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IPackageAppService _packageAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        public SkuBorrowAppService(ICrudRepository crudRepository, ISkuBorrowRepository skuborrowcrudRepository, IBaseAppService baseAppService, IWMSSqlRepository wmsSqlRepository, IPackageAppService packageAppService, IThirdPartyAppService thirdPartyAppService)
        {
            this._crudRepository = crudRepository;
            this._skuborrowcrudRepository = skuborrowcrudRepository;
            this._baseAppService = baseAppService;
            this._WMSSqlRepository = wmsSqlRepository;
            this._packageAppService = packageAppService;
            this._thirdPartyAppService = thirdPartyAppService;
        }

        public Pages<SkuBorrowListDto> GetSkuBorrowListByPage(SkuBorrowQuery query)
        {
            _crudRepository.ChangeDB(query.WarehouseSysId);
            return _skuborrowcrudRepository.GetSkuBorrowListByPage(query);
        }

        public Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery)
        {
            _crudRepository.ChangeDB(skuQuery.WarehouseSysId);
            return _skuborrowcrudRepository.GetSkuInventoryList(skuQuery);
        }

        public SkuBorrowViewDto GetSkuBorrowBySysId(Guid SysId, Guid WareHouseSysId)
        {
            _crudRepository.ChangeDB(WareHouseSysId);
            var skuborrow = _skuborrowcrudRepository.GetSkuBorrowBySysId(SysId);
            if (skuborrow != null)
            {
                skuborrow.SkuBorrowDetailList = _skuborrowcrudRepository.GetSkuBorrowDetails(SysId);

                #region 图片文件
                if (skuborrow.SkuBorrowDetailList != null && skuborrow.SkuBorrowDetailList.Count > 0)
                {
                    foreach (var item in skuborrow.SkuBorrowDetailList)
                    {
                        item.PictureDtoList = new List<PictureDto>();
                        var pictures = _crudRepository.GetQuery<picture>(x => x.TableKey == PublicConst.FileSkuBorrowDetail && x.TableSysId == item.SysId).ToList();
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
                                        ShowUrl = PublicConst.httpAddress + "/" + PublicConst.SkuBorrow + "/" + p.Url
                                    });
                            }
                        }
                    }
                }
                #endregion
            }

            return skuborrow;
        }

        public SkuBorrowViewDto GetSkuBorrowByOrder(string BorrowOrder, Guid WareHouseSysId)
        {
            _crudRepository.ChangeDB(WareHouseSysId);
            var skuborrow = _skuborrowcrudRepository.GetSkuBorrowByOrder(BorrowOrder);
            if (skuborrow != null)
            {
                skuborrow.SkuBorrowDetailList = _skuborrowcrudRepository.GetSkuBorrowDetails(skuborrow.SysId);
            }

            return skuborrow;
        }

        public void AddSkuBorrow(SkuBorrowDto skuBorrowDto)
        {
            if (skuBorrowDto == null || skuBorrowDto.SkuBorrowDetailList == null)
            {
                throw new Exception("传入商品外借单无效,缺失主要数据源");
            }
            if (!skuBorrowDto.WareHouseSysId.HasValue)
            {
                throw new Exception("请选择仓库");
            }
            _crudRepository.ChangeDB(Guid.Parse(skuBorrowDto.WareHouseSysId.ToString()));
            var skuList = skuBorrowDto.SkuBorrowDetailList.Select(p => p.SkuSysId).ToList();

            var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == skuBorrowDto.WareHouseSysId).FirstOrDefault();
            if (frozenSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == frozenSku.SkuSysId).FirstOrDefault();
                throw new Exception($"商品{sku.SkuName}已被冻结，无法完成商品外借!");
            }

            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == skuBorrowDto.WareHouseSysId).ToList();
            if (locskuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in skuBorrowDto.SkuBorrowDetailList
                                        join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{sku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，无法完成商品外借!");
                }
            }

            #region 判断库存 不满足则保存失败 

            foreach (var item in skuBorrowDto.SkuBorrowDetailList)
            {
                //校验库存冻结
                var location = _crudRepository.GetQuery<location>(p => p.Loc.ToUpper() == item.Loc.ToUpper() && p.WarehouseSysId == skuBorrowDto.WareHouseSysId).FirstOrDefault();
                if (location == null)
                {
                    throw new Exception($"{item.SkuName} 货位不存在,无法完成商品外借");
                }
                if (location.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"{item.SkuName} 货位已冻结,无法完成商品外借");
                }


                //当前可用数量 
                var invlot = _crudRepository.GetQuery<invlot>(x => x.WareHouseSysId == skuBorrowDto.WareHouseSysId && x.Lot == item.Lot && x.SkuSysId == item.SkuSysId).FirstOrDefault();
                if (invlot != null)
                {
                    var currentQty = CommonBussinessMethod.GetAvailableQty(invlot.Qty, invlot.AllocatedQty, invlot.PickedQty, invlot.FrozenQty);
                    if (currentQty < item.Qty)
                    {
                        throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                    }
                }
                else
                {
                    throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                }

                var invskuloc = _crudRepository.GetQuery<invskuloc>(x => x.WareHouseSysId == skuBorrowDto.WareHouseSysId && x.Loc == item.Loc && x.SkuSysId == item.SkuSysId).FirstOrDefault();
                if (invskuloc != null)
                {
                    var currentQty = CommonBussinessMethod.GetAvailableQty(invskuloc.Qty, invskuloc.AllocatedQty, invskuloc.PickedQty, invskuloc.FrozenQty);
                    if (currentQty < item.Qty)
                    {
                        throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                    }
                }
                else
                {
                    throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                }

                var invlotloclpn = _crudRepository.GetQuery<invlotloclpn>(x => x.WareHouseSysId == skuBorrowDto.WareHouseSysId && x.Loc == item.Loc && x.Lot == item.Lot && x.Lpn == item.Lpn && x.SkuSysId == item.SkuSysId).FirstOrDefault();
                if (invlotloclpn != null)
                {
                    var currentQty = CommonBussinessMethod.GetAvailableQty(invlotloclpn.Qty, invlotloclpn.AllocatedQty, invlotloclpn.PickedQty, invlotloclpn.FrozenQty);
                    if (currentQty < item.Qty)
                    {
                        throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                    }
                }
                else
                {
                    throw new Exception($"{item.SkuName} 库存不足,无法完成商品外借");
                }
            }

            #endregion

            skuBorrowDto.SysId = Guid.NewGuid();
            skuBorrowDto.Status = (int)SkuBorrowStatus.New;

            skuBorrowDto.CreateDate = DateTime.Now;
            skuBorrowDto.UpdateDate = DateTime.Now;

            skuBorrowDto.SkuBorrowDetailList.ForEach(p =>
            {
                //原材料单位转换
                int transQty = 0;
                pack transPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(p.SkuSysId, p.DisplayQty, out transQty, ref transPack) == false)
                {
                    transQty = Convert.ToInt32(p.DisplayQty);
                }
                p.Qty = transQty;

                p.SysId = Guid.NewGuid();
                p.SkuBorrowSysId = skuBorrowDto.SysId.Value;
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
                            TableKey = PublicConst.FileSkuBorrowDetail,
                            TableSysId = p.SysId,
                            Name = item.Name,
                            Url = item.Url,
                            Size = item.Size,
                            Suffix = item.Suffix,
                            CreateBy = skuBorrowDto.UpdateBy,
                            CreateDate = DateTime.Now,
                            UpdateBy = skuBorrowDto.UpdateBy,
                            UpdateDate = DateTime.Now
                        };
                        _crudRepository.Insert(picture);
                    }
                }
                #endregion
            });

            //skuBorrowDto.BorrowOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberSkuBorrow);
            skuBorrowDto.BorrowOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberSkuBorrow);

            var skuborrow = skuBorrowDto.JTransformTo<skuborrow>();

            skuborrow.skuborrowdetails = skuBorrowDto.SkuBorrowDetailList.JTransformTo<skuborrowdetail>();

            _crudRepository.Insert(skuborrow);

        }

        public void UpdateSkuBorrow(SkuBorrowDto skuborrowDto)
        {
            if (skuborrowDto == null || skuborrowDto.SkuBorrowDetailList == null)
            {
                throw new Exception("传入商品外借单无效,缺失主要数据源");
            }
            _crudRepository.ChangeDB(Guid.Parse(skuborrowDto.WareHouseSysId.ToString()));
            var skuborrowSource = _crudRepository.FirstOrDefault<skuborrow>(p => p.SysId == skuborrowDto.SysId);
            if (skuborrowSource == null)
            {
                throw new Exception("请求的商品外借单不存在！");
            }

            skuborrowSource.UpdateBy = skuborrowDto.UpdateBy;
            skuborrowSource.UpdateDate = DateTime.Now;
            skuborrowSource.UpdateUserName = skuborrowDto.UpdateUserName;
            skuborrowSource.TS = Guid.NewGuid();

            if (skuborrowSource.skuborrowdetails != null && skuborrowSource.skuborrowdetails.Count > 0)
            {
                skuborrowSource.skuborrowdetails.ToList().ForEach(p =>
                {
                    var skuborrowDetail = skuborrowDto.SkuBorrowDetailList.FirstOrDefault(q =>
                        q.SkuSysId == p.SkuSysId
                        && q.Lot == p.Lot
                        && q.Loc == p.Loc && q.SysId == p.SysId);
                    if (skuborrowDetail != null)
                    {
                        //原材料单位转换
                        int transQty = 0;
                        pack transPack = new pack();
                        if (_packageAppService.GetSkuConversiontransQty(skuborrowDetail.SkuSysId, skuborrowDetail.DisplayQty, out transQty, ref transPack) == false)
                        {
                            transQty = Convert.ToInt32(skuborrowDetail.DisplayQty);
                        }
                        p.Qty = transQty;

                        p.BorrowStartTime = skuborrowDetail.BorrowStartTime;
                        p.BorrowEndTime = skuborrowDetail.BorrowEndTime;
                        //p.Qty = skuborrowDetail.Qty;
                        p.IsDamage = skuborrowDetail.IsDamage;
                        p.Remark = skuborrowDetail.Remark;
                        p.UpdateBy = skuborrowDetail.UpdateBy;
                        p.UpdateDate = DateTime.Now;
                        p.UpdateUserName = skuborrowDetail.UpdateUserName;
                        p.TS = Guid.NewGuid();

                        #region 图片文件
                        _crudRepository.Delete<picture>(x => x.TableKey == PublicConst.FileSkuBorrowDetail && x.TableSysId == skuborrowDetail.SysId);

                        if (skuborrowDetail.PictureDtoList != null && skuborrowDetail.PictureDtoList.Count > 0)
                        {
                            foreach (var item in skuborrowDetail.PictureDtoList)
                            {
                                var picture = new picture()
                                {
                                    SysId = Guid.NewGuid(),
                                    TableKey = PublicConst.FileSkuBorrowDetail,
                                    TableSysId = skuborrowDetail.SysId,
                                    Name = item.Name,
                                    Url = item.Url,
                                    Size = item.Size,
                                    Suffix = item.Suffix,
                                    CreateBy = skuborrowDto.UpdateBy,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = skuborrowDto.UpdateBy,
                                    UpdateDate = DateTime.Now
                                };
                                _crudRepository.Insert(picture);
                            }
                        }
                        #endregion
                    }
                });
            }

            _crudRepository.Update(skuborrowSource);

        }

        public void DeleteSkuBorrowSkus(List<Guid> request)
        {
            _crudRepository.Delete<skuborrowdetail>(request);
        }

        public void Audit(SkuBorrowDto dto)
        {
            var businessLogDto = new BusinessLogDto();
            _crudRepository.ChangeDB(dto.WareHouseSysId.GetValueOrDefault());

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.PushSkuBorrow
            };

            try
            {
                #region 组织日志数据

                businessLogDto.doc_sysId = dto.SysId;
                businessLogDto.doc_order = "";
                businessLogDto.access_log_sysId = this.AccessLogSysId.HasValue ? this.AccessLogSysId.Value : Guid.NewGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(dto);
                businessLogDto.user_id = dto.AuditingBy.ToString();
                businessLogDto.user_name = dto.AuditingName;
                businessLogDto.business_name = BusinessName.SubBorrow.ToDescription();
                businessLogDto.business_type = BusinessType.VAS.ToDescription();
                businessLogDto.business_operation = PublicConst.AuditSkuBorrow;
                businessLogDto.descr = "[old_json记录 商品外借记录 , new_json记录 交接数据 记录]";
                businessLogDto.flag = true;

                #endregion

                #region 更新业务表

                var skuborrowSource = _crudRepository.FirstOrDefault<skuborrow>(p => p.SysId == dto.SysId);
                if (skuborrowSource == null)
                {
                    throw new Exception("请求的商品外借单不存在！");
                }

                interfaceLogDto.doc_sysId = skuborrowSource.SysId;
                interfaceLogDto.doc_order = skuborrowSource.BorrowOrder;

                businessLogDto.doc_order += skuborrowSource.BorrowOrder;
                if (skuborrowSource.Status == (int)SkuBorrowStatus.New)
                {
                    skuborrowSource.Status = (int)SkuBorrowStatus.Audit;
                    skuborrowSource.BorrowStartTime = DateTime.Now;
                }
                else if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                {
                    skuborrowSource.Status = (int)SkuBorrowStatus.ReturnAudit;
                    skuborrowSource.BorrowEndTime = DateTime.Now;
                }
                skuborrowSource.AuditingBy = dto.AuditingBy.ToString();
                skuborrowSource.AuditingDate = DateTime.Now;
                skuborrowSource.AuditingName = dto.AuditingName;

                _crudRepository.Update(skuborrowSource);

                #endregion

                #region 借出时确定货位判断库存

                List<SkuBorrowDetailDto> detailCheckList = new List<SkuBorrowDetailDto>();

                if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                {
                    if (dto.SkuBorrowDetailList != null && dto.SkuBorrowDetailList.Count > 0)
                    {
                        foreach (SkuBorrowDetailDto detail in dto.SkuBorrowDetailList)
                        {
                            var detailDelete = _crudRepository.FirstOrDefault<skuborrowdetail>(p => p.SysId == detail.SysId);                           

                            List<SkuBorrowDetailDto> detailAddList = new List<SkuBorrowDetailDto>();

                            //取出所有货位信息按照数量倒序排列
                            List<invlotloclpn> invlotloclpnList = _crudRepository.GetAllList<invlotloclpn>(i => i.WareHouseSysId == dto.WareHouseSysId && i.SkuSysId == detail.SkuSysId).OrderByDescending(i => i.Qty).ToList();
                            if (invlotloclpnList == null || invlotloclpnList.Count <= 0)
                            {
                                throw new Exception($"商品：{detail.SkuName} 库存不足，无法完成外借");
                            }
                            //总数量
                            int totalQty = detail.Qty;

                            foreach (var invlotloclpn in invlotloclpnList)
                            {
                                //判断货位冻结
                                var location = _crudRepository.GetQuery<location>(p => p.Loc.ToUpper() == invlotloclpn.Loc.ToUpper() && p.WarehouseSysId == dto.WareHouseSysId).FirstOrDefault();
                                if (location == null || location.Status == (int)LocationStatus.Frozen)
                                {
                                    continue;
                                }

                                SkuBorrowDetailDto detailDto = new SkuBorrowDetailDto();
                                var currentQty = CommonBussinessMethod.GetAvailableQty(invlotloclpn.Qty, invlotloclpn.AllocatedQty, invlotloclpn.PickedQty, invlotloclpn.FrozenQty);
                                if (currentQty <= 0)
                                {
                                    continue;
                                }

                                detailDto.SysId = Guid.NewGuid();
                                detailDto.SkuBorrowSysId = dto.SysId == null ? Guid.Empty : (Guid)dto.SysId;
                                detailDto.SkuSysId = detail.SkuSysId;
                                detailDto.SkuCode = detail.SkuCode;
                                detailDto.Loc = invlotloclpn.Loc;
                                detailDto.Lot = invlotloclpn.Lot;
                                detailDto.Lpn = invlotloclpn.Lpn;
                                detailDto.BorrowStartTime = DateTime.Now;
                                detailDto.Status = (int)SkuBorrowStatus.Audit;
                                detailDto.CreateBy = detailDelete.CreateBy;
                                detailDto.CreateDate = detailDelete.CreateDate;
                                detailDto.CreateUserName = detailDelete.CreateUserName;
                                detailDto.UpdateBy = dto.UpdateBy;
                                detailDto.UpdateDate = dto.UpdateDate;
                                detailDto.UpdateUserName = dto.UpdateUserName;
                                detailDto.Remark = detailDelete.Remark;

                                if (currentQty < totalQty)
                                {
                                    detailDto.Qty = currentQty;
                                    totalQty = totalQty - currentQty;
                                    detailAddList.Add(detailDto);
                                    continue;
                                }
                                else
                                {
                                    detailDto.Qty = totalQty;
                                    totalQty = 0;
                                    detailAddList.Add(detailDto);
                                    break;
                                }
                            }

                            if (totalQty > 0)
                            {
                                throw new Exception($"商品：{detail.SkuName} 库存不足，无法完成外借");
                            }

                            if (detailAddList.Count > 0)
                            {
                                //插入要校验的list
                                detailCheckList.AddRange(detailAddList);

                                //插入有货位的明细数据
                                List<skuborrowdetail> list = detailAddList.JTransformTo<skuborrowdetail>();
                                _crudRepository.BatchInsert(list);
                                //删除无货位的明细数据 
                                if (detailDelete != null)
                                {
                                    _crudRepository.Delete(detailDelete);
                                }
                            }
                        }
                    }
                }
                else if (skuborrowSource.Status == (int)SkuBorrowStatus.ReturnAudit)
                {
                    if (dto.SkuBorrowDetailList != null && dto.SkuBorrowDetailList.Count > 0)
                    {
                        detailCheckList = dto.SkuBorrowDetailList;
                    }
                }

                #endregion

                #region 判断冻结 

                if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                {
                    if (detailCheckList == null || !detailCheckList.Any())
                    {
                        throw new Exception("外借单中没有具体商品,无法完成商品外借!");
                    }

                    var skuList = detailCheckList.Select(p => p.SkuSysId).ToList();
                    var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                        && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == dto.WareHouseSysId).FirstOrDefault();
                    if (frozenSku != null)
                    {
                        var sku = _crudRepository.GetQuery<sku>(x => x.SysId == frozenSku.SkuSysId).FirstOrDefault();
                        throw new Exception($"商品{sku.SkuName}已被冻结,无法完成商品外借!");
                    }

                    var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == dto.WareHouseSysId).ToList();
                    if (locskuList.Count > 0)
                    {
                        var locskuFrozenQuery = from T1 in detailCheckList
                                                join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                                select T2;

                        if (locskuFrozenQuery.Count() > 0)
                        {
                            var firstFrozenLocsku = locskuFrozenQuery.First();
                            var skuSysId = firstFrozenLocsku.SkuSysId;
                            var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                            throw new Exception($"商品'{sku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结,无法完成商品外借!");
                        }
                    }
                }

                #endregion

                #region 判断库存 

                //判断渠道库存
                bool channelStockCheck = false;
                var assemblyrule = _crudRepository.FirstOrDefault<assemblyrule>(a => a.WarehouseSysId == dto.WareHouseSysId);
                if (assemblyrule != null && assemblyrule.MatchingSkuBorrowChannel == true)
                {
                    channelStockCheck = true;
                } 

                foreach (var item in detailCheckList)
                { 
                    if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                    {
                        //判断渠道库存 
                        if (channelStockCheck)
                        {
                            List<invlot> invlotList = _crudRepository.GetAllList<invlot>(i => i.WareHouseSysId == dto.WareHouseSysId && i.SkuSysId == item.SkuSysId && i.Lot == item.Lot && i.LotAttr01 == dto.Channel).ToList();
                            if (invlotList != null && invlotList.Any())
                            {
                                int totalInvLot = invlotList.Sum(i => i.Qty) - invlotList.Sum(i => i.AllocatedQty) - invlotList.Sum(i => i.PickedQty) - invlotList.Sum(i => i.FrozenQty);
                                if (totalInvLot < item.Qty)
                                {
                                    throw new Exception($"商品：{item.SkuName} 渠道库存不足，无法完成外借");
                                }
                            } 
                        } 
                    }  
                    else if (skuborrowSource.Status == (int)SkuBorrowStatus.ReturnAudit)
                    {
                        //原材料单位转换
                        int transQty = 0;
                        pack transPack = new pack();
                        if (_packageAppService.GetSkuConversiontransQty(item.SkuSysId, item.DisplayReturnQty, out transQty, ref transPack) == false)
                        {
                            transQty = Convert.ToInt32(item.DisplayReturnQty);
                        }
                        item.ReturnQty = transQty;

                        if (item.ReturnQty > item.Qty)
                        {
                            throw new Exception($"{item.SkuName} 归还数量大于借出数量，无法完成商品外借");
                        }
                    }

                }

                #endregion

                #region 更新库存 插入交易记录

                List<invtran> tranList = new List<invtran>();

                foreach (var item in detailCheckList)
                {
                    _WMSSqlRepository.UpdateInventoryBySkuBorrow(item, (Guid)dto.WareHouseSysId, dto.AuditingBy, dto.AuditingName, skuborrowSource.Status);

                    invtran tran = new invtran();
                    tran.SysId = Guid.NewGuid();
                    tran.WareHouseSysId = (Guid)skuborrowSource.WareHouseSysId;
                    tran.DocOrder = skuborrowSource.BorrowOrder;
                    tran.DocSysId = skuborrowSource.SysId;
                    tran.DocDetailSysId = item.SysId;
                    tran.SkuSysId = item.SkuSysId;
                    tran.SkuCode = item.SkuCode;
                    if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                    {
                        tran.TransType = InvTransType.Outbound;
                        tran.Qty = -item.Qty;
                    }
                    else if (skuborrowSource.Status == (int)SkuBorrowStatus.ReturnAudit)
                    {
                        tran.TransType = InvTransType.Inbound;
                        tran.Qty = item.ReturnQty;
                    }
                    tran.SourceTransType = "SkuBorrow";
                    tran.Loc = item.Loc;
                    tran.Lot = item.Lot;
                    tran.Lpn = item.Lpn;
                    tran.ToLoc = item.Loc;
                    tran.ToLot = item.Lot;
                    tran.ToLpn = item.Lpn;
                    tran.Status = InvTransStatus.Ok;
                    tran.CreateBy = dto.AuditingBy;
                    tran.CreateUserName = dto.AuditingName;
                    tran.UpdateBy = dto.AuditingBy;
                    tran.UpdateUserName = dto.AuditingName;
                    tranList.Add(tran);
                }
                if (tranList.Any())
                {
                    _WMSSqlRepository.BatchInsertInvTrans(tranList);
                }

                #endregion 

                #region 调用ECC接口写入外借数据

                var result = new CommonResponse();
                //借出
                if (skuborrowSource.Status == (int)SkuBorrowStatus.Audit)
                {
                    ThirdPartySkuBorrowLendDto eccLendDto = new ThirdPartySkuBorrowLendDto()
                    {
                        LendOrderId = string.IsNullOrEmpty(skuborrowSource.OtherId) ? 0 : int.Parse(skuborrowSource.OtherId),
                        EditUserName = dto.AuditingName,
                        EditUserId = dto.AuditingBy,
                        EditTime = DateTime.Now
                    };

                    result = _thirdPartyAppService.PushLendInfoToECC(eccLendDto);
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(result);
                    interfaceLogDto.flag = result.IsSuccess;
                    if (result.IsSuccess == false)
                    {
                        throw new Exception("调用ECC接口传外借数据失败！失败原因：" + result.ErrorMessage);
                    }
                }
                //归还
                else if (skuborrowSource.Status == (int)SkuBorrowStatus.ReturnAudit)
                {
                    List<ThirdPartySkuBorrowReturnDetail> returnDetail = new List<ThirdPartySkuBorrowReturnDetail>();
                    foreach (var item in detailCheckList)
                    {
                        var sku = _crudRepository.FirstOrDefault<sku>(p => p.SysId == item.SkuSysId);

                        returnDetail.Add(new ThirdPartySkuBorrowReturnDetail()
                        {
                            ProductCode = int.Parse(sku.OtherId),
                            DamageLevel = item.IsDamage,
                            DamageReason = item.DamageReason,
                            Quantity = item.ReturnQty,
                            RejectQuantity = item.Qty - item.ReturnQty
                        });
                    }

                    ThirdPartySkuBorrowReturnDto returnDto = new ThirdPartySkuBorrowReturnDto()
                    {
                        LendOrderId = string.IsNullOrEmpty(skuborrowSource.OtherId) ? 0 : int.Parse(skuborrowSource.OtherId),
                        EditUserName = dto.AuditingName,
                        EditUserId = dto.AuditingBy,
                        EditTime = DateTime.Now,
                        Detail = returnDetail,
                        InStockWay = 1,
                        Memo = dto.Remark
                    };

                    result = _thirdPartyAppService.PushReturnInfoToECC(returnDto);
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(result);
                    interfaceLogDto.flag = result.IsSuccess;
                    if (result.IsSuccess == false)
                    {
                        throw new Exception("调用ECC接口传外借归还数据失败！失败原因：" + result.ErrorMessage);
                    }

                }

                #endregion 
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {
                //发送MQ
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        public void Void(SkuBorrowAuditDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var skuborrowSource = _crudRepository.FirstOrDefault<skuborrow>(p => p.SysId == dto.SysId);
            if (skuborrowSource == null)
            {
                throw new Exception("请求的商品外借单不存在！");
            }

            if (skuborrowSource.Status != (int)SkuBorrowStatus.New)
            {
                throw new Exception("只有新建的商品外借单可以作废，请检查！");
            }

            skuborrowSource.Status = (int)SkuBorrowStatus.Void;
            skuborrowSource.AuditingBy = dto.AuditingBy.ToString();
            skuborrowSource.AuditingDate = DateTime.Now;
            skuborrowSource.AuditingName = dto.AuditingName;
            skuborrowSource.TS = Guid.NewGuid();

            _crudRepository.Update(skuborrowSource);
        }

    }
}