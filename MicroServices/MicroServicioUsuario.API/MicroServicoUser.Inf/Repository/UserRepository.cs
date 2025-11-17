using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Persistence;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicoUser.Inf.Repository
{
    public class UserRepository : IRepository
    {
        private readonly MySqlConnectionDB connectionDB;
        public UserRepository(MySqlConnectionDB con)
        {
            connectionDB = con;
        }
        public Task<int> Delete(User t)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Insert(User t)
        {
            int id = 0;
            string query = @"INSERT INTO `user` (username, email, password_hash, first_name, second_name, first_last_name, second_last_name, role, created_by)
                            VALUES (@username, @email, MD5(@password_hash), @first_name, @second_name, @first_last_name, @second_last_name, @role, @created_by);SELECT LAST_INSERT_ID();";

            using (var conn = connectionDB.GetConnection())
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", t.Username);
                command.Parameters.AddWithValue("@email", t.Email);
                command.Parameters.AddWithValue("@password_hash", t.PasswordHash);
                command.Parameters.AddWithValue("@first_name", t.FirstName);
                command.Parameters.AddWithValue("@second_name", t.SecondName);
                command.Parameters.AddWithValue("@first_last_name", t.FirstLastName);
                command.Parameters.AddWithValue("@second_last_name", t.SecondLastName);
                command.Parameters.AddWithValue("@role", t.Role);
                command.Parameters.AddWithValue("@created_by", t.CreatedBy);

                await conn.OpenAsync();
                var res = await command.ExecuteScalarAsync();
                id = Convert.ToInt32(res);

            }

            return id;
        }

        public async Task<List<User>> Search(string p)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> Select()
        {
            List<User> lista = new List<User>();
            string query = @"SELECT username, first_name, second_name, first_last_name, second_last_name, email, role
                                FROM user
                                WHERE status=1
                                ORDER BY 4";
            using (var conn = connectionDB.GetConnection())
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                await conn.OpenAsync();

                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lista.Add(new User
                    {
                        Username = reader["username"].ToString(),
                        FirstName = reader["first_name"].ToString(),
                        SecondName = reader["second_name"].ToString(),
                        FirstLastName = reader["first_last_name"].ToString(),
                        SecondLastName = reader["second_last_name"].ToString(),
                        Email = reader["email"].ToString(),
                        Role = reader["role"].ToString()
                    });
                }
                return lista;
            }
        }

        public async Task<int> Update(User t)
        {
            throw new NotImplementedException();
        }
    }
}
