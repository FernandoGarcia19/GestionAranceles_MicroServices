using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace MicroServicoUser.Inf.Persistence.Database;

public class MySqlConnectionManager
{
    private readonly string _connectionString;
    public MySqlConnectionManager(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MysqlUserServicioDB");
    }
    private MySqlConnection CreateConnection() => new(_connectionString);

    public IEnumerable<T> ExecuteParameterizedQuery<T>(string query, T model) where T : new()
    {
        using MySqlCommand command = DbParameterHelper.PopulateCommandParameters(query, model);
        return ExecuteCommand<T>(command);
    }
    public IEnumerable<TOut> ExecuteParameterizedQuery<TOut, TParam>(string query, TParam model) where TOut : new()
    {
        using MySqlCommand command = DbParameterHelper.PopulateCommandParameters(query, model);
        return ExecuteCommand<TOut>(command);
    }

    public IEnumerable<T> ExecuteQuery<T>(string query) where T : new()
    {
        MySqlCommand command = new(query);
        return ExecuteCommand<T>(command);
    }
    public int ExecuteParameterizedNonQuery<T>(string query, T model) where T : new()
    {
        using MySqlCommand command = DbParameterHelper.PopulateCommandParameters(query, model);
        return ExecuteCommand(command);
    }

    public int ExecuteNonQuery(string query)
    {
        using MySqlCommand command = new(query);
        return ExecuteCommand(command);
    }

    private int ExecuteCommand(MySqlCommand command)
    {
        using MySqlConnection connection = CreateConnection();
        command.Connection = connection;
        connection.Open();
        int affectedRows = command.ExecuteNonQuery();
        return affectedRows;
    }
    private IEnumerable<T> ExecuteCommand<T>(MySqlCommand command) where T : new()
    {
        using MySqlConnection connection = CreateConnection();

        command.Connection = connection;
        connection.Open();

        using MySqlDataAdapter adapter = new(command);
        DataTable dataTable = new();
        adapter.Fill(dataTable);

        IEnumerable<T> results = DbMapper.MapDataTableToModelIterable<T>(dataTable);
        return results;
    }
}
