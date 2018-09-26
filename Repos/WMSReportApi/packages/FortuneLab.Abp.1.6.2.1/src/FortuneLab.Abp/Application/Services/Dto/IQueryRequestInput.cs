using Abp.Runtime.Validation;

namespace Abp.Application.Services.Dto
{
    public interface IQueryRequestInput : IInputDto, IFTPagedResultRequest, IFTSortedResultRequest, ICustomValidate
    {
        string Keywords { get; set; }
    }
}
