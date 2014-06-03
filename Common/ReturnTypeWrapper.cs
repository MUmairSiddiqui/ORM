using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using Database.Interfaces;

namespace Database.Common
{
    [DataContract]
    [KnownType(typeof(object[]))]        
    public class ReturnTypeWrapper
    {
        [DataMember]
        public int effectedRows { get; set; }

        [DataMember]
        public List<object> ReturnValues { get; set; }

        [DataMember]
        public DataSet Data { get; set; }

        [DataMember]
        public DataTable Table
        {
            get
            {
                if (this.Data != null && this.Data.Tables.Count > 0)
                    return this.Data.Tables[0];

                return null;
            }
        }

        public ReturnTypeWrapper()
        {
            this.ReturnValues = new List<object>();
        }
    }
}
