using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MicroServiceCategory.Infrastructure.Persistence
{
    public class MySqlConnectionDB
    {
        private readonly string _connectionString;
        public MySqlConnectionDB(IConfiguration connectionString)
        {
            _connectionString = connectionString.GetConnectionString("MySqlTariffingDB");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

    }
}
