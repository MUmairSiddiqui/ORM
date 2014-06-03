using System;
using System.Collections.Generic;
using System.Text;
using Database.Base;
using Database.Common.Enumurations;
using Database.DatabaseProviders;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using Database.Interfaces;
using Database.Exceptions;

namespace Database
{
    public static class DbProviderFactory
    {
        public static DbProvider Get(IDbConfiguration dbConfiguration)
        {            
            var dbType = dbConfiguration.DbType;

            switch (dbType)
            {
                case DatabaseType.Oracle:
                    return new OracleDbProvider(dbConfiguration);
                case DatabaseType.SqlServer:
                    return new SqlServerDbProvider(dbConfiguration);
                case DatabaseType.MySql:
                    return new MySqlDbProvider(dbConfiguration);
                default:
                    throw new UnknowDatabaseTypeException("Database type not defined.");
            }
        }        
    }
}
