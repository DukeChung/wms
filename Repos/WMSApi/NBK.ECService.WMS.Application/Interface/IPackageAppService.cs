using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPackageAppService : IApplicationService
    {
        /// <summary>
        /// 分页条件查询UOM集合
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Pages<UOMDto> GetUOMList(UOMQuery query);

        /// <summary>
        /// 根据sysid查询UOM对象
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        UOMDto GetUOMBySysId(Guid sysId);

        /// <summary>
        /// 更新uom
        /// </summary>
        /// <param name="uomDto"></param>
        /// <returns></returns>
        void UpdateUOM(UOMDto uomDto);

        /// <summary>
        /// 添加uom
        /// </summary>
        /// <param name="uom"></param>
        void AddUOM(UOMDto uom);

        /// <summary>
        /// 删除UOM
        /// </summary>
        /// <param name="sysId"></param>
        void DeleteUOM(string sysIdList);

        #region pack managemet

        /// <summary>
        /// 分页条件查询Pack集合
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Pages<PackDto> GetPackList(PackQuery query);

        /// <summary>
        /// 根据sysid查询PackM对象
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PackDto GetPackBySysId(Guid sysId);


        /// <summary>
        /// 更新Pack
        /// </summary>
        /// <param name="packDto"></param>
        /// <returns></returns>
        void UpdatePack(PackDto packDto);

        /// <summary>
        /// 新增pack
        /// </summary>
        /// <param name="packDto"></param>
        void AddPack(PackDto packDto);

        /// <summary>
        /// 删除包装
        /// </summary>
        /// <param name="sysId"></param>
        void DeletePack(string sysIdList);

        /// <summary>
        /// 获取包装下拉框集合
        /// </summary>
        /// <param name="packCode"></param>
        /// <returns></returns>
        List<SelectItem> GetSelectPack(string packCode);

        bool GetSkuConversiontransQty(Guid skuSysId, int requestQty, out int responseQty, ref pack pack);

        bool GetSkuConversiontransQty(Guid skuSysId, decimal requestQty, out int responseQty, ref pack pack);

        bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty);

        bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty, ref uom uom);


        /// <summary>
        /// 根据商品包赚信息获取单位转化后的数据
        /// </summary>
        /// <param name="request">需要转化的商品信息</param>
        /// <param name="response"></param>
        void GetSkuConversionQty(ref SkuPackageConvertDto request);
        #endregion
    }
}
