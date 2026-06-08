using ErpCli.Models;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Pkix;

namespace ErpCli.Data
{
    public partial class Database
    {
        public SalesOrderHeader? GetSalesOrderHeader(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlCommand headerCmd = connection.CreateCommand();
            headerCmd.CommandText = @"SELECT OrderNumber, OrderCreatedAt, OrderCompletedAt, ContactPersonId, FirstName, LastName, Status
                                        FROM SalesOrderHeaders
                                        INNER JOIN Persons p
                                        ON ContactPersonId = p.Id
                                        WHERE OrderNumber = @id;";
            headerCmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = headerCmd.ExecuteReader();
            if (reader.Read())
                return ReadHeader(reader);
            return null;
        }
        public List<SalesOrderHeader> GetSalesOrderHeaders()
        {
            List<SalesOrderHeader> headers = new();
            using SqlConnection connection = GetConnection();
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT OrderNumber, OrderCreatedAt, OrderCompletedAt, ContactPersonId, FirstName, LastName, Status
                                FROM SalesOrderHeaders
                                INNER JOIN Persons p
                                ON ContactPersonId = p.Id;";      
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                headers.Add(ReadHeader(reader));
            return headers;
        }
        public void AddSalesOrderHeader(SalesOrderHeader SalesOrderHeader)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"INSERT INTO SalesOrderHeaders (OrderCreatedAt, OrderCompletedAt, ContactPersonId, Status)
                                    VALUES (@OrderCreatedAt, @OrderCompletedAt, @ContactPersonId, @Status)";
                BindHeaderParameters(cmd, SalesOrderHeader);
                cmd.ExecuteNonQuery();

                using SqlCommand customerCmd = connection.CreateCommand();
                customerCmd.Transaction = transaction;
                customerCmd.CommandText = @"UPDATE p
                                            SET p.LastPurchaseAt = @LastPurchaseAt
                                            FROM Persons p
                                            INNER JOIN SalesOrderHeaders s ON s.ContactPersonId = p.Id
                                            WHERE p.Id = @id;";
                customerCmd.Parameters.AddWithValue("@LastPurchaseAt", DateTime.Now);
                customerCmd.Parameters.AddWithValue("@id", SalesOrderHeader.ContactPersonId);
                customerCmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void UpdateSalesOrderHeader(SalesOrderHeader editSalesOrderHeader)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"UPDATE SalesOrderHeaders
                                    SET OrderCreatedAt = @OrderCreatedAt,
                                        OrderCompletedAt = @OrderCompletedAt,
                                        ContactPersonId = @ContactPersonId,
                                        Status = @Status
                                    WHERE OrderNumber = @id;";
                cmd.Parameters.AddWithValue("@id", editSalesOrderHeader.OrderNumber);
                BindHeaderParameters(cmd, editSalesOrderHeader);
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void DeleteSalesOrderHeader(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"DELETE header
                                    FROM SalesOrderHeaders header
                                    WHERE OrderNumber = @id;";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        private static SalesOrderHeader ReadHeader(SqlDataReader reader)
        {
            return new SalesOrderHeader
            {
                OrderNumber         = reader.GetInt32(0),
                OrderCreatedAt      = reader.GetDateTime(1),
                OrderCompletedAt    = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                ContactPersonId     = reader.GetInt32(3),
                FirstName           = reader.GetString(4),
                LastName            = reader.GetString(5),
                Status              = (SalesOrderHeader.OrderStatus)reader.GetInt32(6)
            };
        }
        private static void BindHeaderParameters(SqlCommand cmd, SalesOrderHeader h)
        {
            cmd.Parameters.AddWithValue("@OrderNumber", (object?)h.OrderNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderCreatedAt", (object?)h.OrderCreatedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderCompletedAt", (object?)h.OrderCompletedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactPersonId", (object?)h.ContactPersonId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object?)h.Status ?? DBNull.Value);
        }
    }   
}