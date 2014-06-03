using System;
using System.Collections.Generic;
using System.Text;
using Database.Base;
using Database.Common;

namespace Database.QueryProviders
{
    public class DbQueryProvider //: QueryProvider
    {
        private static DbQueryProvider instance = new DbQueryProvider();
        private IDictionary<string, Query> queries = null;

        private DbQueryProvider()
        { }

        public static DbQueryProvider Instance
        {
            get
            {
                if (instance.queries == null)
                    instance.queries = new Dictionary<string, Query>();

                return instance;
            }
        }

        public Query this[string queryName]
        {
            get 
            {
                if (!this.queries.ContainsKey(queryName))
                    this.queries.Add(queryName, new Query(queryName, queryName,
                        System.Data.CommandType.StoredProcedure, new List<QueryParameter>()));
                else
                    this.queries[queryName].QueryParameters.Clear();

                return this.queries[queryName];
            }
        }
    }
}
