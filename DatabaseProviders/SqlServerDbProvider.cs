using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Database.Base;
using Database.Common.Enumurations;
using Database.Interfaces;
using System.Data.Common;
using Database.Common;

namespace Database.DatabaseProviders
{
    public class SqlServerDbProvider : DbProvider
    {
        public SqlServerDbProvider(IDbConfiguration dbConfigurations)
            : base (dbConfigurations)
        {
        }

        public SqlServerDbProvider(IDbConfiguration dbConfiguration, QueryProvider queryProvider)
            : base (dbConfiguration, queryProvider)
		{
            this.QueryProvider = queryProvider;
		}

        #region DbProvider Implementation

        public override DatabaseType DbType
		{
            get { return DatabaseType.SqlServer; }
		}

        public override IDbConnection GetConnectionObject()
        {
            return new SqlConnection(this.DbConfigurations.DbConnectionString);
        }

        public override IDbConnection GetConnectionObject(string connectionString)
		{
			return new SqlConnection(connectionString);
		}

        public override IDbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public override IDbDataAdapter GetDataAdapter(IDbCommand selectCommand)
        {
            var adapter = new SqlDataAdapter();
            adapter.SelectCommand = (SqlCommand)selectCommand;
            return adapter;
        }

        public override DbCommand GetDataCommand()
        {
            return new SqlCommand();
        }

        public override IDbDataParameter GetDataParameter()
        {
            return new SqlParameter();
        }

        public override IDbDataParameter GetDataParameter(QueryParameter param)
        {
            var dbParam = new SqlParameter();
            dbParam.ParameterName = param.ParameterName;
            dbParam.Value = param.Value;
            dbParam.Direction = param.Direction;
            dbParam.SqlDbType = (SqlDbType)param.ParamType;

            return dbParam;
        }

        #endregion
    }
}
