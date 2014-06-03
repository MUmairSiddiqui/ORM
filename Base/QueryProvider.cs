using System;
using System.Collections.Generic;
using System.Text;
using Database.Common;

namespace Database.Base
{
    public abstract class QueryProvider
    {
        public abstract Query this[string queryName]
        {
            get;
        }
    }    
}
