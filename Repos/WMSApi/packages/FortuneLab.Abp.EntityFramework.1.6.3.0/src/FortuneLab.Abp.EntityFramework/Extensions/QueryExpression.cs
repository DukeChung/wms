using Abp.Application.Services.Dto;
using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;
using System.Collections.Immutable;
using System.Linq.Dynamic;

namespace Abp.EntityFramework.Extensions
{
    public class QueryExpression<TSource> 
        where TSource : class
    {
        private IQueryable<TSource> _source;
        private IQueryRequestInput _requestInput;

        public IQueryable<TSource> Source { get { return _source; } }
        public IQueryRequestInput RequestInput { get { return _requestInput; } }

        public QueryExpression(IQueryable<TSource> source, IQueryRequestInput requestInput = null)
        {
            this._source = source;
            this._requestInput = requestInput;
        }

        //public QueryResultOutput<TDto> To<TDto>() where TDto : class, IDto
        //{
        //    var count = _source.FutureCount();
        //    var query = _source;
        //    if (!string.IsNullOrWhiteSpace(_requestInput.Sorting))
        //    {
        //        query = query.OrderBy(_requestInput.Sorting);
        //    }
        //    var items = query.PageBy(_requestInput).Project().To<TDto>().Future();

        //    return new QueryResultOutput<TDto>
        //    {
        //        TotalCount = count.Value,
        //        Items = items.ToList()
        //    };
        //}

        /// <summary>
        /// 返回添加排序和分页后的IQueryable
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public IQueryable<TSource> To()
        {
            return GetQuery(_source);
        }

        /// <summary>
        /// 转换指定Dto类型的IQueryable
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public IQueryable<TDto> To<TDto>() where TDto : class, IDto
        {
            var query = GetQuery(_source);

            return query.Project().To<TDto>();
        }


        /// <summary>
        /// 生成指定Dto类型的分页结果
        /// </summary>
        /// <typeparam name="TDto">输出的Dto类型</typeparam>
        /// <returns></returns>
        public QueryResultOutput<TDto> ToOutput<TDto>() where TDto : class, IDto
        {
            return AsyncHelper.RunSync(() => ToOutputAsync<TDto>());
        }

        /// <summary>
        /// 生成指定Dto类型的分页结果
        /// </summary>
        /// <typeparam name="TDto">输出的Dto类型</typeparam>
        /// <returns></returns>
        public async Task<QueryResultOutput<TDto>> ToOutputAsync<TDto>() where TDto : class, IDto
        {
            var query = GetQuery(_source);
            var output = new QueryResultOutput<TDto>()
            {
                Items = (await query.Project().To<TDto>().ToListAsync()).ToImmutableList()
            };

            //没有分页参数，或者第1页的结果不足一整页时，不需要统计总记录数
            if (_requestInput == null || (_requestInput.SkipCount == 0 && output.Items.Count < _requestInput.PageSize))
            {
                output.TotalCount = output.Items.Count;
            }
            else
            {
                output.TotalCount = await _source.CountAsync();
            }
            return output;
        }


        /// <summary>
        /// 生成指定Dto类型的列表
        /// </summary>
        public List<TDto> ToList<TDto>() where TDto : class, IDto
        {
            return AsyncHelper.RunSync(() => ToListAsync<TDto>());
        }

        /// <summary>
        /// 生成指定Dto类型的列表
        /// </summary>
        public async Task<List<TDto>> ToListAsync<TDto>() where TDto : class, IDto
        {
            var query = GetQuery(_source);

            return await query.Project().To<TDto>().ToListAsync();
        }

        /// <summary>
        /// 生成原类型的列表
        /// </summary>
        public List<TSource> ToList()
        {
            return AsyncHelper.RunSync(() => ToListAsync());
        }

        /// <summary>
        /// 生成原类型的列表
        /// </summary>
        public async Task<List<TSource>> ToListAsync()
        {
            var query = GetQuery(_source);

            return await query.ToListAsync();
        }

        public IQueryable<T> GetQuery<T>(IQueryable<T> query)
        {
            if (_requestInput != null)  //不排序、分页参数
            {
                if (!string.IsNullOrWhiteSpace(_requestInput.SortField) && !string.IsNullOrWhiteSpace(_requestInput.SortDirection))
                {
                    query = query.OrderBy(string.Format(" {0} {1} ", _requestInput.SortField, _requestInput.SortDirection));
                }
                query = query.PageBy(_requestInput);
            }
            return query;
        }
    }
}
