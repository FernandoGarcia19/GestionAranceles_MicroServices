using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicoUser.Inf.Persistence
{
    public class MySqlConnectionDB
    {
        private readonly string connectionString;

        public MySqlConnectionDB(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MysqlUserServicioDB");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
