using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Repositories
{
    public interface ISimpleRepository<TPrimaryKey> : IRepository
    {
        #region Property
        string ConnectionString { get; }
        IDbConnection Connection { get; }
        #endregion

        #region Select/Get/Query

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// <see cref="UnitOfWorkAttribute"/> attribute must be used to be able to call this method since this method
        /// returns IQueryable and it requires open database connection to use it.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        List<TEntity> GetAllList<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllListAsync<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        List<TEntity> GetAllList<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessary (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        T Query<TEntity, T>(Func<IQueryable<TEntity>, T> queryMethod) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        TEntity Get<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        Task<TEntity> GetAsync<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
        TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">Entity</param>
        Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        TEntity FirstOrDefault<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FirstOrDefaultAsync<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given given predicate or null if not found.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets an entity with given given predicate or null if not found.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        TEntity Load<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        TPrimaryKey InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        TEntity InsertOrUpdate<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> InsertOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        TPrimaryKey InsertOrUpdateAndGetId<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        TEntity Update<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        TEntity Update<TEntity>(TPrimaryKey id, Action<TEntity> updateAction) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> UpdateAsync<TEntity>(TPrimaryKey id, Func<TEntity, Task> updateAction) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        int BatchUpdate<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 异步执行：通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        Task<int> BatchUpdateAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        void BatchUpdateDisplayOrder<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        Task BatchUpdateDisplayOrderAsync<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        void Delete<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        Task DeleteAsync<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 批量删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        void Delete<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 异步执行：批量删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        Task DeleteAsync<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 批量删除符合指定条件表达式的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        void BatchDelete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 异步执行：批量删除符合指定条件表达式的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        Task BatchDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// 批量删除指定Id列表中的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="idList">Id列表</param>
        void BatchDelete<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;
        /// <summary>
        /// 异步执行：批量删除指定Id列表中的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="idList">Id列表</param>
        Task BatchDeleteAsync<TEntity>(IEnumerable<TPrimaryKey> idList) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        int Count<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        long LongCount<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<long> LongCountAsync<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        long LongCount<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<long> LongCountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        #endregion
    }
}
