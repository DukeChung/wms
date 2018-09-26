using Abp.Application.Services.Dto;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Abp.EntityFramework.Extensions
{
    /// <summary>
    /// Some useful extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable IncludeIf(this IQueryable source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable<T> IncludeIf<T>(this IQueryable<T> source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The type of navigation property being included.</param>
        public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> source, bool condition, Expression<Func<T, TProperty>> path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
        /// </summary>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (skipCount > 0)  //优化查询，第一页数据时，在SQL中不用生成row_number，所以只有大于第一页，才加Skip。
            {
                query = query.Skip(skipCount);
            }

            return query.Take(maxResultCount);
        }

        /// <summary>
        /// Used for paging with an <see cref="IPagedResultRequest"/> object.
        /// </summary>
        /// <param name="query">Queryable to apply paging</param>
        /// <param name="pagedResultRequest">An object implements <see cref="IPagedResultRequest"/> interface</param>
        /// <returns></returns>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, IFTPagedResultRequest pagedResultRequest)
        {
            //pagedResultRequest.SkipCount= 
            return query.PageBy((pagedResultRequest.PageIndex - 1) * pagedResultRequest.PageSize, pagedResultRequest.PageSize);
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the query</param>
        /// <returns>Filtered or not filtered query based on <see cref="condition"/></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the query</param>
        /// <returns>Filtered or not filtered query based on <see cref="condition"/></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 生成一个新的查询表达式，用于支持调用To方法时，只需要指定目标Dto类型，而不需要再指定源实体类型
        /// 根据requestInput进行排序、分页。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="requestInput">传递查询参数的对象</param>
        /// <returns></returns>
        public static QueryExpression<TSource> Query<TSource>(this IQueryable<TSource> source, IQueryRequestInput requestInput = null) where TSource : class//, IEntity  //TODO:加约束
        {
            //source = filterSoftDeleted(source);
            var expression = new QueryExpression<TSource>(source, requestInput);
            return expression;
        }


        //public static IProjectionExpression Query<TSource>(this IQueryable<TSource> source) where TSource : class//, IEntity
        //{
        //    return source.Query(Mapper.Engine);
        //}

        //public static IProjectionExpression Query<TSource>(this IQueryable<TSource> source, IMappingEngine mappingEngine) where TSource : class//, IEntity
        //{
        //    //source = filterSoftDeleted(source); 
        //    return new ProjectionExpression<TSource>(source, mappingEngine);
        //}

        //////自动过滤已软删除的数据
        //private static IQueryable<TSource> filterSoftDeleted<TSource>(IQueryable<TSource> source) where TSource : class//, ISoftDelete
        //{
        //    if (typeof(ISoftDelete).IsAssignableFrom(typeof(TSource)))  //实现了ISoftDelete接口
        //    {
        //        source = source.Where(createIsDeletedExpression<TSource>());
        //    }
        //    return source;
        //}

        //private static Expression<Func<TEntity, bool>> createIsDeletedExpression<TEntity>() where TEntity : class//, IEntity
        //{
        //    var lambdaParam = Expression.Parameter(typeof(TEntity));

        //    var lambdaBody = Expression.Equal(
        //        Expression.PropertyOrField(lambdaParam, "IsDeleted"),
        //        Expression.Constant(false, typeof(bool))
        //        );

        //    return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        //}

        ////自动过滤租户
        //private static IQueryable<TSource> filterByTenant<TSource>(IQueryable<TSource> source) where TSource : class//, IEntity
        //{
        //    if (typeof(TSource).GetInterface("IFilterByTenant") != null)  //实现了IFilterByTenant接口，直接通过TenantId过滤
        //    {
        //        var session = IocManager.Instance.IocContainer.Resolve<IAbpSession>();
        //        var tenantId = session.TenantId;

        //        if (typeof(TSource).GetInterface("IMayHaveTenant") != null)
        //        {
        //            source = source.Where(CreateTenantIdEqualityExpression<TSource>(tenantId));
        //            //source = filterByMayHaveTenant(source);
        //        }
        //        else if (typeof(TSource).GetInterface("IMustHaveTenant") != null && tenantId.HasValue)
        //        {
        //            source = source.Where(CreateTenantIdEqualityExpression<TSource>(tenantId.Value));
        //            //source = source.Where(item => ((IMustHaveTenant)item).TenantId == tenantId.Value);
        //        }
        //        else
        //        {
        //            throw new AbpException(typeof(TSource).FullName + "需要TenantId，但当前用户无TenantId");
        //        }

        //    }
        //    return source;
        //}


        //private static Expression<Func<TEntity, bool>> CreateTenantIdEqualityExpression<TEntity>(Guid? tenantId) where TEntity : class//, IEntity
        //{
        //    var lambdaParam = Expression.Parameter(typeof(TEntity));

        //    var lambdaBody = Expression.Equal(
        //        Expression.PropertyOrField(lambdaParam, "TenantId"),
        //        Expression.Constant(tenantId, typeof(Guid?))
        //        );

        //    return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        //}

    }
}
