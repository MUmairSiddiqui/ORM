using System;
using System.Collections.Generic;
using System.Text;
using Database.Interfaces;
using Database.Common.Enumurations;
using System.Configuration;

namespace Database
{
    public class DefaultDbConfiguration : IDbConfiguration
    {
        #region Attributes

        private DatabaseType dbType = DatabaseType.Oracle;
        private string dbConnectionStringName = string.Empty;
        private string dbConnectionString = string.Empty;
        private int dbConnectRetryDelay = 100;
        private int dbConnectRetryCount = 3;

        #endregion

        #region Cosntructor

        public DefaultDbConfiguration(string dbConnStringName, DatabaseType dbType)
        {
            this.dbConnectionStringName = dbConnStringName;
            this.dbConnectionString = ConfigurationManager.ConnectionStrings[this.dbConnectionStringName].ConnectionString;
            this.dbType = dbType;
        }

        public DefaultDbConfiguration(string connectionSring, DatabaseType dbType, int dbMaxConnectRetries, int dbReconnectDelay)
        {
            this.dbType = dbType;
            this.dbConnectionString = connectionSring;
            this.dbConnectRetryCount = dbMaxConnectRetries;
            this.dbConnectRetryDelay = dbReconnectDelay;
        }

        #endregion

        #region IDbConfiguration Members

        public DatabaseType DbType
        {
            get { return this.dbType; }
        }

        public string DbConnectionStringName 
        { 
            get { return this.dbConnectionStringName; } 
        }

        public string DbConnectionString
        {
            get { return this.dbConnectionString; }
        }
        
        public int DbConnectRetryDelay
        {
            get
            {
                return this.dbConnectRetryDelay;
            }
            set
            {
                this.dbConnectRetryDelay = value;
            }
        }

        public int DbConnectRetryCount
        {
            get
            {
                return this.dbConnectRetryCount;
            }
            set
            {
                this.dbConnectRetryCount = value;
            }
        }

        #endregion
    }
}
