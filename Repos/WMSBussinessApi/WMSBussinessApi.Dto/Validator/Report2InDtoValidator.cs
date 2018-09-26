using FluentValidation.Results;
using Schubert.Framework.Web.Validation;
using FluentValidation;
using WMSBussinessApi.Dto.Common;
using WMSBussinessApi.Dto.XXX;

namespace WMSBussinessApi.Dto.Validator
{
    public class Report2InDtoValidator : DependencyValidator<Report2InDto>
    {
        public Report2InDtoValidator()
        {
            RuleFor(request => request.TiaoJian1).Equal("Jelax").WithMessage("TiaoJian1 必须是Jelax");   //意思是: TiaoJian1属性必须等于jelax,否则验证不通过,且提示信息为 "TiaoJian1 必须是Jelax"
            RuleFor(request => request.TiaoJian2).Equal("Wang").WithMessage("TiaoJian2 必须是Wang");
            Custom(request =>
            {
                if (request.TiaoJian1 != "Jelax")
                {
                    return new ValidationFailure("TiaoJian1", "TiaoJian1 必须是Jelax");
                }
                if (request.TiaoJian2 != "Wang")
                {
                    return new ValidationFailure("TiaoJian2", "TiaoJian2 必须是Wang");
                }
                return null;
            });
            //上面两个RuleFor 与 Custom 类似
            RuleFor(request => request.PageInfo).NotNull().WithMessage("分页信息必须有");
            RuleFor(request => request.PageInfo).Must(PageInfoMustRight).WithMessage("分页信息必须正确");
        }


        private bool PageInfoMustRight(PagingInfoInDto pginfo)
        {
            if (pginfo.CurrentPage <=0)
            {
                return false;
            }
            else if (pginfo.PageSize <= 0)
            {
                return false;
            }
            return true;
        }

        //程序先执行RuleFor ,执行完所有的RuleFor后,如果没有错误,则再执行Custom.
        //程序先执行RuleFor ,执行完所有的RuleFor后,如果有错误,则不会执行Custom.
    }
}
