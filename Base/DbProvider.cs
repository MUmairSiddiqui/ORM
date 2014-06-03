using System;
using System.Collections.Generic;
using System.Text;
using Database.Common.Enumurations;
using System.Data;
using Database.Interfaces;
using System.Data.Common;
using Database.Common;

namespace Database.Base
{
    public abstract class DbProvider
    {
        #region Private Attribues

        private QueryProvider queryProvider;
        private IDbConfiguration defaultDbConfigurations;

        #endregion

        #region Constructors

        public DbProvider(IDbConfiguration dbConfigurations)
        {
            this.defaultDbConfigurations = dbConfigurations;
        }

        public DbProvider(IDbConfiguration dbConfigurations, QueryProvider queryProvider)
        {
            this.defaultDbConfigurations = dbConfigurations;            
        }

        #endregion

        public abstract DatabaseType DbType { get; }
        public virtual IDbConfiguration DbConfigurations
        {
            get { return this.defaultDbConfigurations; }
        }

        public virtual QueryProvider QueryProvider
        {
            get { return queryProvider; }
            protected set { queryProvider = value; }
        }
        
        public abstract IDbConnection GetConnectionObject();
        public abstract IDbConnection GetConnectionObject(string connectionString);
        public abstract IDbDataAdapter GetDataAdapter();
        public abstract IDbDataAdapter GetDataAdapter(IDbCommand selectCommand);
        public abstract DbCommand GetDataCommand();
        public abstract IDbDataParameter GetDataParameter();
        public abstract IDbDataParameter GetDataParameter(QueryParameter param);
    }
}
