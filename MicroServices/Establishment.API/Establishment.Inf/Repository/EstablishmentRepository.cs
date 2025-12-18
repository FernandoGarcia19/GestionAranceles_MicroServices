using MySql.Data.MySqlClient;
using Establishment.Dom.Interface;
using Establishment.Dom.Model;
using Establishment.Inf.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;

namespace Establishment.Inf.Repository;

public class EstablishmentRepository: IRepository
{
    private readonly MySqlConnectionDB _connectionDB;

    public EstablishmentRepository(MySqlConnectionDB connectionDB)
    {
        _connectionDB = connectionDB;
    }

    public async Task<Result<int>> Insert(Establishment.Dom.Model.Establishment t)
    {
        const string sql = @"INSERT INTO establishment
            (name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, person_in_charge_id, created_by, status, last_update)
            VALUES
            (@name, @tax_id, @sanitary_license, @sanitary_license_expiry, @address, @phone, @email, @establishment_type, @person_in_charge_id, @created_by, @status, @last_update);";

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            var lastUpdate = t.LastUpdate == default ? DateTime.Now : t.LastUpdate;

            cmd.Parameters.AddWithValue("@name", t.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tax_id", t.TaxId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@sanitary_license", t.SanitaryLicense ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@sanitary_license_expiry", t.SanitaryLicenseExpiry ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", t.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", t.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", t.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@establishment_type", t.EstablishmentType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@person_in_charge_id", t.PersonInChargeId > 0 ? (object)t.PersonInChargeId : DBNull.Value);
            cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);
            cmd.Parameters.AddWithValue("@status", 1);
            cmd.Parameters.AddWithValue("@last_update", lastUpdate);

            var affected = await cmd.ExecuteNonQueryAsync();
            var lastId = (int)cmd.LastInsertedId;

            if (affected == 0 || lastId == 0)
                return Result<int>.Failure("NoRowsAffected");

            return Result<int>.Success(lastId);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public async Task<Result<int>> Update(Establishment.Dom.Model.Establishment t)
    {
        const string sql = @"UPDATE establishment SET
            name = @name,
            tax_id = @tax_id,
            sanitary_license = @sanitary_license,
            sanitary_license_expiry = @sanitary_license_expiry,
            address = @address,
            phone = @phone,
            email = @email,
            establishment_type = @establishment_type,
            person_in_charge_id = @person_in_charge_id,
            last_update = CURRENT_TIMESTAMP,
            status = @status
            WHERE id = @id;";

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", t.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tax_id", t.TaxId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@sanitary_license", t.SanitaryLicense ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@sanitary_license_expiry", t.SanitaryLicenseExpiry ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@address", t.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", t.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", t.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@establishment_type", t.EstablishmentType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@person_in_charge_id", t.PersonInChargeId > 0 ? (object)t.PersonInChargeId : DBNull.Value);
            cmd.Parameters.AddWithValue("@status", 1);
            cmd.Parameters.AddWithValue("@id", t.Id);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected == 0)
                return Result<int>.Failure("NoRowsAffected");

            return Result<int>.Success((int)affected);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    // Logical delete: set status = 0 and update last_update
    public async Task<Result<int>> Delete(Establishment.Dom.Model.Establishment t)
    {
        const string sql = @"UPDATE establishment SET status = 0, last_update = CURRENT_TIMESTAMP WHERE id = @id;";

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", t.Id);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected == 0)
                return Result<int>.Failure("NoRowsAffected");

            return Result<int>.Success((int)affected);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public async Task<Result<Establishment.Dom.Model.Establishment>> SelectById(int id)
    {
        const string sql = @"SELECT id, name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, person_in_charge_id, created_by, created_date, last_update, status
            FROM establishment WHERE id = @id;";

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return Result<Establishment.Dom.Model.Establishment>.Failure("NotFound");

            var est = MapReaderToEstablishment(reader);
            return Result<Establishment.Dom.Model.Establishment>.Success(est);
        }
        catch (Exception ex)
        {
            return Result<Establishment.Dom.Model.Establishment>.Failure(ex.Message);
        }
    }

    public async Task<Result<List<Establishment.Dom.Model.Establishment>>> Select()
    {
        const string sql = @"SELECT id, name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, person_in_charge_id, created_by, created_date, last_update, status
            FROM establishment WHERE status = 1;";

        var list = new List<Establishment.Dom.Model.Establishment>();

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);

            await using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReaderToEstablishment(reader));
            }

            return Result<List<Establishment.Dom.Model.Establishment>>.Success(list);
        }
        catch (Exception ex)
        {
            return Result<List<Establishment.Dom.Model.Establishment>>.Failure(ex.Message);
        }
    }

    private static Establishment.Dom.Model.Establishment MapReaderToEstablishment(MySqlDataReader reader)
    {
        var est = new Establishment.Dom.Model.Establishment();

        est.Id = reader.GetInt32("id");
        est.Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name");
        est.TaxId = reader.IsDBNull(reader.GetOrdinal("tax_id")) ? null : reader.GetString("tax_id");
        est.SanitaryLicense = reader.IsDBNull(reader.GetOrdinal("sanitary_license")) ? null : reader.GetString("sanitary_license");
        est.SanitaryLicenseExpiry = reader.IsDBNull(reader.GetOrdinal("sanitary_license_expiry")) ? (DateTime?)null : reader.GetDateTime("sanitary_license_expiry");
        est.Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address");
        est.Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone");
        est.Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email");
        est.EstablishmentType = reader.IsDBNull(reader.GetOrdinal("establishment_type")) ? null : reader.GetString("establishment_type");
        est.PersonInChargeId = reader.IsDBNull(reader.GetOrdinal("person_in_charge_id")) ? 0 : reader.GetInt32("person_in_charge_id");
        est.CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? 0 : reader.GetInt32("created_by");
        est.CreatedDate = reader.IsDBNull(reader.GetOrdinal("created_date")) ? DateTime.MinValue : reader.GetDateTime("created_date");
        est.LastUpdate = reader.IsDBNull(reader.GetOrdinal("last_update")) ? DateTime.MinValue : reader.GetDateTime("last_update");
        est.Status = !reader.IsDBNull(reader.GetOrdinal("status")) && reader.GetInt32("status") == 1;

        return est;
    }
    
    public async Task<Result<IEnumerable<Dom.Model.Establishment>>> Search(string property)
    {
        const string sql = @"
        SELECT
            id,
            name,
            tax_id,
            sanitary_license,
            sanitary_license_expiry,
            address,
            phone,
            email,
            establishment_type,
            created_by,
            created_date,
            last_update,
            status
        FROM establishment
        WHERE status = 1 AND (
            (@prop IS NOT NULL AND name LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND tax_id LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND address LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND phone LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND email LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND establishment_type LIKE CONCAT('%', @prop, '%'))
        )
        ORDER BY id DESC;";

        var list = new List<Dom.Model.Establishment>();

        try
        {
            await using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@prop", string.IsNullOrWhiteSpace(property) ? DBNull.Value : property);

            await using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReaderToEstablishment(reader));
            }

            return Result<IEnumerable<Dom.Model.Establishment>>.Success(list);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Dom.Model.Establishment>>.Failure(ex.Message);
        }
    }
}