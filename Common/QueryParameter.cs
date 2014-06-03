using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace Database.Common
{
    [DataContract]
    public class QueryParameter : IDataParameter
    {
        [DataMember]
        public string ParameterName {get;set;}
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public QueryParameterType ParamType { get; set; }
        [DataMember]
        public DbType DbType { get; set; }
        [DataMember]
        public ParameterDirection Direction { get; set; }
        [DataMember]
        public bool IsNullable { get; set; }
        [DataMember]
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }

        public QueryParameter()
        {
            this.SourceVersion = DataRowVersion.Original;
        }

        public QueryParameter(string name)
            : this ()
        {
            ParameterName = name;
        }

        public QueryParameter(string name, object value)
        {
            ParameterName = name;
            Value = value;
        }

        //public CustomDbParameter(string name, object value, DbType type,
        //    ParameterDirection direction = ParameterDirection.Input)
        //{
        //    ParameterName = name;
        //    Value = value;
        //    DbType = type;
        //    Direction = direction;
        //}

        public QueryParameter(string name, object value, QueryParameterType type,
            ParameterDirection direction = ParameterDirection.Input)
        {
            ParameterName = name;
            Value = value;
            ParamType = type;
            Direction = direction;
        }        
    }
}
