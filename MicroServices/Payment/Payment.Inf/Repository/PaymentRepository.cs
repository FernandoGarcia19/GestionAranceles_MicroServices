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
        MySqlConnection? conn = null;
        MySqlTransaction? transaction = null;
        
        try
        {
            conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            transaction = await conn.BeginTransactionAsync();

            // Insert payment
            using var cmdPayment = conn.CreateCommand();
            cmdPayment.Transaction = transaction;
            cmdPayment.CommandText = @"INSERT INTO payment
(establishment_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status)
VALUES (@establishment_id, @payment_date, @amount_paid, @payment_method, @receipt_number, @created_by, @created_date, @last_update, @status);";

            cmdPayment.Parameters.AddWithValue("@establishment_id", t.EstablishmentId);
            cmdPayment.Parameters.AddWithValue("@payment_date", t.PaymentDate);
            cmdPayment.Parameters.AddWithValue("@amount_paid", t.AmountPaid);
            cmdPayment.Parameters.AddWithValue("@payment_method", t.PaymentMethod);
            cmdPayment.Parameters.AddWithValue("@receipt_number", t.ReceiptNumber);
            cmdPayment.Parameters.AddWithValue("@created_by", t.CreatedBy);

            var createdDate = t.CreatedDate == default ? DateTime.Now : t.CreatedDate;
            var lastUpdate = t.UpdateDate == default ? DateTime.Now : t.UpdateDate;

            cmdPayment.Parameters.AddWithValue("@created_date", createdDate);
            cmdPayment.Parameters.AddWithValue("@last_update", lastUpdate);
            cmdPayment.Parameters.AddWithValue("@status", 1);

            await cmdPayment.ExecuteNonQueryAsync();

            var paymentId = Convert.ToInt32(cmdPayment.LastInsertedId);
            if (paymentId <= 0)
            {
                await transaction.RollbackAsync();
                return Result<int>.Failure("InsertFailed");
            }

            // Insert category_payment items
            if (t.Items != null && t.Items.Any())
            {
                foreach (var item in t.Items)
                {
                    using var cmdItem = conn.CreateCommand();
                    cmdItem.Transaction = transaction;
                    cmdItem.CommandText = @"INSERT INTO category_payment
(payment_id, category_id, quantity, unit_price)
VALUES (@payment_id, @category_id, @quantity, @unit_price);";

                    cmdItem.Parameters.AddWithValue("@payment_id", paymentId);
                    cmdItem.Parameters.AddWithValue("@category_id", item.CategoryId);
                    cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                    cmdItem.Parameters.AddWithValue("@unit_price", item.UnitPrice);

                    await cmdItem.ExecuteNonQueryAsync();
                }
            }

            await transaction.CommitAsync();
            return Result<int>.Success(paymentId);
        }
        catch (Exception ex)
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
        finally
        {
            transaction?.Dispose();
            conn?.Dispose();
        }
    }

    public async Task<Result<int>> Update(Payment.Dom.Model.Payment t)
    {
        MySqlConnection? conn = null;
        MySqlTransaction? transaction = null;
        
        try
        {
            conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            transaction = await conn.BeginTransactionAsync();

            // Update payment
            using var cmdPayment = conn.CreateCommand();
            cmdPayment.Transaction = transaction;
            cmdPayment.CommandText = @"UPDATE payment SET
establishment_id = @establishment_id,
payment_date = @payment_date,
amount_paid = @amount_paid,
payment_method = @payment_method,
receipt_number = @receipt_number,
created_by = @created_by,
last_update = @last_update,
status = @status
WHERE id = @id;";

            cmdPayment.Parameters.AddWithValue("@establishment_id", t.EstablishmentId);
            cmdPayment.Parameters.AddWithValue("@payment_date", t.PaymentDate);
            cmdPayment.Parameters.AddWithValue("@amount_paid", t.AmountPaid);
            cmdPayment.Parameters.AddWithValue("@payment_method", t.PaymentMethod);
            cmdPayment.Parameters.AddWithValue("@receipt_number", t.ReceiptNumber);
            cmdPayment.Parameters.AddWithValue("@created_by", t.CreatedBy);
            cmdPayment.Parameters.AddWithValue("@last_update", t.UpdateDate == default ? DateTime.Now : t.UpdateDate);
            cmdPayment.Parameters.AddWithValue("@status", 1);
            cmdPayment.Parameters.AddWithValue("@id", t.Id);

            var affected = await cmdPayment.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                await transaction.RollbackAsync();
                return Result<int>.Failure("NoRowsAffected");
            }

            // Delete existing category_payment items
            using var cmdDelete = conn.CreateCommand();
            cmdDelete.Transaction = transaction;
            cmdDelete.CommandText = @"DELETE FROM category_payment WHERE payment_id = @payment_id;";
            cmdDelete.Parameters.AddWithValue("@payment_id", t.Id);
            await cmdDelete.ExecuteNonQueryAsync();

            // Insert updated category_payment items
            if (t.Items != null && t.Items.Any())
            {
                foreach (var item in t.Items)
                {
                    using var cmdItem = conn.CreateCommand();
                    cmdItem.Transaction = transaction;
                    cmdItem.CommandText = @"INSERT INTO category_payment
(payment_id, category_id, quantity, unit_price)
VALUES (@payment_id, @category_id, @quantity, @unit_price);";

                    cmdItem.Parameters.AddWithValue("@payment_id", t.Id);
                    cmdItem.Parameters.AddWithValue("@category_id", item.CategoryId);
                    cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                    cmdItem.Parameters.AddWithValue("@unit_price", item.UnitPrice);

                    await cmdItem.ExecuteNonQueryAsync();
                }
            }

            await transaction.CommitAsync();
            return Result<int>.Success(affected);
        }
        catch (Exception ex)
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
        finally
        {
            transaction?.Dispose();
            conn?.Dispose();
        }
    }
    
    public async Task<Result<int>> Delete(Payment.Dom.Model.Payment t)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            // Soft delete: only update status to 0
            // category_payment records remain (for audit/restore purposes)
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
    
    // Optional: Hard delete method (if needed in the future)
    // Note: If FK has ON DELETE CASCADE, only need to delete payment
    // If FK doesn't have CASCADE, must delete category_payment first
    public async Task<Result<int>> HardDelete(int paymentId)
    {
        MySqlConnection? conn = null;
        MySqlTransaction? transaction = null;
        
        try
        {
            conn = _connectionDB.GetConnection();
            await conn.OpenAsync();
            transaction = await conn.BeginTransactionAsync();

            // Delete category_payment records first (if no CASCADE)
            using var cmdItems = conn.CreateCommand();
            cmdItems.Transaction = transaction;
            cmdItems.CommandText = @"DELETE FROM category_payment WHERE payment_id = @payment_id;";
            cmdItems.Parameters.AddWithValue("@payment_id", paymentId);
            await cmdItems.ExecuteNonQueryAsync();

            // Delete payment
            using var cmdPayment = conn.CreateCommand();
            cmdPayment.Transaction = transaction;
            cmdPayment.CommandText = @"DELETE FROM payment WHERE id = @id;";
            cmdPayment.Parameters.AddWithValue("@id", paymentId);
            
            var affected = await cmdPayment.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                await transaction.RollbackAsync();
                return Result<int>.Failure("NoRowsAffected");
            }

            await transaction.CommitAsync();
            return Result<int>.Success(affected);
        }
        catch (Exception ex)
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            
            return Result<int>.Failure($"DbError: {ex.Message}");
        }
        finally
        {
            transaction?.Dispose();
            conn?.Dispose();
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
            cmd.CommandText = @"SELECT id, establishment_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status FROM payment WHERE status=1;";

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var item = new Payment.Dom.Model.Payment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        EstablishmentId = reader.IsDBNull(reader.GetOrdinal("establishment_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("establishment_id")),
                        PaymentDate = reader.IsDBNull(reader.GetOrdinal("payment_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("payment_date")),
                        AmountPaid = reader.IsDBNull(reader.GetOrdinal("amount_paid")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount_paid")),
                        PaymentMethod = reader.IsDBNull(reader.GetOrdinal("payment_method")) ? string.Empty : reader.GetString(reader.GetOrdinal("payment_method")),
                        ReceiptNumber = reader.IsDBNull(reader.GetOrdinal("receipt_number")) ? 0 : reader.GetInt32(reader.GetOrdinal("receipt_number")),
                        CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? 0 : reader.GetInt32(reader.GetOrdinal("created_by")),
                        CreatedDate = reader.IsDBNull(reader.GetOrdinal("created_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("created_date")),
                        UpdateDate = reader.IsDBNull(reader.GetOrdinal("last_update")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("last_update")),
                        Status = reader.IsDBNull(reader.GetOrdinal("status")) ? false : reader.GetInt32(reader.GetOrdinal("status")) == 1
                    };

                    result.Add(item);
                }
            } // Reader is now closed

            // Load items for each payment
            foreach (var payment in result)
            {
                payment.Items = await LoadPaymentItems(conn, payment.Id);
            }

            return Result<List<Payment.Dom.Model.Payment>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<Payment.Dom.Model.Payment>>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<Payment.Dom.Model.Payment>> SelectById(int id)
    {
        try
        {
            using var conn = _connectionDB.GetConnection();
            await conn.OpenAsync();

            Payment.Dom.Model.Payment item;
            
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT id, establishment_id, payment_date, amount_paid, payment_method, receipt_number, created_by, created_date, last_update, status
                                FROM payment
                                WHERE id = @id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        return Result<Payment.Dom.Model.Payment>.Failure("NotFound");
                    }

                    item = new Payment.Dom.Model.Payment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        EstablishmentId = reader.IsDBNull(reader.GetOrdinal("establishment_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("establishment_id")),
                        PaymentDate = reader.IsDBNull(reader.GetOrdinal("payment_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("payment_date")),
                        AmountPaid = reader.IsDBNull(reader.GetOrdinal("amount_paid")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount_paid")),
                        PaymentMethod = reader.IsDBNull(reader.GetOrdinal("payment_method")) ? string.Empty : reader.GetString(reader.GetOrdinal("payment_method")),
                        ReceiptNumber = reader.IsDBNull(reader.GetOrdinal("receipt_number")) ? 0 : reader.GetInt32(reader.GetOrdinal("receipt_number")),
                        CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? 0 : reader.GetInt32(reader.GetOrdinal("created_by")),
                        CreatedDate = reader.IsDBNull(reader.GetOrdinal("created_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("created_date")),
                        UpdateDate = reader.IsDBNull(reader.GetOrdinal("last_update")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("last_update")),
                        Status = reader.IsDBNull(reader.GetOrdinal("status")) ? false : reader.GetInt32(reader.GetOrdinal("status")) == 1
                    };
                }
            } // Reader is now closed

            // Load payment items
            item.Items = await LoadPaymentItems(conn, id);

            return Result<Payment.Dom.Model.Payment>.Success(item);
        }
        catch (Exception ex)
        {
            return Result<Payment.Dom.Model.Payment>.Failure($"DbError: {ex.Message}");
        }
    }
    
    public async Task<Result<IEnumerable<Payment.Dom.Model.Payment>>> Search(string property)
    {
        const string sql = @"
        SELECT
            id,
            establishment_id,
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
            (@prop IS NOT NULL AND CAST(receipt_number AS CHAR) LIKE CONCAT('%', @prop, '%'))
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
    
            using (var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var payment = MapReaderToPayment(reader);
                    list.Add(payment);
                }
            } // Reader is now closed

            // Load items for each payment
            foreach (var payment in list)
            {
                payment.Items = await LoadPaymentItems(conn, payment.Id);
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
        return new Payment.Dom.Model.Payment
        {
            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
            EstablishmentId = reader.IsDBNull(reader.GetOrdinal("establishment_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("establishment_id")),
            PaymentDate = reader.IsDBNull(reader.GetOrdinal("payment_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("payment_date")),
            AmountPaid = reader.IsDBNull(reader.GetOrdinal("amount_paid")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount_paid")),
            PaymentMethod = reader.IsDBNull(reader.GetOrdinal("payment_method")) ? string.Empty : reader.GetString(reader.GetOrdinal("payment_method")),
            ReceiptNumber = reader.IsDBNull(reader.GetOrdinal("receipt_number")) ? 0 : reader.GetInt32(reader.GetOrdinal("receipt_number")),
            CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? 0 : reader.GetInt32(reader.GetOrdinal("created_by")),
            CreatedDate = reader.IsDBNull(reader.GetOrdinal("created_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("created_date")),
            UpdateDate = reader.IsDBNull(reader.GetOrdinal("last_update")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("last_update")),
            Status = reader.IsDBNull(reader.GetOrdinal("status")) ? false : reader.GetInt32(reader.GetOrdinal("status")) == 1
        };
    }

    private async Task<List<CategoryPayment>> LoadPaymentItems(MySqlConnection conn, int paymentId)
    {
        var items = new List<CategoryPayment>();
        
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT payment_id, category_id, quantity, unit_price 
                           FROM category_payment 
                           WHERE payment_id = @payment_id;";
        cmd.Parameters.AddWithValue("@payment_id", paymentId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new CategoryPayment
            {
                PaymentId = reader.GetInt32(reader.GetOrdinal("payment_id")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
                Quantity = reader.GetByte(reader.GetOrdinal("quantity")),
                UnitPrice = reader.GetInt32(reader.GetOrdinal("unit_price"))
            });
        }

        return items;
    }
}
