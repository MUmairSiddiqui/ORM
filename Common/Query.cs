using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Database.Common.Enumurations;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;

namespace Database.Common
{
    //[Serializable]
    //[XmlRoot(ElementName = "Query", IsNullable = false)]
    [DataContract]
    public class Query
    {
        private string queryName = string.Empty;
        private string queryText = string.Empty;
        private IEnumerable<QueryParameter> parameters;
        private CommandType type;

        public Query()
        {
            parameters = new List<QueryParameter>();
        }

        public Query(string name, string query, CommandType queryType)
        {
            queryName = name;
            queryText = query;
            type = queryType;
            parameters = new List<QueryParameter>();
        }

        public Query(string name, string query, CommandType queryType, IEnumerable<QueryParameter> queryParameters)
        {
            queryName = name;
            queryText = query;
            type = queryType;
            parameters = queryParameters;
        }

        //[XmlElement(ElementName = "Text")]
        [DataMember]
        public string QueryName
        {
            get { return queryName; }
            set { queryName = value; }
        }

        //[XmlAttribute(AttributeName = "Type")]
        [DataMember]
        public CommandType QueryType
        {
            get { return type; }
            set { type = value; }
        }

        //[XmlElement(ElementName = "Text")]
        [DataMember]
        public string QueryText
        {
            get { return queryText; }
            set { queryText = value; }
        }

        //[XmlElementAttribute(ElementName="Parameters")]
        [DataMember]
        public List<QueryParameter> QueryParameters
        {
            get { return parameters.ToList<QueryParameter>(); }
            set { parameters = value; }
        }

        public override string ToString()
        {
            var value = new StringBuilder("[Name: " + this.queryName + "] [Query: " + this.queryText + "]");

            foreach (var param in this.parameters)
            {
                if (param.ParamType == QueryParameterType.Clob || param.ParamType == QueryParameterType.Blob
                    || param.ParamType == QueryParameterType.NClob)
                    value.Append("[Parameter: Name| ").Append(param.ParameterName)
                        .Append(" | Type|").Append(param.DbType.ToString())
                        .Append(" | Direction|").Append(param.Direction.ToString())
                        .Append("]");
                else
                    value.Append("[Parameter: Name| ").Append(param.ParameterName)
                        .Append(" | Value| ").Append(param.Value)
                        .Append(" | Type|").Append(param.DbType.ToString())
                        .Append(" | Direction|").Append(param.Direction.ToString())
                        .Append("]");
            }

            return value.ToString();
        }
    }
}
