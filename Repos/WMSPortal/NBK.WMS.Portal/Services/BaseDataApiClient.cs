using System;
using System.Collections.Generic;
using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.System;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using NBK.WMS.Portal.Models;

namespace NBK.WMS.Portal.Services
{

    public class BaseDataApiClient
    {

        private static readonly BaseDataApiClient instance = new BaseDataApiClient();

        private BaseDataApiClient()
        {
        }

        public static BaseDataApiClient GetInstance()
        {
            return instance;
        }

        #region 菜单
        public ApiResponse<List<MenuDto>> GetSystemMenuList()
        {
            var query = new CoreQuery();
            return ApiClient.Get<List<MenuDto>>(PublicConst.WmsApiUrl, "BaseData/System/GetSystemMenuList", query);
        }
        #endregion

        #region 登陆相关

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> UserLoginCheck(ApplicationUser user)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Account/UserLoginCheck", new CoreQuery(), user);
        }


        public ApiResponse UserLoginSuccess(ApplicationUser user)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Account/UserLoginSuccess", new CoreQuery(), user);
        }

        public ApiResponse UserLoginFail(ApplicationUser user)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Account/UserLoginFail", new CoreQuery(), user);
        }

        #endregion

        #region 系统代码SysCode
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<SysCodeDto> GetSysCodeDtoById(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId };
            return ApiClient.Post<SysCodeDto>(PublicConst.WmsApiUrl, "BaseData/SysCode/GetSysCodeDtoById", query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<SysCodeDetailDto> GetSysCodeDetailDtoById(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId };
            return ApiClient.Get<SysCodeDetailDto>(PublicConst.WmsApiUrl, "BaseData/SysCode/GetSysCodeDetailDtoById", query);
        }

        /// <summary>
        /// DeleteSysCodeDetailById
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>

        public ApiResponse<SysCodeDetailDto> DeleteSysCodeDetailByIdList(List<Guid> sysIdList)
        {
            var query = new CoreQuery();
            return ApiClient.Post<SysCodeDetailDto>(PublicConst.WmsApiUrl, "BaseData/SysCode/DeleteSysCodeDetailByIdList", query, sysIdList);
        }

        /// <summary>
        /// 系统代码列表
        /// </summary> 
        /// <param name="sysCodeQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SysCodeDto>> GetSysCodeDtoList(SysCodeQuery sysCodeQuery)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<SysCodeDto>>(PublicConst.WmsApiUrl, "/BaseData/SysCode/GetSysCodeDtoList", query, sysCodeQuery);
        }

        /// <summary>
        /// 系统代码明细列表
        /// </summary> 
        /// <param name="sysCodeSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<SysCodeDetailDto>> GetSysCodeDetailDtoList(Guid sysCodeSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysCodeSysId };
            return ApiClient.Post<List<SysCodeDetailDto>>(PublicConst.WmsApiUrl, "/BaseData/SysCode/GetSysCodeDetailDtoList", query);
        }

        /// <summary>
        /// 更新系统代码
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysCodeDto"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveSysCode(SysCodeDto sysCodeDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/SysCode/UpdateSysCode", query, sysCodeDto);
        }

        /// <summary>
        /// 更新系统代码明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public ApiResponse<string> UpdateSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/SysCode/UpdateSysCodeDetail", query, sysCodeDetailDto);
        }

        /// <summary>
        /// 新增系统代码明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public ApiResponse<string> InsertSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/SysCode/InsertSysCodeDetail", query, sysCodeDetailDto);
        }


        /// <summary>
        /// 新增系统代码明细
        /// UOM  计量单位类型
        /// ShelfLifeType 保质期类型
        /// LocUsage 货位用途
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemSysCode(string sysCodeType, bool isActive = true)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysCodeType, isActive };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "BaseData/SysCode/GetSelectBySysCode", query);
        }
        #endregion

        #region 批次模板LotTemplate

        /// <summary>
        /// 获取Lot批次模板
        /// </summary>
        /// <param name="lotTemplateQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<LotTemplateListDto>> GetLotTempListDto(LotTemplateQuery lotTemplateQuery)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<LotTemplateListDto>>(PublicConst.WmsApiUrl, "/BaseData/LotTemplate/GetLotTemplatList", query, lotTemplateQuery);
        }

        /// <summary>
        /// 根据ID获取相关数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<LotTemplateDto> GetLotTemplateDtoById(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId };
            return ApiClient.Post<LotTemplateDto>(PublicConst.WmsApiUrl, "BaseData/LotTemplate/GetLotTemplateDtoById", query);
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<string> DeleteLotTemplate(List<Guid> sysIdList)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/LotTemplate/DeleteLotTemplate", query, sysIdList);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        /// <returns></returns>
        public ApiResponse<string> InsertLotTemplate(LotTemplateDto lotTemplateDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/LotTemplate/InsertLotTemplate", query, lotTemplateDto);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        /// <returns></returns>
        public ApiResponse<string> UpdateLotTemplate(LotTemplateDto lotTemplateDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "BaseData/LotTemplate/UpdateLotTemplate", query, lotTemplateDto);
        }

        /// <summary>
        /// 批次模板下拉 参数可控
        /// </summary>
        /// <param name="lotCode"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemLotTemplate(string lotCode = null)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { lotCode };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "BaseData/LotTemplate/GetSelectLotTemplate", query);
        }

        #endregion

        #region 包装管理相关

        /// <summary>
        /// 条件分页查询包装计量单位
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<UOMDto>> GetUOMList(UOMQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<UOMDto>>(PublicConst.WmsApiUrl, "/BaseData/Package/GetUOMList", query, request);
        }

        /// <summary>
        /// 根据sysid查询UOM对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<UOMDto> GetUOMBySysId(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId };
            return ApiClient.Get<UOMDto>(PublicConst.WmsApiUrl, "/BaseData/Package/GetUOMBySysId", query);
        }

        /// <summary>
        /// 更新UOM
        /// </summary>
        /// <param name="uom"></param>
        /// <returns></returns>
        public ApiResponse UpdateUOM(UOMDto uom)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Package/UpdateUOM", query, uom);
        }

        /// <summary>
        /// 添加UOM
        /// </summary>
        /// <param name="uom"></param>
        /// <returns></returns>
        public ApiResponse AddUOM(UOMDto uom)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Package/AddUOM", query, uom);
        }

        /// <summary>
        /// 删除UOM
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse DeleteUOM(string sysIdList)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysIdList };
            return ApiClient.Get(PublicConst.WmsApiUrl, "/BaseData/Package/DeleteUOM", query);
        }

        /// <summary>
        /// 条件分页查询包装信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PackDto>> GetPackList(PackQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<PackDto>>(PublicConst.WmsApiUrl, "/BaseData/Package/GetPackList", query, request);
        }

        /// <summary>
        /// 查询包装下拉框数据源
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemPack(string packCode)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { packCode };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "BaseData/Package/GetSelectPack", query);
        }

        /// <summary>
        /// 根据sysid查询Pack对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<PackDto> GetPackBySysId(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId };
            return ApiClient.Get<PackDto>(PublicConst.WmsApiUrl, "/BaseData/Package/GetPackBySysId", query);
        }

        /// <summary>
        /// 更新Pack
        /// </summary>
        /// <param name="packDto"></param>
        /// <returns></returns>
        public ApiResponse UpdatePack(PackDto packDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Package/UpdatePack", query, packDto);
        }

        /// <summary>
        /// 添加Pack
        /// </summary>
        /// <param name="packDto"></param>
        /// <returns></returns>
        public ApiResponse AddPack(PackDto packDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Package/AddPack", query, packDto);
        }
        /// <summary>
        /// 删除包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse DeletePack(string sysIdList)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysIdList };
            return ApiClient.Get(PublicConst.WmsApiUrl, "/BaseData/Package/DeletePack", query);
        }

        #endregion

        #region 容器管理

        /// <summary>
        /// 条件分页查询容器信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ContainerDto>> GetContainerList(ContainerQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<ContainerDto>>(PublicConst.WmsApiUrl, "/BaseData/Container/GetContainerList", query, request);
        }

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse DeleteContainer(string sysIdList, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysIdList, warehouseSysId };
            return ApiClient.Get(PublicConst.WmsApiUrl, "/BaseData/Container/DeleteContainer", query);
        }

        /// <summary>
        /// 创建容器
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public ApiResponse AddContainer(ContainerDto container)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Container/AddContainer", query, container);
        }

        /// <summary>
        /// 根据sysid查询Container对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<ContainerDto> GetContainerBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<ContainerDto>(PublicConst.WmsApiUrl, "/BaseData/Container/GetContainerBySysId", query);
        }

        /// <summary>
        /// 更新容器
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public ApiResponse UpdateContainer(ContainerDto container)
        {
            var query = new CoreQuery();
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/Container/UpdateContainer", query, container);
        }

        /// <summary>
        /// 获取可用的箱记录
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<ContainerDto>> GetContainerListByIsActive(Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Get<List<ContainerDto>>(PublicConst.WmsApiUrl, "/BaseData/Container/GetContainerListByIsActive", query);
        }

        #endregion

        #region 商品管理
        /// <summary>
        /// 获取SKU列表
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuListDto>> GetSkuList(CoreQuery query, SkuQuery skuQuery)
        {
            return ApiClient.Post<Pages<SkuListDto>>(PublicConst.WmsApiUrl, "/Sku/GetSkuList", query, skuQuery);
        }

        /// <summary>
        /// 新增SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        public ApiResponse AddSku(CoreQuery query, SkuDto skuDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Sku/AddSku", query, skuDto);
        }

        /// <summary>
        /// 根据Id获取SKU
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<SkuDto> GetSkuById(CoreQuery query, string sysId)
        {
            query.ParmsObj = new { sysId };
            return ApiClient.Get<SkuDto>(PublicConst.WmsApiUrl, "/Sku/GetSkuById", query);
        }

        /// <summary>
        /// 编辑SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> EditSku(CoreQuery query, SkuDto skuDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Sku/EditSku", query, skuDto);
        }

        /// <summary>
        /// 删除SKU
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> DeleteSku(CoreQuery query, List<Guid> sysIds)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Sku/DeleteSku", query, sysIds);
        }

        /// <summary>
        /// 根据UPC 获取 商品UPC 已经 包装UPC对应的商品信息集合
        /// </summary>
        /// <param name="query"></param>
        /// <param name="upc"></param>
        /// <returns></returns>
        public ApiResponse<List<SkuWithPackDto>> GetSkuAndSkuPackListByUPC(CoreQuery query, DuplicateUPCChooseQuery request)
        {
            return ApiClient.Post<List<SkuWithPackDto>>(PublicConst.WmsApiUrl, "/Sku/GetSkuAndSkuPackListByUPC", query, request);
        }

        #endregion

        #region 储区
        /// <summary>
        /// 获取储区列表
        /// </summary>
        /// <param name="zoneQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ZoneDto>> GetZoneList(CoreQuery query, ZoneQuery zoneQuery)
        {
            return ApiClient.Post<Pages<ZoneDto>>(PublicConst.WmsApiUrl, "/Zone/GetZoneList", query, zoneQuery);
        }

        /// <summary>
        /// 新增储区
        /// </summary>
        /// <param name="zoneDto"></param>
        /// <returns></returns>
        public ApiResponse<Guid> AddZone(CoreQuery query, ZoneDto zoneDto)
        {
            return ApiClient.Post<Guid>(PublicConst.WmsApiUrl, "/Zone/AddZone", query, zoneDto);
        }

        /// <summary>
        /// 根据Id获取储区
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<ZoneDto> GetZoneById(CoreQuery query, string sysId, Guid warehouseSydId)
        {
            query.ParmsObj = new { sysId, warehouseSydId };
            return ApiClient.Get<ZoneDto>(PublicConst.WmsApiUrl, "/Zone/GetZoneById", query);
        }

        /// <summary>
        /// 编辑储区
        /// </summary>
        /// <param name="zoneDto"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> EditZone(CoreQuery query, ZoneDto zoneDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Zone/EditZone", query, zoneDto);
        }

        /// <summary>
        /// 删除储区
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> DeleteZone(CoreQuery query, List<Guid> sysIds, Guid warehouseSydId)
        {
            query.ParmsObj = new { warehouseSydId };
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Zone/DeleteZone", query, sysIds);
        }

        /// <summary>
        /// 获取储区数下拉列表据源
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectZone(Guid? warehouseSysId = null, string zoneCode = null)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId, zoneCode };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "Zone/GetSelectZone", query);
        }
        #endregion

        #region 库位
        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<LocationListDto>> GetLocationList(CoreQuery query, LocationQuery locationQuery)
        {
            return ApiClient.Post<Pages<LocationListDto>>(PublicConst.WmsApiUrl, "/Location/GetLocationList", query, locationQuery);
        }

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        public ApiResponse<Guid> AddLocation(CoreQuery query, LocationDto locationDto)
        {
            return ApiClient.Post<Guid>(PublicConst.WmsApiUrl, "/Location/AddLocation", query, locationDto);
        }

        /// <summary>
        /// 根据Id获取库位
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<LocationDto> GetLocationById(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<LocationDto>(PublicConst.WmsApiUrl, "/Location/GetLocationById", query);
        }

        /// <summary>
        /// 编辑库位
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> EditLocation(CoreQuery query, LocationDto locationDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Location/EditLocation", query, locationDto);
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> DeleteLocation(CoreQuery query, List<Guid> sysIds, Guid warehouseSysId)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Location/DeleteLocation", query, sysIds);
        }

        /// <summary>
        /// 获取库位下拉框
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectLocation(CoreQuery query, Guid wareHouseSysId, string zoneSysId = null)
        {
            query.ParmsObj = new { wareHouseSysId, zoneSysId };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "/Location/GetSelectLocation", query);
        }

        /// <summary>
        /// 判断货位是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<RFCommResult> LocIsExist(LocationQuery locationQuery)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Base/LocIsExist", new CoreQuery(), locationQuery);
        }
        #endregion

        #region 承运商
        /// <summary>
        /// 获取承运商列表
        /// </summary>
        /// <param name="carrierQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<CarrierDto>> GetCarrierList(CoreQuery query, CarrierQuery carrierQuery)
        {
            return ApiClient.Post<Pages<CarrierDto>>(PublicConst.WmsApiUrl, "/Carrier/GetCarrierList", query, carrierQuery);
        }

        /// <summary>
        /// 新增承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        /// <returns></returns>
        public ApiResponse AddCarrier(CoreQuery query, CarrierDto carrierDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Carrier/AddCarrier", query, carrierDto);
        }

        /// <summary>
        /// 根据Id获取承运商
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<CarrierDto> GetCarrierById(CoreQuery query, string sysId)
        {
            query.ParmsObj = new { sysId };
            return ApiClient.Get<CarrierDto>(PublicConst.WmsApiUrl, "/Carrier/GetCarrierById", query);
        }

        /// <summary>
        /// 编辑承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> EditCarrier(CoreQuery query, CarrierDto carrierDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Carrier/EditCarrier", query, carrierDto);
        }

        /// <summary>
        /// 删除承运商
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> DeleteCarrier(CoreQuery query, List<Guid> sysIds)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Carrier/DeleteCarrier", query, sysIds);
        }

        /// <summary>
        /// 获取可用承运商
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<CarrierDto>> GetExpressListByIsActive()
        {
            var query = new CoreQuery();
            return ApiClient.Get<List<CarrierDto>>(PublicConst.WmsApiUrl, "/Carrier/GetExpressListByIsActive", query);
        }

        #endregion

        #region 仓库
        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ApiResponse<List<WareHouseDto>> GetWareHouseByUserId(CoreQuery query, int userId)
        {
            query.ParmsObj = new { userId };
            return ApiClient.Post<List<WareHouseDto>>(PublicConst.WmsApiUrl, "/WareHouse/GetWareHouseByUserId", query);
        }

        public ApiResponse<Pages<UserWarehouseDto>> GetNoAssignedWarehouse(UserWarehouseQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<UserWarehouseDto>>(PublicConst.WmsApiUrl, "/WareHouse/GetNoAssignedWarehouse", query, request);
        }

        public ApiResponse<Pages<UserWarehouseDto>> GetAssignedWarehouse(UserWarehouseQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<UserWarehouseDto>>(PublicConst.WmsApiUrl, "/WareHouse/GetAssignedWarehouse", query, request);
        }

        public ApiResponse SetAssignedWarehouse(UserWarehouseDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/WareHouse/SetAssignedWarehouse", query, request);
        }

        public ApiResponse SetNoAssignedWarehouse(UserWarehouseDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/WareHouse/SetNoAssignedWarehouse", query, request);
        }
        #endregion

        #region 组装件
        /// <summary>
        /// 获取组装件列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ComponentListDto>> GetComponentListByPaging(ComponentQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<ComponentListDto>>(PublicConst.WmsApiUrl, "/Component/GetComponentListByPaging", query, request);
        }

        /// <summary>
        /// 获取组装件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<ComponentDto> GetComponentById(ComponentQuery request, CoreQuery query)
        {
            return ApiClient.Post<ComponentDto>(PublicConst.WmsApiUrl, "/Component/GetComponentById", query, request);
        }
        #endregion

        #region 地区
        public ApiResponse<Dictionary<string, string>> GetRegionListByName(CoreQuery query, string name)
        {
            query.ParmsObj = new { name };
            return ApiClient.Get<Dictionary<string, string>>(PublicConst.WmsApiUrl, "/ThirdParty/GetRegionListByName", query);
        }

        public ApiResponse<string> GetRegionIntactBySysId(CoreQuery query, Guid regionSysId)
        {
            query.ParmsObj = new { regionSysId };
            return ApiClient.Get<string>(PublicConst.WmsApiUrl, "/ThirdParty/GetRegionIntactBySysId", query);
        }
        #endregion

        #region 工单用户
        public ApiResponse<Pages<WorkUserDto>> GetWorkUserList(CoreQuery query, WorkUserQuery workUserQuery)
        {
            return ApiClient.Post<Pages<WorkUserDto>>(PublicConst.WmsApiUrl, "/BaseData/WorkUser/GetWorkUserList", query, workUserQuery);
        }

        public ApiResponse AddWorkUser(CoreQuery query, WorkUserDto workUserDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/WorkUser/AddWorkUser", query, workUserDto);
        }

        public ApiResponse<WorkUserDto> GetWorkUserById(CoreQuery query, Guid sysId, Guid warehouseSydId)
        {
            query.ParmsObj = new { sysId, warehouseSydId };
            return ApiClient.Get<WorkUserDto>(PublicConst.WmsApiUrl, "/BaseData/WorkUser/GetWorkUserById", query);
        }

        public ApiResponse<List<WorkUserListDto>> GetWorkUsers(CoreQuery query, WorkUserQuery workUserQuery)
        {
            return ApiClient.Post<List<WorkUserListDto>>(PublicConst.WmsApiUrl, "/BaseData/WorkUser/GetWorkUsers", query, workUserQuery);
        }

        public ApiResponse EditWorkUser(CoreQuery query, WorkUserDto workUserDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/WorkUser/EditWorkUser", query, workUserDto);
        }

        public ApiResponse DeleteWorkUser(CoreQuery query, List<Guid> sysIds, Guid warehouseSydId)
        {
            query.ParmsObj = new { warehouseSydId };
            return ApiClient.Post(PublicConst.WmsApiUrl, "/BaseData/WorkUser/DeleteWorkUser", query, sysIds);
        }
        #endregion
    }
}