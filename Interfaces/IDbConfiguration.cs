using System;
using System.Collections.Generic;
using System.Text;
using Database.Common.Enumurations;

namespace Database.Interfaces
{
    public interface IDbConfiguration
    {
        DatabaseType DbType { get; }
        string DbConnectionStringName { get; }
        string DbConnectionString { get; }
        int DbConnectRetryDelay { get; set; }
        int DbConnectRetryCount { get; set; }
    }    
}
