using System;
using Abp.Domain.Repositories;
using NBK.ECService.WMS.DTO;
using System.Collections.Generic;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface ISkuRepository: ICrudRepository
    {
        Pages<SkuListDto> GetSkuListByPaging(SkuQuery skuQuery);

        List<SkuWithPackDto> GetSkuPackListByUPC(string upc);
    }
}