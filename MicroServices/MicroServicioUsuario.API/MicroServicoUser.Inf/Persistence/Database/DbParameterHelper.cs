using System.Reflection;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MicroServicoUser.Inf.Persistence.Database;

public static class DbParameterHelper
{
    public static MySqlCommand PopulateCommandParameters<T>(string query, T model)
    {
        var parameters = Regex.Matches(query, @"@\w+")
            .Cast<Match>()
            .Select(m => m.Value)
            .Distinct()
            .ToList();

        MySqlCommand command = new(query);

        if (model == null)
            return command;

        PropertyInfo[] modelProperties = typeof(T).GetProperties();

        foreach (string paramName in parameters)
        {
            string modelPropName = paramName[1..];
            var property = modelProperties.FirstOrDefault(p => p.Name == modelPropName);

            if (property != null)
            {
                object value = property.GetValue(model) ?? DBNull.Value;
                command.Parameters.Add(new MySqlParameter(paramName, value));
            }
            else
            {
            }
        }

        return command;
    }
}