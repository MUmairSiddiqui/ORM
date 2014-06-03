using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Transactions;
using Database.Base;
using Database.Common;
using Database.Common.Enumurations;
using Database.Exceptions;
using Database.Interfaces;
using log4net;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Oracle.DataAccess.Client;
using System.Linq;

namespace Database
{
    public class DbManager
    {
        #region Attributes

        private IDbConfiguration applicationConfiguration;
        private static ILog log = LogManager.GetLogger("DbManager");
        private DbProvider dbProvider;
        private Microsoft.Practices.EnterpriseLibrary.Data.Database db = null;
        private bool loggingEnabled = true;

        #endregion Attributes

        #region Properties

        public DbProvider DbProvider
        {
            get { return dbProvider; }
        }

        public string ConnectionStringName
        {
            get { return applicationConfiguration.DbConnectionStringName; }
        }

        public DatabaseType DbType
        {
            get
            {
                return dbProvider.DbType;
            }
        }

        public bool EnableLogging
        {
            get { return loggingEnabled; }
            set { loggingEnabled = value; }
        }

        #endregion Properties

        #region Constructor

        public DbManager(IDbConfiguration dbConfiguration)
            : this (dbConfiguration, true)
        {            
        }

        public DbManager(IDbConfiguration dbConfiguration, bool enableLogging)
        {
            if (dbConfiguration == null)
                throw new ArgumentNullException("dbConfiguration");

            this.loggingEnabled = enableLogging;
            this.Info("Db manager instantiating");
            this.applicationConfiguration = dbConfiguration;
            this.dbProvider = DbProviderFactory.Get(dbConfiguration);
            this.db = DatabaseFactory.CreateDatabase(dbConfiguration.DbConnectionStringName);
        }

        #endregion Constructor

        #region Public Methods

        #region ADO.NET Objects

        public IDbConnection DbConnection()
        {
            return dbProvider.GetConnectionObject(applicationConfiguration.DbConnectionString);
        }

        public IDbConnection DbConnection(string connectionString)
        {
            return dbProvider.GetConnectionObject(connectionString);
        }

        public DbCommand DbCommand()
        {
            return dbProvider.GetDataCommand();
        }

        public IDbDataAdapter DbAdapter()
        {
            return dbProvider.GetDataAdapter();
        }

        public IDbDataParameter DbParameter()
        {
            return dbProvider.GetDataParameter();
        }

        public IDbDataParameter DbParameter(QueryParameter param)
        {
            return dbProvider.GetDataParameter(param);
        }

        #endregion ADO.NET Objects

        public int ExecuteNonQuery(Query query)
        {
            return ExecuteQueryInternal(query);
        }

        public int ExecuteNonQuery(Query query, out IEnumerable<object> returnValues)
        {
            return ExecuteQueryInternal(query, out returnValues);
        }

        public ReturnTypeWrapper ExecuteNonQueryWithReturnType(Query query)
        {
            return ExecuteQueryInternalWithReturnType(query);
        }

        public object GetScalarValue(Query query)
        {
            return this.ReadToScalar(query);
        }

        public DataSet FillDataSet(Query query)
        {
            //var a = new Oracle.DataAccess.Types.OracleBlob(this.DbConnection());
            return this.FillDataToDataSet(query);
        }

        public ReturnTypeWrapper FillDataSetWithReturnType(Query query)
        {
            return this.FillDataToDataSetWithReturnType(query);
        }

        public DataSet FillDataSet(Query query, out IEnumerable<object> returnValues)
        {
            return this.FillDataToDataSet(query, out returnValues);
        }

        public T ReadEntity<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToSingleObject<T>(query);
        }

        public T ReadEntity<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToSingleObject<T>(query, out returnValues);
        }

        public T ReadEntityWithRetry<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToSingleObjectWithRetry<T>(query);
        }

        public IEnumerable<T> ReadEntityList<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToList<T>(query);
        }

        public IEnumerable<T> ReadEntityList<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToList<T>(query, out returnValues);
        }

        public IEnumerable<T> ReadEntityListWithRetry<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            return ReadDataToListWithRetry<T>(query);
        }

        public IEnumerable<T> FillEntityList<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            return this.FillDataToList<T>(query);
        }

        public IEnumerable<T> FillEntityList<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            return this.FillDataToList<T>(query, out returnValues);
        }

        public int ExecuteTransaction(IEnumerable<Query> queries)
        {
            return this.ExecuteTransactionInternal(queries);
        }

        public int ExecuteTransaction(IEnumerable<Query> queries, out Dictionary<string, IEnumerable<object>> returnValues)
        {
            return this.ExecuteTransactionInternal(queries, out returnValues);
        }

        #endregion Public Methods

        #region Private Methods

        private object ReadToScalar(Query query)
        {
            if (query != null)
            {
                this.Info(query.ToString());

                //using (DbConnection connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        //command.Connection = connection;
                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        var obj = db.ExecuteScalar(command);

                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in ReadToScalar [Query: " + query.ToString() + "]", ex);
                    throw;
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private DataSet FillDataToDataSet(Query query)
        {
            if (query != null)
            {
                this.Info(query.ToString());

                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        var ds = db.ExecuteDataSet(command);
                        db.ExecuteDataSet(command);

                        return ds;
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in FillDataToDataSet [Query: " + query.ToString() + "]", ex);
                    throw;
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private DataSet FillDataToDataSet(Query query, out IEnumerable<object> returnValues)
        {
            if (query != null)
            {
                this.Info(query.ToString());
                var values = new List<object>();

                //var returnType = new object();

                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        var result = db.ExecuteDataSet(command);

                        query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                           .ForEach(parameter => values.Add(command.Parameters[parameter.ParameterName].Value));

                        returnValues = values;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in FillDataToDataSet [Query: " + query.ToString() + "]", ex);
                    throw;
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private ReturnTypeWrapper FillDataToDataSetWithReturnType(Query query)
        {
            if (query != null)
            {
                this.Info(query.ToString());
                var wrapper = new ReturnTypeWrapper();

                //var returnValues = new List<object>();

                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        wrapper.Data = db.ExecuteDataSet(command);

                        query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                           .ForEach(parameter => wrapper.ReturnValues.Add(command.Parameters[parameter.ParameterName].Value.ToString()));

                        //wrapper.ReturnValues = returnValues;
                        return wrapper;
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in FillDataToDataSet [Query: " + query.ToString() + "]", ex);
                    throw;
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private int ExecuteQueryInternal(Query query)
        {
            //Query query = _provider.QueryProvider[queryName];

            if (query != null)
            {
                this.Info(query.ToString());

                //int retryCount = 0;
                //do
                //{
                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText;

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        var obj = db.ExecuteNonQuery(command);

                        return obj;
                        //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in ExecuteQueryInternal [Query: " + query.ToString() + "]", ex);
                    throw;

                    //Thread.Sleep(_applicationConfiguration.DbConnectRetryDelay);
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
                //} while (retryCount++ < _applicationConfiguration.DbConnectRetryCount);
            }
            else
                throw new ArgumentNullException("Query");
        }

        private int ExecuteQueryInternal(Query query, out IEnumerable<object> returnValues)
        {
            //Query query = _provider.QueryProvider[queryName];

            if (query != null)
            {
                this.Info(query.ToString());
                var rows = -1;
                var values = new List<object>();

                //int retryCount = 0;
                //do
                //{
                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText;

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        rows = db.ExecuteNonQuery(command);

                        query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                           .ForEach(parameter => values.Add(command.Parameters[parameter.ParameterName].Value));

                        returnValues = values;
                        return rows;
                        //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in ExecuteQueryInternal [Query: " + query.ToString() + "]", ex);
                    throw;

                    //Thread.Sleep(_applicationConfiguration.DbConnectRetryDelay);
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
                //} while (retryCount++ < _applicationConfiguration.DbConnectRetryCount);
            }
            else
                throw new ArgumentNullException("Query");
        }

        private ReturnTypeWrapper ExecuteQueryInternalWithReturnType(Query query)
        {
            //Query query = _provider.QueryProvider[queryName];

            if (query != null)
            {
                this.Info(query.ToString());

                //var returnValues = new List<object>();
                var wrapper = new ReturnTypeWrapper();

                //int retryCount = 0;
                //do
                //{
                //using (var connection = _db.CreateConnection())
                //{
                try
                {
                    using (var command = this.DbCommand())
                    {
                        //command.Connection = connection;

                        foreach (var p in query.QueryParameters)
                            command.Parameters.Add(this.DbParameter(p));

                        command.CommandType = query.QueryType;
                        command.CommandText = query.QueryText;

                        //command.CommandTimeout = connection.ConnectionTimeout;

                        //connection.Open();
                        wrapper.effectedRows = db.ExecuteNonQuery(command);

                        query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                           .ForEach(parameter => wrapper.ReturnValues.Add(command.Parameters[parameter.ParameterName].Value.ToString()));

                        //wrapper.ReturnValues = returnValues;
                        return wrapper; //rows;
                        //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Exception in ExecuteQueryInternal [Query: " + query.ToString() + "]", ex);
                    throw;

                    //Thread.Sleep(_applicationConfiguration.DbConnectRetryDelay);
                }
                finally
                {
                    //if (connection != null)
                    //    if (connection.State != ConnectionState.Closed)
                    //        connection.Close();
                }

                //}
                //} while (retryCount++ < _applicationConfiguration.DbConnectRetryCount);
            }
            else
                throw new ArgumentNullException("Query");
        }

        private int ExecuteTransactionInternal(IEnumerable<Query> queries)
        {
            if (queries == null)
                throw new ArgumentNullException("Queries");

            DbCommand command = null;

            //DbTransaction transaction = null;
            var rowsAffected = -1;

            this.Info("Starting transactoins for " + queries.Count() + " queries");

            //using (var connection = _db.CreateConnection())
            //{
            try
            {
                using (var transaction = new TransactionScope())
                {
                    //connection.Open();
                    //transaction = connection.BeginTransaction();

                    foreach (var query in queries)
                    {
                        this.Info(query.ToString());

                        using (command = this.DbCommand())
                        {
                            //command.Connection = connection;
                            //command.Transaction = transaction;

                            foreach (var p in query.QueryParameters)
                                command.Parameters.Add(this.DbParameter(p));

                            command.CommandType = query.QueryType;
                            command.CommandText = query.QueryText;

                            //command.CommandTimeout = connection.ConnectionTimeout;

                            rowsAffected += db.ExecuteNonQuery(command);

                            //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                        }
                    }
                    transaction.Complete();
                }

                //transaction.Commit();
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                this.Error("Exception in ExecuteTransactionInternal", ex);
                throw;

                //Thread.Sleep(_applicationConfiguration.DbConnectRetryDelay);
            }
            finally
            {
                //if (connection != null)
                //    if (connection.State != ConnectionState.Closed)
                //        connection.Close();
            }

            return rowsAffected;
        }

        private int ExecuteTransactionInternal(IEnumerable<Query> queries, out Dictionary<string, IEnumerable<object>> returnValues)
        {
            if (queries == null)
                throw new ArgumentNullException("Queries");

            DbCommand command = null;

            //DbTransaction transaction = null;
            var rowsAffected = -1;
            var values = new Dictionary<string, IEnumerable<object>>();
            List<object> objects = null;

            this.Info("Starting transactoins for " + queries.Count() + " queries");

            try
            {
                using (var transaction = new TransactionScope())
                {
                    foreach (var query in queries)
                    {
                        this.Info(query.ToString());

                        using (command = this.DbCommand())
                        {
                            //command.Connection = connection;
                            //command.Transaction = transaction;

                            foreach (var p in query.QueryParameters)
                                command.Parameters.Add(this.DbParameter(p));

                            command.CommandType = query.QueryType;
                            command.CommandText = query.QueryText;

                            //command.CommandTimeout = connection.ConnectionTimeout;

                            rowsAffected += db.ExecuteNonQuery(command);

                            objects = new List<object>();
                            query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                                   && parameter.ParamType != QueryParameterType.RefCursor)
                               .ForEach(parameter => objects.Add(command.Parameters[parameter.ParameterName].Value));

                            values.Add(query.QueryName ?? query.QueryText, objects);

                            //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                        }
                    }
                    transaction.Complete();
                }

                returnValues = values;
            }
            catch (Exception ex)
            {
                this.Error("Exception in ExecuteTransactionInternal", ex);
                throw;

                //Thread.Sleep(_applicationConfiguration.DbConnectRetryDelay);
            }
            finally
            {
                //if (connection != null)
                //    if (connection.State != ConnectionState.Closed)
                //        connection.Close();
            }

            //}

            return rowsAffected;
        }

        private T ReadDataToSingleObjectWithRetry<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            //Query query = _provider.QueryProvider[queryName];
            if (query != null)
            {
                this.Info(query.ToString());

                var retryCount = 0;
                do
                {
                    try
                    {
                        return ReadDataToSingleObject<T>(query);
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToSingleObjectWithRetry [Query: " + query.ToString() + "] [Attempt: " + retryCount + "]", ex);
                        Thread.Sleep(applicationConfiguration.DbConnectRetryDelay);
                    }
                } while (retryCount++ < applicationConfiguration.DbConnectRetryCount);

                throw new DbRetryCountBreachException("Retry count breached [Attempts: " + retryCount + "]");
            }
            else
                throw new ArgumentNullException("Query");
        }

        private T ReadDataToSingleObject<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());

                T entity = null;

                //using (var connection = _db.CreateConnection())
                //{
                using (var command = this.DbCommand())
                {
                    //command.Connection = connection;

                    foreach (var p in query.QueryParameters)
                        command.Parameters.Add(this.DbParameter(p));

                    command.CommandType = query.QueryType;
                    command.CommandText = query.QueryText;

                    //command.CommandTimeout = connection.ConnectionTimeout;

                    try
                    {
                        //connection.Open();
                        using (var reader = db.ExecuteReader(command))
                        {
                            if (reader.Read())
                            {
                                entity = new T();
                                //entity.ConstructByColumnBinding(reader);
                                entity.ConstructFromReader(reader);

                                //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                            }

                            //else
                            //throw new Exception("Cannot read from datasource [Query : " + query.ToString() + "]");
                            return entity;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToSingleObject [Query: " + query.ToString() + "]", ex);
                        throw;
                    }
                    finally
                    {
                        //if (connection != null)
                        //    if (connection.State != ConnectionState.Closed)
                        //        connection.Close();
                    }
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private T ReadDataToSingleObject<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());
                T entity = null;
                var values = new List<object>();

                //using (var connection = _db.CreateConnection())
                //{
                using (var command = this.DbCommand())
                {
                    //command.Connection = connection;

                    foreach (var p in query.QueryParameters)
                        command.Parameters.Add(this.DbParameter(p));

                    command.CommandType = query.QueryType;
                    command.CommandText = query.QueryText;

                    //command.CommandTimeout = connection.ConnectionTimeout;

                    try
                    {
                        //connection.Open();
                        using (var reader = db.ExecuteReader(command))
                        {
                            if (reader.Read())
                            {
                                entity = new T();
                                //entity.ConstructByColumnBinding(reader);
                                entity.ConstructFromReader(reader);

                                //OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                            }

                            //else
                            //  throw new Exception("Cannot read from datasource [Query : " + query.ToString() + "]");

                            query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                                .ForEach(parameter => values.Add(command.Parameters[parameter.ParameterName].Value));

                            returnValues = values;
                            return entity;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToSingleObject [Query: " + query.ToString() + "]", ex);
                        throw;
                    }
                    finally
                    {
                        //if (connection != null)
                        //    if (connection.State != ConnectionState.Closed)
                        //        connection.Close();
                    }
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private IEnumerable<T> ReadDataToListWithRetry<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            //Query query = _provider.QueryProvider[queryName];
            if (query != null)
            {
                this.Info(query.ToString());

                var retryCount = 0;
                do
                {
                    try
                    {
                        //using (DbConnection connection = _db.CreateConnection())
                        return ReadDataToList<T>(query);
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToListWithRetry [Query: " + query.ToString() + "] [Attempt: " + retryCount + "]", ex);
                        Thread.Sleep(applicationConfiguration.DbConnectRetryDelay);
                    }
                } while (retryCount++ < applicationConfiguration.DbConnectRetryCount);

                throw new DbRetryCountBreachException("Retry count breached");
            }
            else
                throw new ArgumentNullException("Query");
        }

        private IEnumerable<T> ReadDataToList<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());

                //using (var connection = _db.CreateConnection())
                //{
                using (var command = this.DbCommand())
                {
                    //command.Connection = connection;

                    foreach (var p in query.QueryParameters)
                        command.Parameters.Add(this.DbParameter(p));

                    command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);
                    command.CommandType = query.QueryType;

                    //command.CommandTimeout = connection.ConnectionTimeout;

                    try
                    {
                        //connection.Open();
                        using (var reader = db.ExecuteReader(command))
                        {
                            var entities = new List<T>();//.ConstructByColumnBinding(reader);

                            while (reader.Read())
                            {
                                var entity = new T();
                                //entity.ConstructByColumnBinding(reader);
                                entity.ConstructFromReader(reader);
                                entities.Add(entity);
                            }

                            ////OnDbOperationComplete(DateTime.Now.Ticks - startTimeTicks);
                            return entities;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToList [Query: " + query.ToString() + "]", ex);
                        throw;
                    }
                    finally
                    {
                        //if (connection != null)
                        //    if (connection.State != ConnectionState.Closed)
                        //        connection.Close();
                    }
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private IEnumerable<T> ReadDataToList<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());
                var values = new List<object>();
                var entities = new List<T>();

                //using (var connection = _db.CreateConnection())
                //{
                using (var command = this.DbCommand())
                {
                    //command.Connection = connection;

                    foreach (var p in query.QueryParameters)
                        command.Parameters.Add(this.DbParameter(p));

                    command.CommandText = query.QueryText; //(formatStrings == null) ? query.QueryText : String.Format(query.QueryText, formatStrings);
                    command.CommandType = query.QueryType;

                    //command.CommandTimeout = connection.ConnectionTimeout;

                    try
                    {
                        //connection.Open();
                        using (var reader = db.ExecuteReader(command))
                        {
                            //entities = entities.ConstructByColumnBinding(reader);

                            while (reader.Read())
                            {
                                var entity = new T();
                                //entity.ConstructByColumnBinding(reader);
                                entity.ConstructFromReader(reader);
                                entities.Add(entity);
                            }

                            query.QueryParameters.FindAll(parameter => parameter.Direction != ParameterDirection.Input
                               && parameter.ParamType != QueryParameterType.RefCursor)
                               .ForEach(parameter => values.Add(command.Parameters[parameter.ParameterName].Value));

                            returnValues = values;
                            return entities;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error("Exception in ReadDataToList [Query: " + query.ToString() + "]", ex);
                        throw;
                    }
                    finally
                    {
                        //if (connection != null)
                        //    if (connection.State != ConnectionState.Closed)
                        //        connection.Close();
                    }
                }

                //}
            }
            else
                throw new ArgumentNullException("Query");
        }

        private IEnumerable<T> FillDataToList<T>(Query query)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());

                var data = this.FillDataToDataSet(query);

                if (data != null && data.Tables.Count > 0)
                {
                    var entities = new List<T>();
                    T entity = null;

                    foreach (DataRow row in data.Tables[0].Rows)
                    {
                        entity = new T();
                        entity.ConstructFromDataRow(row);
                        entities.Add(entity);
                    }

                    return entities;
                }

                return null;
            }
            else
                throw new ArgumentNullException("Query");
        }

        private IEnumerable<T> FillDataToList<T>(Query query, out IEnumerable<object> returnValues)
            where T : class, IConstructFromDataSource, new()
        {
            if (query != null)
            {
                this.Info(query.ToString());

                var data = this.FillDataToDataSet(query, out returnValues);

                if (data != null && data.Tables.Count > 0)
                {
                    var entities = new List<T>();
                    T entity = null;

                    foreach (DataRow row in data.Tables[0].Rows)
                    {
                        entity = new T();
                        entity.ConstructFromDataRow(row);
                        entities.Add(entity);
                    }

                    return entities;
                }

                return null;
            }
            else
                throw new ArgumentNullException("Query");
        }

        #region Log Controllers

        private void Debug(string message)
        {
            if (this.loggingEnabled && log.IsDebugEnabled)
                log.Debug(message);
        }

        private void Info(string message)
        {
            if (this.loggingEnabled && log.IsInfoEnabled)
                log.Info(message);
        }

        private void Warn(string message)
        {
            if (this.loggingEnabled && log.IsWarnEnabled)
                log.Warn(message);
        }

        private void Error(string message)
        {
            if (this.loggingEnabled && log.IsErrorEnabled)
                log.Error(message);
        }

        private void Error(string message, Exception ex)
        {
            if (this.loggingEnabled && log.IsErrorEnabled)
                log.Error(message, ex);
        }

        private void Fatal(string message)
        {
            if (this.loggingEnabled && log.IsFatalEnabled)
                log.Fatal(message);
        }

        private void Fatal(string message, Exception ex)
        {
            if (this.loggingEnabled && log.IsFatalEnabled)
                log.Fatal(message, ex);
        }

        #endregion Log Controllers

        #endregion Private Methods
    }
}