using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Payment.Dom.Interface;
using Payment.Dom.Model;
using Payment.Inf.Persistence;
using MySql.Data.MySqlClient;

namespace Payment.Inf.Repository;

public class PaymentRepository: IRepository
{
    private readonly MySqlConnectionDB _connectionDB;

    public PaymentRepository(MySqlConnectionDB connectionDB)
    {
        _connectionDB = connectionDB;
    }

    public async Task<Result<int>> Insert(Payment.Dom.Model.Payment t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO payment
(establishment_id, category_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status)
VALUES (@establishment_id, @category_id, @payment_date, @amount_paid, @payment_method, @receipt_number, @created_by, @created_date, @last_update, @status);";

            cmd.Parameters.AddWithValue("@establishment_id", t.EstablishmentId);
            cmd.Parameters.AddWithValue("@category_id", t.CategoryId);
            cmd.Parameters.AddWithValue("@payment_date", t.PaymentDate);
            cmd.Parameters.AddWithValue("@amount_paid", t.AmountPaid);
            cmd.Parameters.AddWithValue("@payment_method", t.PaymentMethod);
            cmd.Parameters.AddWithValue("@receipt_number", t.ReceiptNumber);
            cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);

            var createdDate = t.CreatedDate == default ? DateTime.Now : t.CreatedDate;
            var lastUpdate = t.UpdateDate == default ? DateTime.Now : t.UpdateDate;

            cmd.Parameters.AddWithValue("@created_date", createdDate);
            cmd.Parameters.AddWithValue("@last_update", lastUpdate);
            cmd.Parameters.AddWithValue("@status", 1);

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

    public async Task<Result<int>> Update(Payment.Dom.Model.Payment t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE payment SET
establishment_id = @establishment_id,
category_id = @category_id,
payment_date = @payment_date,
amount_paid = @amount_paid,
payment_method = @payment_method,
receipt_number = @receipt_number,
created_by = @created_by,
last_update = @last_update,
status = @status
WHERE id = @id;";

            cmd.Parameters.AddWithValue("@establishment_id", t.EstablishmentId);
            cmd.Parameters.AddWithValue("@category_id", t.CategoryId);
            cmd.Parameters.AddWithValue("@payment_date", t.PaymentDate);
            cmd.Parameters.AddWithValue("@amount_paid", t.AmountPaid);
            cmd.Parameters.AddWithValue("@payment_method", t.PaymentMethod);
            cmd.Parameters.AddWithValue("@receipt_number", t.ReceiptNumber);
            cmd.Parameters.AddWithValue("@created_by", t.CreatedBy);
            cmd.Parameters.AddWithValue("@last_update", t.UpdateDate == default ? DateTime.Now : t.UpdateDate);
            cmd.Parameters.AddWithValue("@status", 1);
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
    
    public async Task<Result<int>> Delete(Payment.Dom.Model.Payment t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE payment SET status = 0, last_update = @last_update WHERE id = @id;";
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

    public async Task<Result<List<Payment.Dom.Model.Payment>>> Select()
    {
        try
        {
            var result = new List<Payment.Dom.Model.Payment>();

            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, establishment_id, category_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status FROM payment WHERE status=1;";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // get ordinals once
                var idxId = reader.GetOrdinal("id");
                var idxEstablishmentId = reader.GetOrdinal("establishment_id");
                var idxCategoryId = reader.GetOrdinal("category_id");
                var idxPaymentDate = reader.GetOrdinal("payment_date");
                var idxAmountPaid = reader.GetOrdinal("amount_paid");
                var idxPaymentMethod = reader.GetOrdinal("payment_method");
                var idxReceiptNumber = reader.GetOrdinal("receipt_number");
                var idxCreatedBy = reader.GetOrdinal("created_by");
                var idxCreatedDate = reader.GetOrdinal("created_date");
                var idxLastUpdate = reader.GetOrdinal("last_update");
                var idxStatus = reader.GetOrdinal("status");

                var item = new Payment.Dom.Model.Payment
                {
                    Id = reader.GetInt32(idxId),
                    EstablishmentId = reader.IsDBNull(idxEstablishmentId) ? 0 : reader.GetInt32(idxEstablishmentId),
                    CategoryId = reader.IsDBNull(idxCategoryId) ? 0 : reader.GetInt32(idxCategoryId),
                    PaymentDate = reader.IsDBNull(idxPaymentDate) ? DateTime.MinValue : reader.GetDateTime(idxPaymentDate),
                    AmountPaid = reader.IsDBNull(idxAmountPaid) ? 0 : reader.GetDecimal(idxAmountPaid),
                    PaymentMethod = reader.IsDBNull(idxPaymentMethod) ? string.Empty : reader.GetString(idxPaymentMethod),
                    ReceiptNumber = reader.IsDBNull(idxReceiptNumber) ? string.Empty : reader.GetString(idxReceiptNumber),
                    CreatedBy = reader.IsDBNull(idxCreatedBy) ? 0 : reader.GetInt32(idxCreatedBy),
                    CreatedDate = reader.IsDBNull(idxCreatedDate) ? DateTime.MinValue : reader.GetDateTime(idxCreatedDate),
                    UpdateDate = reader.IsDBNull(idxLastUpdate) ? DateTime.MinValue : reader.GetDateTime(idxLastUpdate),
                    Status = reader.IsDBNull(idxStatus) ? false : reader.GetInt32(idxStatus) == 1
                };

                result.Add(item);
            }

            return Result<List<Payment.Dom.Model.Payment>>.Success(result);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<List<Payment.Dom.Model.Payment>>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<Payment.Dom.Model.Payment>> SelectById(int id)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, establishment_id, category_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status
                            FROM payment
                            WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return Result<Payment.Dom.Model.Payment>.Failure("NotFound");
            }

            // get ordinals once
            var idxId = reader.GetOrdinal("id");
            var idxEstablishmentId = reader.GetOrdinal("establishment_id");
            var idxCategoryId = reader.GetOrdinal("category_id");
            var idxPaymentDate = reader.GetOrdinal("payment_date");
            var idxAmountPaid = reader.GetOrdinal("amount_paid");
            var idxPaymentMethod = reader.GetOrdinal("payment_method");
            var idxReceiptNumber = reader.GetOrdinal("receipt_number");
            var idxCreatedBy = reader.GetOrdinal("created_by");
            var idxCreatedDate = reader.GetOrdinal("created_date");
            var idxLastUpdate = reader.GetOrdinal("last_update");
            var idxStatus = reader.GetOrdinal("status");

            var item = new Payment.Dom.Model.Payment
            {
                Id = reader.GetInt32(idxId),
                EstablishmentId = reader.IsDBNull(idxEstablishmentId) ? 0 : reader.GetInt32(idxEstablishmentId),
                CategoryId = reader.IsDBNull(idxCategoryId) ? 0 : reader.GetInt32(idxCategoryId),
                PaymentDate = reader.IsDBNull(idxPaymentDate) ? DateTime.MinValue : reader.GetDateTime(idxPaymentDate),
                AmountPaid = reader.IsDBNull(idxAmountPaid) ? 0 : reader.GetDecimal(idxAmountPaid),
                PaymentMethod = reader.IsDBNull(idxPaymentMethod) ? string.Empty : reader.GetString(idxPaymentMethod),
                ReceiptNumber = reader.IsDBNull(idxReceiptNumber) ? string.Empty : reader.GetString(idxReceiptNumber),
                CreatedBy = reader.IsDBNull(idxCreatedBy) ? 0 : reader.GetInt32(idxCreatedBy),
                CreatedDate = reader.IsDBNull(idxCreatedDate) ? DateTime.MinValue : reader.GetDateTime(idxCreatedDate),
                UpdateDate = reader.IsDBNull(idxLastUpdate) ? DateTime.MinValue : reader.GetDateTime(idxLastUpdate),
                Status = reader.IsDBNull(idxStatus) ? false : reader.GetInt32(idxStatus) == 1
            };

            return Result<Payment.Dom.Model.Payment>.Success(item);
        }
        catch (Exception ex)
        {
            // Consider logging here
            return Result<Payment.Dom.Model.Payment>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<IEnumerable<Payment.Dom.Model.Payment>>> Search(string property)
    {
        const string sql = @"
        SELECT
            id,
            establishment_id,
            category_id,
            payment_date,
            amount_paid,
            payment_method,
            receipt_number,
            created_by,
            created_date,
            last_update,
            status
        FROM payment
        WHERE status = 1 AND (
            (@prop IS NOT NULL AND payment_method LIKE CONCAT('%', @prop, '%')) OR
            (@prop IS NOT NULL AND receipt_number LIKE CONCAT('%', @prop, '%'))
        )
        ORDER BY id DESC;";
    
        var list = new List<Payment.Dom.Model.Payment>();
    
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
    
            cmd.Parameters.AddWithValue("@prop", string.IsNullOrWhiteSpace(property) ? DBNull.Value : property);
    
            using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReaderToPayment(reader));
            }
    
            return Result<IEnumerable<Payment.Dom.Model.Payment>>.Success(list);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Payment.Dom.Model.Payment>>.Failure(ex.Message);
        }
    }
    
    private static Payment.Dom.Model.Payment MapReaderToPayment(MySqlDataReader reader)
    {
        var idxId = reader.GetOrdinal("id");
        var idxEstablishmentId = reader.GetOrdinal("establishment_id");
        var idxCategoryId = reader.GetOrdinal("category_id");
        var idxPaymentDate = reader.GetOrdinal("payment_date");
        var idxAmountPaid = reader.GetOrdinal("amount_paid");
        var idxPaymentMethod = reader.GetOrdinal("payment_method");
        var idxReceiptNumber = reader.GetOrdinal("receipt_number");
        var idxCreatedBy = reader.GetOrdinal("created_by");
        var idxCreatedDate = reader.GetOrdinal("created_date");
        var idxLastUpdate = reader.GetOrdinal("last_update");
        var idxStatus = reader.GetOrdinal("status");
    
        return new Payment.Dom.Model.Payment
        {
            Id = reader.IsDBNull(idxId) ? 0 : reader.GetInt32(idxId),
            EstablishmentId = reader.IsDBNull(idxEstablishmentId) ? 0 : reader.GetInt32(idxEstablishmentId),
            CategoryId = reader.IsDBNull(idxCategoryId) ? 0 : reader.GetInt32(idxCategoryId),
            PaymentDate = reader.IsDBNull(idxPaymentDate) ? DateTime.MinValue : reader.GetDateTime(idxPaymentDate),
            AmountPaid = reader.IsDBNull(idxAmountPaid) ? 0 : reader.GetDecimal(idxAmountPaid),
            PaymentMethod = reader.IsDBNull(idxPaymentMethod) ? string.Empty : reader.GetString(idxPaymentMethod),
            ReceiptNumber = reader.IsDBNull(idxReceiptNumber) ? string.Empty : reader.GetString(idxReceiptNumber),
            CreatedBy = reader.IsDBNull(idxCreatedBy) ? 0 : reader.GetInt32(idxCreatedBy),
            CreatedDate = reader.IsDBNull(idxCreatedDate) ? DateTime.MinValue : reader.GetDateTime(idxCreatedDate),
            UpdateDate = reader.IsDBNull(idxLastUpdate) ? DateTime.MinValue : reader.GetDateTime(idxLastUpdate),
            Status = reader.IsDBNull(idxStatus) ? false : reader.GetInt32(idxStatus) == 1
        };
    }
}
