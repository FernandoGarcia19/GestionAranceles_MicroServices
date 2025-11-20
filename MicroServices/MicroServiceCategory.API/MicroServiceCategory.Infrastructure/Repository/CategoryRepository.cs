using MicroServiceCategory.Infrastructure.Persistence;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroServiceCategory.Domain.Entities;
using MicroServiceCategory.Domain.Interfaces;

namespace MicroServiceCategory.Infrastructure.Repository
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly MySqlConnectionDB _connectionDB;

        public CategoryRepository(MySqlConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public async Task<int> Insert(Category model)
        {
            int id = 0;
            string query = @"INSERT INTO 
                           category(name, description, base_amount, created_by, status) 
                           VALUES (@Name, @Description, @BaseAmount, @CreatedBy, 1);
                           SELECT LAST_INSERT_ID();";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@BaseAmount", model.BaseAmount);
                    command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

                    var result = await command.ExecuteScalarAsync();
                    id = Convert.ToInt32(result);
                }
            }

            return id;
        }

        public async Task<int> Update(Category model)
        {
            string query = @"UPDATE category 
                           SET name = @Name, 
                           description = @Description, 
                           base_amount = @BaseAmount, 
                           last_update = NOW()
                           WHERE id = @Id;";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", model.Id);
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@BaseAmount", model.BaseAmount);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? model.Id : 0;
                }
            }
        }

        public async Task<int> Delete(Category model)
        {
            string query = @"UPDATE category 
                           SET status = 0, last_update = NOW() 
                           WHERE id = @Id;";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", model.Id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? model.Id : 0;
                }
            }
        }

        public async Task<List<Category>> Select()
        {
            List<Category> listaCategories = new List<Category>();

            string query = @"SELECT id, 
                           name, description, base_amount, created_by, 
                           created_date, last_update, status

                           FROM category 
                           WHERE status = 1;";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listaCategories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                BaseAmount = Convert.ToDecimal(reader["base_amount"]),
                                CreatedBy = Convert.ToInt32(reader["created_by"]),
                                CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                LastUpdate = Convert.ToDateTime(reader["last_update"]),
                                Status = Convert.ToByte(reader["status"])
                            });
                        }
                    }
                }
            }

            return listaCategories;
        }

        public async Task<List<Category>> Search(string property)
        {
            List<Category> listaCategories = new List<Category>();

            string query = @"SELECT id, 
                           name, description, base_amount, created_by,
                           created_date, last_update, status 
                           FROM category 
                           WHERE status = 1 
                           AND (name LIKE @Search OR description LIKE @Search);";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{property}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listaCategories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                BaseAmount = Convert.ToDecimal(reader["base_amount"]),
                                CreatedBy = Convert.ToInt32(reader["created_by"]),
                                CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                LastUpdate = Convert.ToDateTime(reader["last_update"]),
                                Status = Convert.ToByte(reader["status"])
                            });
                        }
                    }
                }
            }

            return listaCategories;
        }
    }
}
