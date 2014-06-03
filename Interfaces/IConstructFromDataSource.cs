using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
//ColumnAttribute
namespace Database.Interfaces
{
    public interface IConstructFromDataSource
    {
        bool ConstructFromReader(IDataReader dataReader);
        bool ConstructFromDataRow(DataRow dataRow);
        bool ConstructFromDataTable(DataTable dataTable);
        bool ConstructFromDataSet(DataSet dataSet);
    }
}
