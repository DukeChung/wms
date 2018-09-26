using Abp.Application.Services.Dto;
using Abp.Dependency;
using Dapper;
using FortuneLab.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Repositories.Dapper
{
    public abstract class DapperRepositoryBase<TDbConnProvider, TPrimaryKey> : IDapperRepository
        where TDbConnProvider : IDbConnProvider
    {
        private string _nameOrConnectionString;
        private ILogger _logger = LogManager.GetLogger("FortuneLab.DapperRepository");

        public DapperRepositoryBase()
        {
            var dbConnConfig = Activator.CreateInstance<TDbConnProvider>();
            this._nameOrConnectionString = dbConnConfig.NameOrConnectionString;
            //this._logger = IocManager.Instance.Resolve<ILogger>().CreateChildLogger("FortuneLab.DapperSql");
        }

        #region IRepository Members

        //public T GetById<T>(TPrimaryKey entityId) where T : class, Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>
        //{
        //    return internalRepository.Get<T>(entityId);
        //}

        //public TPrimaryKey Insert<T>(T model) where T : class, Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>
        //{
        //    //model.SysId = Guid.NewGuid();
        //    //_unitOfWork.RegisterAdded(model, this);
        //    //return model.SysId;
        //    internalRepository.Insert(model);
        //    return model.SysId;

        //}

        //public void Update<T>(T model) where T : class, Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>
        //{
        //    //_unitOfWork.RegisterChanged(model, this);
        //    internalRepository.Update(model);
        //}

        //public void Delete<T>(TPrimaryKey id) where T : class, Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>
        //{
        //    //_unitOfWork.RegisterRemoved(GetById<T>(id), this);
        //    internalRepository.Delete<T>(id);
        //}

        /////// <summary>
        /////// 此方法会在工作单元执行
        /////// </summary>
        /////// <param name="command"></param>
        ////public void ExecuteNoQueryCommand(IDbCommand command)
        ////{
        ////    _unitOfWork.RegisterNoQueryCommand(command, this);
        ////}


        //protected IList<T> FindAll<T>() where T : class, Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>
        //{
        //    return internalRepository.GetAll<T>().ToList();
        //}

        #endregion

        #region 供服务层调用的数据库操作方法
        /// <summary>
        /// 执行无返回值的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="outerTrans"></param>
        /// <returns></returns>
        protected int ExecuteNoQuery(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            return ExecuteInternal(sql, param, outerTrans);
        }

        protected Task<int> ExecuteNoQueryAsync(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            return ExecuteInternalAsync(sql, param, outerTrans);
        }

        /// <summary>
        /// 获取某个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="outerTrans"></param>
        /// <returns></returns>
        protected T Get<T>(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            return GetInternal<T>(sql, param, outerTrans);
        }

        /// <summary>
        /// 获取某一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="outerTrans"></param>
        /// <returns></returns>
        protected T Scalar<T>(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.ExecuteScalar<T>(sql, param, outerTrans);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }

        /// <summary>
        /// 执行存储过程(Store Procedure)
        /// </summary>
        /// <param name="storeProcedureName">SP名称</param>
        /// <param name="param"></param>
        /// <param name="outerTrans"></param>
        /// <returns></returns>
        protected int ExecuteStoreProcedure(string storeProcedureName, DynamicParameters param = null, IDbTransaction outerTrans = null)
        {
            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.Execute(storeProcedureName, param, outerTrans, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }

        protected IEnumerable<T> ExecuteStoreProcedure<T>(string storeProcedureName, DynamicParameters param = null, IDbTransaction outerTrans = null)
        {
            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.Query<T>(storeProcedureName, param, outerTrans, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        protected virtual Page<T> GetPageList<T>(int pageIndex, int pageSize, string selectSql, string tableName, string whereSql, string orderBy, object param = null, IDbTransaction outerTrans = null)
        {
            if (string.IsNullOrEmpty(whereSql))
                whereSql = " 1 = 1 ";

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                throw new Exception("OrderBy 参数是必须的");
            }

            var pagerSql =
                string.Format(
                    @"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {5}) RowIndex, {2} from {3} where {4}) p_paged WHERE RowIndex > {0} AND RowIndex <= {1}",
                    (pageIndex - 1) * pageSize, pageIndex * pageSize, selectSql, tableName, whereSql, orderBy);
            var countSql = string.Format(@"select count(1) from {0} where {1}", tableName, whereSql);

            var result = new Page<T> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
            return GetPagerInternal<T>(pagerSql, countSql, param, pageIndex, pageSize, outerTrans);
        }

        protected virtual Page<T> GetPageList<T>(int pageIndex, int pageSize, string pageDataSql, string totalSql, string whereSql, object param = null, IDbTransaction outerTrans = null)
        {
            int rowFrom = (pageIndex - 1) * pageSize;
            int rowEnd = pageIndex * pageSize;
            var pagerSql = pageDataSql.Replace("{RowFrom}", rowFrom.ToString()).Replace("{RowEnd}", rowEnd.ToString()).Replace("{Where}", whereSql);
            var countSql = totalSql.Replace("{Where}", whereSql);

            var result = new Page<T> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
            return GetPagerInternal<T>(pagerSql, countSql, param, pageIndex, pageSize, outerTrans);
        }

        protected virtual IEnumerable<T> GetList<T>(string selectSql, string tableName, string whereSql, string orderBy = null, object param = null, IDbTransaction outerTrans = null)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT {0} from {1} ", selectSql, tableName);
            if (!string.IsNullOrWhiteSpace(whereSql))
            {
                sbSql.AppendFormat(@" where {0}", whereSql);
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                sbSql.AppendFormat(@" order by {0}", orderBy);
            }
            return GetListInternal<T>(sbSql.ToString(), param, outerTrans);
        }

        protected virtual IEnumerable<T> GetList<T>(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            return GetListInternal<T>(sql, param, outerTrans);
        }

        #endregion

        #region Warpped Dapper Internal Method and Logging
        /// <summary>
        /// 通过当前的 outerTrans执行当前的Action, 
        /// Action本身需要一个Connection与一个Transaction
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="outerTrans"></param>
        /// <returns></returns>
        private int ExecuteInternal(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.Execute(sql, param, outerTrans);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }

        private Task<int> ExecuteInternalAsync(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.ExecuteAsync(sql, param, outerTrans);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }

        private T GetInternal<T>(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.Query<T>(sql, param, outerTrans).FirstOrDefault();
            }
            catch (Exception)
            {
                //WriteLog("sql err msg", ex.ToString());
                //if (cmd != null)
                //    WriteLog("sql err", SqlCommandHelper.SqlCommandToString(cmd));
                throw;
            }
            finally
            {
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }
        private IEnumerable<T> GetListInternal<T>(string sql, object param = null, IDbTransaction outerTrans = null)
        {
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                return conn.Query<T>(sql, param, outerTrans);
            }
            catch (Exception)
            {
                //WriteLog("sql err msg", ex.ToString());
                //if (cmd != null)
                //    WriteLog("sql err", SqlCommandHelper.SqlCommandToString(cmd));
                throw;
            }
            finally
            {
                //cmd.Dispose();
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }
        private Page<T> GetPagerInternal<T>(string pagerSql, string countSql, object param, int pageIndex, int pageSize, IDbTransaction outerTrans = null)
        {
            var sql = string.Format("{0} {1}", pagerSql, countSql);
            WriteLog(sql, param);

            bool isTrans;
            IDbConnection conn = null;
            GetConnection(outerTrans, out conn, out isTrans);

            try
            {
                DbConnectionManager.Instance.SafeOpenConnection(conn);
                var result = new Page<T> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
                using (var multi = conn.QueryMultiple(sql, param, outerTrans))
                {
                    result.Records = multi.Read<T>().ToList();
                    result.Paging.Total = multi.Read<int>().Single();
                }
                return result;
            }
            catch (Exception)
            {
                //WriteLog("sql err msg", ex.ToString());
                //if (cmd != null)
                //    WriteLog("sql err", SqlCommandHelper.SqlCommandToString(cmd));
                throw;
            }
            finally
            {
                //cmd.Dispose();
                if (!isTrans)
                    DbConnectionManager.Instance.SafeCloseConnection(conn);
            }
        }
        private void GetConnection(IDbTransaction trans, out IDbConnection conn, out bool isTransaction)
        {
            if (trans == null)
            {
                conn = DbConnectionManager.Instance.GetConnection(this._nameOrConnectionString);
                isTransaction = false;
            }
            else
            {
                conn = trans.Connection;
                isTransaction = true;
            }
        }

        /// <summary>
        /// 输出执行的SQL脚本到ASP.NET Trace
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        protected virtual void WriteLog(string sql, object parms)
        {
            _logger.Debug(string.Format("SQL: {0} Parms: {1}", sql, JsonConvert.SerializeObject(parms)));
        }
        #endregion
    }


}
