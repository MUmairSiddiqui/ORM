using System;
using System.Collections.Generic;
using System.Text;
using Database.Base;
using Database.Interfaces;
using Database.Common.Enumurations;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Database.Common;

namespace Database.DatabaseProviders
{
    public class MySqlDbProvider : DbProvider
    {
        public MySqlDbProvider(IDbConfiguration dbConfigurations)
            : base (dbConfigurations)
        {
        }

        public MySqlDbProvider(IDbConfiguration dbConfigurations, QueryProvider queryProvider)
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
            return new MySqlConnection(this.DbConfigurations.DbConnectionString);
        }

        public override IDbConnection GetConnectionObject(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        public override IDbDataAdapter GetDataAdapter(IDbCommand selectCommand)
        {
            var adapter = new MySqlDataAdapter();
            adapter.SelectCommand = (MySqlCommand)selectCommand;
            return adapter;
        }

        public override DbCommand GetDataCommand()
        {
            return new MySqlCommand();
        }

        public override IDbDataParameter GetDataParameter()
        {
            return new MySqlParameter();
        }

        public override IDbDataParameter GetDataParameter(QueryParameter param)
        {
            var dbParam = new MySqlParameter();
            dbParam.ParameterName = param.ParameterName;
            dbParam.Value = param.Value;
            dbParam.Direction = param.Direction;
            dbParam.DbType = param.DbType;

            return dbParam;
        }

        #endregion
    }
}
