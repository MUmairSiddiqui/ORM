using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Database.Interfaces;
using System.Data;
using System.Reflection;

namespace Database.Common
{
    public static class EntityExtensions
    {
        public static void ConstructByColumnBinding<T>(this T entity, IDataReader dataReader)
            where T : class, IConstructFromColumnBinding
        {
            if (dataReader == null || entity == null)
                return;

            //var bindedProperties = entity.GetType().GetProperties()
            //        .Where(property => property.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0 && property.CanWrite).AsParallel();

            var bindedProperties = (from property in entity.GetType().GetProperties()
                                    where property.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0 && property.CanWrite
                                    select property).AsParallel();

            if (bindedProperties == null || bindedProperties.Count() < 1)
                return;

            foreach (var prop in bindedProperties)
            {
                var colName = (prop.GetCustomAttributes(typeof(ColumnAttribute), true)[0] as ColumnAttribute).Name;

                if (!dataReader.HasColumn(colName)
                    || dataReader[colName] == DBNull.Value || string.IsNullOrWhiteSpace(dataReader[colName].ToString()))
                    continue;

                var value = dataReader[colName].ToString();
                var type = prop.PropertyType;

                if (type == typeof(string))
                    prop.SetValue(entity, value, null);
                else if (type == typeof(short) || type == typeof(short?))
                    prop.SetValue(entity, Convert.ToInt16(value), null);
                else if (type == typeof(int) || type == typeof(int?))
                    prop.SetValue(entity, Convert.ToInt32(value), null);
                else if (type == typeof(long) || type == typeof(long?))
                    prop.SetValue(entity, Convert.ToInt64(value), null);
                else if (type == typeof(double) || type == typeof(double?))
                    prop.SetValue(entity, Convert.ToDouble(value), null);
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                    prop.SetValue(entity, Convert.ToDateTime(value), null);
                else if (type == typeof(bool) || type == typeof(bool?))
                    prop.SetValue(entity, Convert.ToBoolean(value), null);
                else
                    prop.SetValue(entity, value, null);
            }

            //var count = dataReader.FieldCount;
            //for (var i = 0; i < count; i++)
            //{
            //    if (dataReader[i] == DBNull.Value)
            //        continue;

            //    var name = dataReader.GetName(i).ToLower();
            //    var value = dataReader[i].ToString();                

            //    var prop = bindedProperties.Where(property => (property.GetCustomAttributes(typeof(ColumnAttribute), true)
            //        .ElementAt(0) as ColumnAttribute).Name.ToLower() == name).FirstOrDefault<PropertyInfo>();

            //    if (prop != null && !string.IsNullOrWhiteSpace(value))
            //    {                    
            //        var type = prop.PropertyType;

            //        if (type == typeof(string))
            //            prop.SetValue(entity, value, null);
            //        else if (type == typeof(short) || type == typeof(short?))
            //            prop.SetValue(entity, Convert.ToInt16(value), null);
            //        else if (type == typeof(int) || type == typeof(int?))
            //            prop.SetValue(entity, Convert.ToInt32(value), null);
            //        else if (type == typeof(long) || type == typeof(long?))
            //            prop.SetValue(entity, Convert.ToInt64(value), null);
            //        else if (type == typeof(double) || type == typeof(double?))
            //            prop.SetValue(entity, Convert.ToDouble(value), null);
            //        else if (type == typeof(DateTime) || type == typeof(DateTime?))
            //            prop.SetValue(entity, Convert.ToDateTime(value), null);
            //        else if (type == typeof(bool) || type == typeof(bool?))
            //            prop.SetValue(entity, Convert.ToBoolean(value), null);
            //        else
            //            prop.SetValue(entity, value, null);
            //    }
            //}

            GC.Collect();
        }

        public static List<T> ConstructByColumnBinding<T>(this List<T> entityList, IDataReader dataReader)
            where T : class, IConstructFromColumnBinding, new()
        {
            if (dataReader == null || dataReader.IsClosed || entityList == null)
                return null;

            var entity = new T();
            var bindedProperties = (from property in typeof(T).GetProperties()
                                    where property.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0 && property.CanWrite
                                    select property).AsParallel();

            if (bindedProperties == null || bindedProperties.Count() < 1)
                return null;

            while (dataReader.Read())
            {
                foreach (var prop in bindedProperties)
                {
                    var colName = (prop.GetCustomAttributes(typeof(ColumnAttribute), true)[0] as ColumnAttribute).Name;

                    if (!dataReader.HasColumn(colName)
                        || dataReader[colName] == DBNull.Value || string.IsNullOrWhiteSpace(dataReader[colName].ToString()))
                        continue;

                    var value = dataReader[colName].ToString();
                    var type = prop.PropertyType;

                    if (type == typeof(string))
                        prop.SetValue(entity, value, null);
                    else if (type == typeof(short) || type == typeof(short?))
                        prop.SetValue(entity, Convert.ToInt16(value), null);
                    else if (type == typeof(int) || type == typeof(int?))
                        prop.SetValue(entity, Convert.ToInt32(value), null);
                    else if (type == typeof(long) || type == typeof(long?))
                        prop.SetValue(entity, Convert.ToInt64(value), null);
                    else if (type == typeof(double) || type == typeof(double?))
                        prop.SetValue(entity, Convert.ToDouble(value), null);
                    else if (type == typeof(DateTime) || type == typeof(DateTime?))
                        prop.SetValue(entity, Convert.ToDateTime(value), null);
                    else if (type == typeof(bool) || type == typeof(bool?))
                        prop.SetValue(entity, Convert.ToBoolean(value), null);
                    else
                        prop.SetValue(entity, value, null);

                    entity.ConstructFromReader(dataReader);
                    entityList.Add(entity);
                }
            }

            GC.Collect();
            return entityList;
        }

        public static void ConstructByColumnBinding<T>(this T entity, DataRow dataRow)
            where T : class, IConstructFromColumnBinding
        {
            if (dataRow == null)
                return;

            var bindedProperties = entity.GetType().GetProperties()
                    .Where(property => property.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0 && property.CanWrite).AsParallel();

            if (bindedProperties == null || bindedProperties.Count() < 1)
                return;

            foreach (var prop in bindedProperties)
            {
                var colName = (prop.GetCustomAttributes(typeof(ColumnAttribute), true)[0] as ColumnAttribute).Name;

                if (!dataRow.Table.Columns.Contains(colName)
                    || dataRow[colName] == DBNull.Value || string.IsNullOrWhiteSpace(dataRow[colName].ToString()))
                    continue;

                var value = dataRow[colName].ToString();
                var type = prop.PropertyType;

                if (type == typeof(string))
                    prop.SetValue(entity, value, null);
                else if (type == typeof(short) || type == typeof(short?))
                    prop.SetValue(entity, Convert.ToInt16(value), null);
                else if (type == typeof(int) || type == typeof(int?))
                    prop.SetValue(entity, Convert.ToInt32(value), null);
                else if (type == typeof(long) || type == typeof(long?))
                    prop.SetValue(entity, Convert.ToInt64(value), null);
                else if (type == typeof(double) || type == typeof(double?))
                    prop.SetValue(entity, Convert.ToDouble(value), null);
                else if (type == typeof(DateTime) || type == typeof(DateTime?))
                    prop.SetValue(entity, Convert.ToDateTime(value), null);
                else if (type == typeof(bool) || type == typeof(bool?))
                    prop.SetValue(entity, Convert.ToBoolean(value), null);
                else
                    prop.SetValue(entity, value, null);
            }

            //var count = dataRow.ItemArray.Count();
            //for (var i = 0; i < count; i++)
            //{
            //    if (dataRow[i] == DBNull.Value)
            //        continue;

            //    var name = dataRow.Table.Columns[i].ColumnName.ToLower();
            //    var value = dataRow[i].ToString();
            //    var prop = bindedProperties.Where(property => (property.GetCustomAttributes(typeof(ColumnAttribute), true)
            //        .ElementAt(0) as ColumnAttribute).Name.ToLower() == name).FirstOrDefault<PropertyInfo>();

            //    if (prop != null && !string.IsNullOrWhiteSpace(value))
            //    {
            //        var type = prop.PropertyType;

            //        if (type == typeof(string))
            //            prop.SetValue(entity, value, null);
            //        else if (type == typeof(short) || type == typeof(short?))
            //            prop.SetValue(entity, Convert.ToInt16(value), null);
            //        else if (type == typeof(int) || type == typeof(int?))
            //            prop.SetValue(entity, Convert.ToInt32(value), null);
            //        else if (type == typeof(long) || type == typeof(long?))
            //            prop.SetValue(entity, Convert.ToInt64(value), null);
            //        else if (type == typeof(double) || type == typeof(double?))
            //            prop.SetValue(entity, Convert.ToDouble(value), null);
            //        else if (type == typeof(DateTime) || type == typeof(DateTime?))
            //            prop.SetValue(entity, Convert.ToDateTime(value), null);
            //        else if (type == typeof(bool) || type == typeof(bool?))
            //            prop.SetValue(entity, Convert.ToBoolean(value), null);
            //        else
            //            prop.SetValue(entity, value, null);
            //    }
            //}

            GC.Collect();
        }

        public static bool HasColumn(this IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;

            //try
            //{
            //    return reader.GetOrdinal(columnName) >= 0;
            //}
            //catch (IndexOutOfRangeException)
            //{ }

            //return false;
        }
    }
}
