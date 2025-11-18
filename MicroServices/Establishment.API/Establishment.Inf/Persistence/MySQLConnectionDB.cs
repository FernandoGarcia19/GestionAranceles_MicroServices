using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Establishment.Inf.Persistence;

public class MySqlConnectionDB
{
    private readonly string connectionString;

    public MySqlConnectionDB(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("EstablishmentDB");
    }

    public MySqlConnection GetConnection() {
        return new MySqlConnection(connectionString);
    }
}