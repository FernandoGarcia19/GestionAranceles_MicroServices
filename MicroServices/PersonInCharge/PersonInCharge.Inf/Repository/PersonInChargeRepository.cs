using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonInCharge.Dom.Interface;
using PersonInCharge.Dom.Model;
using PersonInCharge.Inf.Persistence;
using MySql.Data.MySqlClient;

namespace PersonInCharge.Inf.Repository;

public class PersonInChargeRepository: IRepository
{
    private readonly MySqlConnectionDB _connectionDB;

    public PersonInChargeRepository(MySqlConnectionDB connectionDB)
    {
        _connectionDB = connectionDB;
    }

    public async Task<Result<int>> Insert(PersonInCharge.Dom.Model.PersonInCharge t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO person_in_charge
(first_name, last_name, email, phone, ci, created_by, created_date, last_update, status)
VALUES (@first_name, @last_name, @email, @phone, @ci, @created_by, @created_date, @last_update, @status);";

            cmd.Parameters.AddWithValue("@first_name", t.FirstName);
            cmd.Parameters.AddWithValue("@last_name", t.LastName);
            cmd.Parameters.AddWithValue("@email", t.Email);
            cmd.Parameters.AddWithValue("@phone", t.Phone);
            cmd.Parameters.AddWithValue("@ci", t.Ci);
            cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);

            var createdDate = t.CreatedDate == default ? DateTime.Now : t.CreatedDate;
            var lastUpdate = t.UpdateDate == default ? DateTime.Now : t.UpdateDate;

            cmd.Parameters.AddWithValue("@created_date", createdDate);
            cmd.Parameters.AddWithValue("@last_update", lastUpdate);
            cmd.Parameters.AddWithValue("@status", t.Status ? 1 : 0);

            await cmd.ExecuteNonQueryAsync();

            // LastInsertedId is returned as a long/ulong depending on provider; convert to int safely
            var lastId = Convert.ToInt32(cmd.LastInsertedId);
            if (lastId <= 0)
                return Result<int>.Failure("InsertFailed");

            return Result<int>.Success(lastId);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
    }

    public async Task<Result<int>> Update(PersonInCharge.Dom.Model.PersonInCharge t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE person_in_charge SET
first_name = @first_name,
last_name = @last_name,
email = @email,
phone = @phone,
ci = @ci,
created_by = @created_by,
last_update = @last_update,
status = @status
WHERE id = @id;";

            cmd.Parameters.AddWithValue("@first_name", t.FirstName);
            cmd.Parameters.AddWithValue("@last_name", t.LastName);
            cmd.Parameters.AddWithValue("@email", t.Email);
            cmd.Parameters.AddWithValue("@phone", t.Phone);
            cmd.Parameters.AddWithValue("@ci", t.Ci);
            cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);
            cmd.Parameters.AddWithValue("@last_update", t.UpdateDate == default ? DateTime.Now : t.UpdateDate);
            cmd.Parameters.AddWithValue("@status", t.Status ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", t.Id);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected == 0)
                return Result<int>.Failure("NoRowsAffected");

            return Result<int>.Success(affected);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<int>> Delete(PersonInCharge.Dom.Model.PersonInCharge t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE person_in_charge SET status = 0, last_update = @last_update WHERE id = @id;";
            cmd.Parameters.AddWithValue("@last_update", t.UpdateDate == default ? DateTime.Now : t.UpdateDate);
            cmd.Parameters.AddWithValue("@id", t.Id);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected == 0)
                return Result<int>.Failure("NoRowsAffected");

            return Result<int>.Success(affected);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
    }

    public async Task<Result<List<PersonInCharge.Dom.Model.PersonInCharge>>> Select()
    {
        try
        {
            var result = new List<PersonInCharge.Dom.Model.PersonInCharge>();

            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, first_name, last_name, email, phone, ci, created_by, created_date, last_update, status FROM person_in_charge;";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // get ordinals once
                var idxId = reader.GetOrdinal("id");
                var idxFirstName = reader.GetOrdinal("first_name");
                var idxLastName = reader.GetOrdinal("last_name");
                var idxEmail = reader.GetOrdinal("email");
                var idxPhone = reader.GetOrdinal("phone");
                var idxCi = reader.GetOrdinal("ci");
                var idxCreatedBy = reader.GetOrdinal("created_by");
                var idxCreatedDate = reader.GetOrdinal("created_date");
                var idxLastUpdate = reader.GetOrdinal("last_update");
                var idxStatus = reader.GetOrdinal("status");

                var item = new PersonInCharge.Dom.Model.PersonInCharge
                {
                    Id = reader.GetInt32(idxId),
                    FirstName = reader.IsDBNull(idxFirstName) ? string.Empty : reader.GetString(idxFirstName),
                    LastName = reader.IsDBNull(idxLastName) ? string.Empty : reader.GetString(idxLastName),
                    Email = reader.IsDBNull(idxEmail) ? string.Empty : reader.GetString(idxEmail),
                    Phone = reader.IsDBNull(idxPhone) ? string.Empty : reader.GetString(idxPhone),
                    Ci = reader.IsDBNull(idxCi) ? string.Empty : reader.GetString(idxCi),
                    CreatedBy = reader.IsDBNull(idxCreatedBy) ? 0 : reader.GetInt32(idxCreatedBy),
                    CreatedDate = reader.IsDBNull(idxCreatedDate) ? DateTime.MinValue : reader.GetDateTime(idxCreatedDate),
                    UpdateDate = reader.IsDBNull(idxLastUpdate) ? DateTime.MinValue : reader.GetDateTime(idxLastUpdate),
                    Status = reader.IsDBNull(idxStatus) ? false : reader.GetInt32(idxStatus) == 1
                };

                result.Add(item);
            }

            return Result<List<PersonInCharge.Dom.Model.PersonInCharge>>.Success(result);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<List<PersonInCharge.Dom.Model.PersonInCharge>>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<PersonInCharge.Dom.Model.PersonInCharge>> SelectById(int id)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, first_name, last_name, email, phone, ci, created_by, created_date, last_update, status
                            FROM person_in_charge
                            WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return Result<PersonInCharge.Dom.Model.PersonInCharge>.Failure("NotFound");
            }

            // get ordinals once
            var idxId = reader.GetOrdinal("id");
            var idxFirstName = reader.GetOrdinal("first_name");
            var idxLastName = reader.GetOrdinal("last_name");
            var idxEmail = reader.GetOrdinal("email");
            var idxPhone = reader.GetOrdinal("phone");
            var idxCi = reader.GetOrdinal("ci");
            var idxCreatedBy = reader.GetOrdinal("created_by");
            var idxCreatedDate = reader.GetOrdinal("created_date");
            var idxLastUpdate = reader.GetOrdinal("last_update");
            var idxStatus = reader.GetOrdinal("status");

            var item = new PersonInCharge.Dom.Model.PersonInCharge
            {
                Id = reader.GetInt32(idxId),
                FirstName = reader.IsDBNull(idxFirstName) ? string.Empty : reader.GetString(idxFirstName),
                LastName = reader.IsDBNull(idxLastName) ? string.Empty : reader.GetString(idxLastName),
                Email = reader.IsDBNull(idxEmail) ? string.Empty : reader.GetString(idxEmail),
                Phone = reader.IsDBNull(idxPhone) ? string.Empty : reader.GetString(idxPhone),
                Ci = reader.IsDBNull(idxCi) ? string.Empty : reader.GetString(idxCi),
                CreatedBy = reader.IsDBNull(idxCreatedBy) ? 0 : reader.GetInt32(idxCreatedBy),
                CreatedDate = reader.IsDBNull(idxCreatedDate) ? DateTime.MinValue : reader.GetDateTime(idxCreatedDate),
                UpdateDate = reader.IsDBNull(idxLastUpdate) ? DateTime.MinValue : reader.GetDateTime(idxLastUpdate),
                Status = reader.IsDBNull(idxStatus) ? false : reader.GetInt32(idxStatus) == 1
            };

            return Result<PersonInCharge.Dom.Model.PersonInCharge>.Success(item);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<PersonInCharge.Dom.Model.PersonInCharge>.Failure($"DbError: {ex.Message}");
        }
    }
}