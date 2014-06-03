using System;
using System.Collections.Generic;
using System.Text;
using Database.Base;
using Database.Common.Enumurations;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.Common;
using Database.Interfaces;
using Database.Common;

namespace Database.DatabaseProviders
{
    public class OracleDbProvider : DbProvider
    {
        public OracleDbProvider(IDbConfiguration dbConfigurations)
            : base (dbConfigurations)
        {
        }

        public OracleDbProvider(IDbConfiguration dbConfigurations, QueryProvider queryProvider)
            : base (dbConfigurations, queryProvider)
        {
            this.QueryProvider = queryProvider;
        }
        
        #region DbProvider Implementation

        public override DatabaseType DbType
		{
            get { return DatabaseType.Oracle; }
		}

        public override IDbConnection GetConnectionObject()
        {
            return new OracleConnection(this.DbConfigurations.DbConnectionString);
        }

        public override IDbConnection GetConnectionObject(string connectionString)
		{
			return new OracleConnection(connectionString);
		}

        public override IDbDataAdapter GetDataAdapter()
        {
            return new OracleDataAdapter();
        }

        public override IDbDataAdapter GetDataAdapter(IDbCommand selectCommand)
        {
            var adapter = new OracleDataAdapter();
            adapter.SelectCommand = (OracleCommand)selectCommand;
            return adapter;
        }

        public override DbCommand GetDataCommand()
        {
            return new OracleCommand();
        }

        public override IDbDataParameter GetDataParameter()
        {
            return new OracleParameter();
        }

        public override IDbDataParameter GetDataParameter(QueryParameter param)
        {
            var dbParam = new OracleParameter();
            dbParam.ParameterName = param.ParameterName;
            dbParam.Value = param.Value;
            dbParam.Direction = param.Direction;            
            dbParam.OracleDbType = (OracleDbType)param.ParamType;

            return dbParam;
        }

        #endregion
    }
}
