using Abp.Application.Services.Dto;
using AutoMapper.QueryableExtensions;
using FortuneLab.Models;
using System.Linq;

namespace Abp.EntityFramework.Extensions
{
    public static class QueryExpressionExtension
    {
        public static Page<TDto> ToPageOutput<TSource, TDto>(this QueryExpression<TSource> obj)
            where TSource : class
            where TDto : class, IDto
        {
            var query = obj.GetQuery(obj.Source);
            var output = new Page<TDto>()
            {
                Paging = new Paging() { PageIndex = obj.RequestInput.PageIndex, PageSize = obj.RequestInput.PageSize },
                Records = query.Project().To<TDto>().ToList()
            };

            //没有分页参数，或者第1页的结果不足一整页时，不需要统计总记录数
            if (obj.RequestInput == null || (obj.RequestInput.SkipCount == 0 && output.Records.Count < obj.RequestInput.PageSize))
            {
                output.Paging.Total = output.Records.Count;
            }
            else
            {
                output.Paging.Total = obj.Source.Count();
            }
            return output;
        }
    }
}
