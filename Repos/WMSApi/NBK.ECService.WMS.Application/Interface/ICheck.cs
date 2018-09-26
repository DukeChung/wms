using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ICheck
    {
        CommonResponse Execute();

        void BeforeCheck();

        CommonResponse Check();

        void AfterCheck();
    }
}
