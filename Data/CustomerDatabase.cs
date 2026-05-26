using ErpCli.Models;
using ErpCli.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

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
        public bool AddCustomer(Customer customer)
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
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value =
                    (object?)customer.LastPurchaseAt ?? DBNull.Value;

                customerCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ExceptionHelper.ExceptionText(ex, "Fejl ved oprettelse af kunde");
                Console.ReadKey(true);
                return false;
                
            }
        }

        /// <summary>
        /// Updates the customer, person and address rows matching each of their corresponding IDs (Id, Id and Address.Id) with the values from the given customer.
        /// No-op if no row matches.
        /// </summary>
        public bool UpdateCustomer(Customer updatedCustomer)
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
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ExceptionHelper.ExceptionText(ex, "Fejl ved opdatering af kunde");
                Console.ReadKey(true);
                return false;
            }
        }

        /// <summary>
        /// Deletes the Person with the given Id. No-op if no row matches.
        /// </summary>
        public bool DeleteCustomerById(int id)
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
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ExceptionHelper.ExceptionText(ex, "Fejl ved sletning af kunde");
                Console.ReadKey(true);
                return false;
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
        private static void BindAddressParameters(SqlCommand cmd, Customer c)
        {
            cmd.Parameters.AddWithValue("@Street", c.Street);
            cmd.Parameters.AddWithValue("@Number", c.Number);
            cmd.Parameters.AddWithValue("@PostalCode", c.PostalCode);
            cmd.Parameters.AddWithValue("@City", c.City);
            cmd.Parameters.AddWithValue("@Country", c.Country);
        }
        private static void BindPersonParameters(SqlCommand cmd, Customer c)
        {
            cmd.Parameters.AddWithValue("@FirstName", c.FirstName);
            cmd.Parameters.AddWithValue("@LastName", c.LastName);
            cmd.Parameters.AddWithValue("@Phone", c.Phone);
            cmd.Parameters.AddWithValue("@Email", c.Email);
        }
    }

}
