using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ICarrierAppService : IApplicationService
    {
        /// <summary>
        /// 获取承运商列表
        /// </summary>
        /// <param name="carrierQuery"></param>
        /// <returns></returns>
        Pages<CarrierDto> GetCarrierList(CarrierQuery carrierQuery);

        /// <summary>
        /// 新增承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        /// <returns></returns>
        void AddCarrier(CarrierDto carrierDto);

        /// <summary>
        /// 根据Id获取承运商
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        CarrierDto GetCarrierById(Guid sysId);

        /// <summary>
        /// 编辑承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        void EditCarrier(CarrierDto carrierDto);

        /// <summary>
        /// 删除承运商
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteCarrier(List<Guid> sysIds);

        /// <summary>
        /// 获取可用的承运商
        /// </summary>
        /// <returns></returns>
        List<CarrierDto> GetExpressListByIsActive();
    }
}
