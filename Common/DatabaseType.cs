using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Common.Enumurations
{
    public enum DatabaseType
    {
        Oracle = 0,
        SqlServer = 1,
        MySql = 2
    }

    public enum QueryType
    {
        StoredProcedure = 0,
        Inline = 1
    }    
}
