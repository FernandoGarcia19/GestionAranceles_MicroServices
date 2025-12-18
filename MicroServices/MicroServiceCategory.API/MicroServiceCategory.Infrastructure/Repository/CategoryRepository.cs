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
                           category(name, description, base_amount, created_by, status, number_of_inserts) 
                           VALUES (@Name, @Description, @BaseAmount, @CreatedBy, 1, @NumberOfInserts);
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
                    command.Parameters.AddWithValue("@NumberOfInserts", model.NumberOfInserts);

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
                           number_of_inserts = @NumberOfInserts,
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
                    command.Parameters.AddWithValue("@NumberOfInserts", model.NumberOfInserts);

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
                           created_date, last_update, status, number_of_inserts

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
                                Status = Convert.ToByte(reader["status"]),
                                NumberOfInserts = Convert.ToInt32(reader["number_of_inserts"])
                            });
                        }
                    }
                }
            }

            return listaCategories;
        }

        public async Task<Category> SelectById(int id)
        {
            string query = @"SELECT id, name, description, base_amount, created_by, created_date, last_update, status, number_of_inserts
                     FROM category
                     WHERE id = @Id AND status = 1;";

            try
            {
                using (var connection = _connectionDB.GetConnection())
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync()) return null;

                            var category = new Category
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                BaseAmount = Convert.ToDecimal(reader["base_amount"]),
                                CreatedBy = Convert.ToInt32(reader["created_by"]),
                                CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                LastUpdate = Convert.ToDateTime(reader["last_update"]),
                                Status = Convert.ToByte(reader["status"]),
                                NumberOfInserts = Convert.ToInt32(reader["number_of_inserts"])
                            };

                            return category;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<List<Category>> Search(string property)
        {
            List<Category> listaCategories = new List<Category>();

            string query = @"SELECT id, 
                           name, description, base_amount, created_by,
                           created_date, last_update, status,  number_of_inserts
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
                                Status = Convert.ToByte(reader["status"]),
                                NumberOfInserts = Convert.ToInt32(reader["number_of_inserts"])
                            });
                        }
                    }
                }
            }

            return listaCategories;
        }
        public async Task<int> IncrementNumberOfInserts(int categoryId, int increment)
        {
            // We update the count and refresh the last_update timestamp
            string query = @"UPDATE category 
                   SET number_of_inserts = number_of_inserts + @Increment,
                   last_update = NOW()
                   WHERE id = @Id;";

            using (var connection = _connectionDB.GetConnection())
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", categoryId);
                    command.Parameters.AddWithValue("@Increment", increment);

                    // ExecuteNonQueryAsync returns the number of rows affected
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    // Return the ID if successful (row found and updated), otherwise 0
                    return rowsAffected > 0 ? categoryId : 0;
                }
            }
        }
    }
    
    
}
