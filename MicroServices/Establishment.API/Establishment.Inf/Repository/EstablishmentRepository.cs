using MySql.Data.MySqlClient;
using Establishment.Dom.Interface;
using Establishment.Dom.Model;
using Establishment.Inf.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Establishment.Inf.Repository;

public class EstablishmentRepository: IRepository
{
    private readonly MySqlConnectionDB _connectionDB;

    public EstablishmentRepository(MySqlConnectionDB connectionDB)
    {
        _connectionDB = connectionDB;
    }

    public async Task<int> Insert(Establishment.Dom.Model.Establishment t)
    {
        const string sql = @"INSERT INTO establishment
            (name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, created_by, status, last_update)
            VALUES
            (@name, @tax_id, @sanitary_license, @sanitary_license_expiry, @address, @phone, @email, @establishment_type, @created_by, @status, @last_update);";

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
        cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);
        cmd.Parameters.AddWithValue("@status", t.Status ? 1 : 0);
        cmd.Parameters.AddWithValue("@last_update", lastUpdate);

        await cmd.ExecuteNonQueryAsync();
         // LastInsertedId is available on MySqlCommand after execution
         var lastId = (int)cmd.LastInsertedId;
         return lastId;
    }

    public async Task<int> Update(Establishment.Dom.Model.Establishment t)
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
            last_update = CURRENT_TIMESTAMP,
            status = @status
            WHERE id = @id;";

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
        cmd.Parameters.AddWithValue("@status", t.Status ? 1 : 0);
        cmd.Parameters.AddWithValue("@id", t.Id);

        var affected = await cmd.ExecuteNonQueryAsync();
        return affected;
    }

    // Logical delete: set status = 0 and update last_update
    public async Task<int> Delete(Establishment.Dom.Model.Establishment t)
    {
        const string sql = @"UPDATE establishment SET status = 0, last_update = CURRENT_TIMESTAMP WHERE id = @id;";

        await using var conn = _connectionDB.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", t.Id);

        var affected = await cmd.ExecuteNonQueryAsync();
        return affected;
    }

    public async Task<Establishment.Dom.Model.Establishment> SelectById(int id)
    {
        const string sql = @"SELECT id, name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, created_by, created_date, last_update, status
            FROM establishment WHERE id = @id;";

        await using var conn = _connectionDB.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var est = MapReaderToEstablishment(reader);
        return est;
    }

    public async Task<List<Establishment.Dom.Model.Establishment>> Select()
    {
        const string sql = @"SELECT id, name, tax_id, sanitary_license, sanitary_license_expiry, address, phone, email, establishment_type, created_by, created_date, last_update, status
            FROM establishment WHERE status = 1;";

        var list = new List<Establishment.Dom.Model.Establishment>();

        await using var conn = _connectionDB.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);

        await using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapReaderToEstablishment(reader));
        }

        return list;
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
        est.CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? 0 : reader.GetInt32("created_by");
        est.CreatedDate = reader.IsDBNull(reader.GetOrdinal("created_date")) ? DateTime.MinValue : reader.GetDateTime("created_date");
        est.LastUpdate = reader.IsDBNull(reader.GetOrdinal("last_update")) ? DateTime.MinValue : reader.GetDateTime("last_update");
        est.Status = !reader.IsDBNull(reader.GetOrdinal("status")) && reader.GetInt32("status") == 1;

        return est;
    }
}