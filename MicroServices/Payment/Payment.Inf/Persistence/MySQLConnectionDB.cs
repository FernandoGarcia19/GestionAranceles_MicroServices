using Microsoft.Extensions.Configuration;

namespace Payment.Inf.Persistence;
using MySql.Data.MySqlClient;

public class MySqlConnectionDB
{
    private readonly string connectionString;

    public MySqlConnectionDB(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("PaymentDB") ?? throw new InvalidOperationException("PaymentDB connection string not found");
    }

    public MySqlConnection GetConnection() {
        return new MySqlConnection(connectionString);
    }
}
