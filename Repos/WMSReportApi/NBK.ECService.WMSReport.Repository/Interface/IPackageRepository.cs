using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Package;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IPackageRepository : ICrudRepository
    {
        Pages<UOMDto> GetUOMList(UOMQuery query);
    }
}