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
        public void CreateCustomer(Customer customer)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            int addressId = GetOrCreateAddressId(customer.Address, connection, transaction);
            try
            {
                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;

                personCmd.CommandText = @"INSERT INTO Persons (FirstName, LastName, Phone, Email, AddressId)
                                        VALUES (@FirstName, @LastName, @Phone, @Email, @AddressId);
                                        SELECT SCOPE_IDENTITY();";
                personCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = customer.FirstName;
                personCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = customer.LastName;
                personCmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = customer.Phone;
                personCmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = customer.Email;
                personCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;
                int personId = Convert.ToInt32(personCmd.ExecuteScalar());

                using SqlCommand customerCmd = connection.CreateCommand();
                customerCmd.Transaction = transaction;

                customerCmd.CommandText = @"INSERT INTO Customers (PersonId, LastPurchaseAt)
                                            VALUES (@PersonId, @LastPurchaseAt);";
                customerCmd.Parameters.Add("@PersonId", SqlDbType.Int).Value = personId;
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value = (object?)customer.LastPurchaseAt ?? DBNull.Value;
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
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value =
                    (object?)updatedCustomer.LastPurchaseAt ?? DBNull.Value;
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
                personCmd.Parameters.AddWithValue("@FirstName", updatedCustomer.FirstName);
                personCmd.Parameters.AddWithValue("@LastName", updatedCustomer.LastName);
                personCmd.Parameters.AddWithValue("@Phone", updatedCustomer.Phone);
                personCmd.Parameters.AddWithValue("@Email", updatedCustomer.Email);
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
                addressCmd.Parameters.AddWithValue("@Street", updatedCustomer.Street);
                addressCmd.Parameters.AddWithValue("@Number", updatedCustomer.Number);
                addressCmd.Parameters.AddWithValue("@City", updatedCustomer.City);
                addressCmd.Parameters.AddWithValue("@Country", updatedCustomer.Country);
                addressCmd.Parameters.AddWithValue("@PostalCode", updatedCustomer.PostalCode);
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
                Id              = reader.GetInt32(0),
                CustomerId      = reader.GetInt32(1),
                LastPurchaseAt  = reader.IsDBNull(2) ? null : reader.GetDateTime(2),

                FirstName       = reader.GetString(3),
                LastName        = reader.GetString(4),
                Phone           = reader.GetString(5),
                Email           = reader.GetString(6),

                Street          = reader.GetString(7),
                Number          = reader.GetString(8),
                PostalCode      = reader.GetString(9),
                City            = reader.GetString(10),
                Country         = reader.GetString(11)
            };
        }
    }

}
