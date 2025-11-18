using Microsoft.Extensions.Configuration;

namespace PersonInCharge.Inf.Persistence;
using MySql.Data.MySqlClient;

public class MySqlConnectionDB
{
    private readonly string connectionString;

    public MySqlConnectionDB(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("PersonInChargeDB");
    }

    public MySqlConnection GetConnection() {
        return new MySqlConnection(connectionString);
    }



}