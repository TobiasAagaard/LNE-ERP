using ErpCli.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ErpCli.Data
{
    public partial class Database
    {
        /// <summary>
        /// Returns the customer (incl. person and address) with the given Id, or null if no such customer exists.
        /// </summary>
        public Customer? GetCustomerById(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT  p.Id, c.Id, LastPurchaseAt, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country
                                FROM Customers c
                                JOIN Persons p
                                ON c.PersonId = p.Id
                                JOIN Addresses
                                ON AddressId = Addresses.Id
                                WHERE c.Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                return ReadCustomer(reader);
            return null;
        }

        /// <summary>
        /// Returns all customers (incl. person and address) from the Customers table. Empty list if the table is empty.
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new();
            using SqlConnection connection = GetConnection();
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT p.Id, c.Id, LastPurchaseAt, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country
                                FROM Customers c
                                JOIN Persons p
                                ON c.PersonId = p.Id
                                JOIN Addresses
                                ON AddressId = Addresses.Id;";
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                customers.Add(ReadCustomer(reader));
            return customers;
        }

        /// <summary>
        /// Inserts a new customer.
        /// </summary>
        public void AddCustomer(Customer customer)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using SqlCommand addressCmd = connection.CreateCommand();
                addressCmd.Transaction = transaction;
                addressCmd.CommandText = @"INSERT INTO Addresses (Street, Number, PostalCode, City, Country)
                                        VALUES (@Street, @Number, @PostalCode, @City, @Country);
                                        SELECT SCOPE_IDENTITY();";
                BindAddressParameters(addressCmd, customer);
                int addressId = Convert.ToInt32(addressCmd.ExecuteScalar());

                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;

                personCmd.CommandText = @"INSERT INTO Persons (FirstName, LastName, Phone, Email, AddressId)
                                        VALUES (@FirstName, @LastName, @Phone, @Email, @AddressId);
                                        SELECT SCOPE_IDENTITY();";
                BindPersonParameters(personCmd, customer);
                personCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;
                int personId = Convert.ToInt32(personCmd.ExecuteScalar());

                using SqlCommand customerCmd = connection.CreateCommand();
                customerCmd.Transaction = transaction;

                customerCmd.CommandText = @"INSERT INTO Customers (PersonId, LastPurchaseAt)
                                            VALUES (@PersonId, @LastPurchaseAt);";
                customerCmd.Parameters.Add("@PersonId", SqlDbType.Int).Value = personId;
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value = customer.LastPurchaseAt;

                customerCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Updates the customer, person and address rows matching each of their corresponding IDs (Id, Id and Address.Id) with the values from the given customer.
        /// No-op if no row matches.
        /// </summary>
        public void UpdateCustomer(Customer updatedCustomer)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using SqlCommand customerCmd = connection.CreateCommand();
                customerCmd.Transaction = transaction;
                customerCmd.CommandText = @"UPDATE c
                                            SET c.LastPurchaseAt = @LastPurchaseAt
                                            FROM Customers c
                                            INNER JOIN Persons p ON p.Id = c.PersonId
                                            WHERE p.Id = @id;";
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value = updatedCustomer.LastPurchaseAt;
                customerCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                customerCmd.ExecuteNonQuery();

                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;
                personCmd.CommandText = @"UPDATE Persons
                                          SET FirstName = @FirstName,
                                              LastName = @LastName,
                                              Phone = @Phone,
                                              Email = @Email
                                          WHERE Id = @id;";
                BindPersonParameters(personCmd, updatedCustomer);
                personCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                personCmd.ExecuteNonQuery();

                using SqlCommand addressCmd = connection.CreateCommand();
                addressCmd.Transaction = transaction;
                addressCmd.CommandText = @"UPDATE a
                                           SET a.Street = @Street,
                                               a.Number = @Number,
                                               a.City = @City,
                                               a.Country = @Country,
                                               a.PostalCode = @PostalCode
                                           FROM Addresses a
                                           INNER JOIN Persons p ON p.AddressId = a.Id
                                           WHERE p.Id = @id;";
                BindAddressParameters(addressCmd, updatedCustomer);
                addressCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                addressCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Deletes the Person with the given Id. No-op if no row matches.
        /// </summary>
        public void DeleteCustomerById(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int addressId;

                using SqlCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"SELECT p.AddressId
                                    FROM Customers c
                                    INNER JOIN Persons p
                                    ON c.PersonId = p.Id
                                    WHERE c.Id = @Id;";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                object result = cmd.ExecuteScalar();
                addressId = Convert.ToInt32(result);

                using SqlCommand deleteCmd = connection.CreateCommand();
                deleteCmd.Transaction = transaction;
                deleteCmd.CommandText = @"DELETE FROM Addresses
                                          WHERE Id = @AddressId;";
                deleteCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;

                deleteCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Maps the current row of the given reader to a Customer. Column order must match
        /// the SELECT statements in this file: Id, LastPurchasedAt, FirstName, LastName, Phone,
        /// Email, Street, Number, PostalCode, City.
        /// </summary>
        private static Customer ReadCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                Id      = reader.GetInt32(0),
                CustomerId      = reader.GetInt32(1),
                LastPurchaseAt  = reader.GetDateTime(2),

                FirstName       = reader.IsDBNull(3) ? null : reader.GetString(3),
                LastName        = reader.IsDBNull(4) ? null : reader.GetString(4),
                Phone           = reader.IsDBNull(5) ? null : reader.GetString(5),
                Email           = reader.IsDBNull(6) ? null : reader.GetString(6),

                Street          = reader.IsDBNull(7) ? null : reader.GetString(7),
                Number          = reader.IsDBNull(8) ? null : reader.GetString(8),
                PostalCode      = reader.IsDBNull(9) ? null : reader.GetString(9),
                City            = reader.IsDBNull(10) ? null : reader.GetString(10),
                Country         = reader.IsDBNull(11) ? null : reader.GetString(11)
            };
        }
        private static void BindAddressParameters(SqlCommand cmd, Customer c)
        {
            cmd.Parameters.AddWithValue("@Street", (object?)c.Street ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Number", (object?)c.Number ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", (object?)c.PostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object?)c.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", (object?)c.Country ?? DBNull.Value);
        }
        private static void BindPersonParameters(SqlCommand cmd, Customer c)
        {
            cmd.Parameters.AddWithValue("@FirstName", (object?)c.FirstName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", (object?)c.LastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object?)c.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
        }
    }

}
