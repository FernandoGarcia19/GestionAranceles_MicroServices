using System.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MicroServicoUser.Inf.Persistence.Database;

public static class DbMapper
{
    public static T MapDataRowToModel<T>(DataRow row) where T : new()
    {
        T model = new();
        PropertyInfo[] propertyInfo = typeof(T).GetProperties();
        foreach (PropertyInfo property in propertyInfo)
        {
            if (row.Table.Columns.Contains(property.Name))
            {
                if (row[property.Name] == DBNull.Value)
                {
                    if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                    {
                        property.SetValue(model,  null);
                    }
                    else
                    {
                        property.SetValue(model, Activator.CreateInstance(property.PropertyType));
                    }
                }
                else
                {
                    Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var castedValue = Convert.ChangeType(row[property.Name], targetType);
                    property.SetValue(model, castedValue);
                }
            }
        }
        return model;
    }

    public static IEnumerable<T> MapDataTableToModelIterable<T>(DataTable dataTable) where T : new()
    {
        List<T> resultList = [];
        foreach (DataRow row in dataTable.Rows)
        {
            T mappedObject = MapDataRowToModel<T>(row);
            resultList.Add(mappedObject);
        }
        return resultList;
    }
}