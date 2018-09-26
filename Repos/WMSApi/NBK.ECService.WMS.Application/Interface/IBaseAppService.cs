using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System.Collections.Generic;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IBaseAppService : IApplicationService
    {
        /// <summary>
        /// 生成单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<string> GenNextNumber(string tableName, int count);

        /// <summary>
        /// 获取单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<string> GetNumber(string tableName, int count);

        /// <summary>
        /// 获取单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GetNumber(string tableName);

        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        CoordinateDto GetCoordinate(string city, string address);

        /// <summary>
        /// 从新推送出库单到ECC
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        CommonResponse PushOutboundECC(string outboundOrder);
    }
}
