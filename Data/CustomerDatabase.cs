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
            cmd.CommandText = @"SELECT  Persons.Id, Customers.Id, LastPurchaseAt, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country, CompanyId, Companies.Name
                                FROM Customers
                                JOIN Persons
                                ON Customers.PersonId = Persons.Id
                                JOIN Addresses
                                ON Persons.AddressId = Addresses.Id
                                JOIN Companies
                                ON Customers.CompanyId = Companies.Id
                                WHERE Customers.Id = @id;";
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
            cmd.CommandText = @"SELECT  Persons.Id, Customers.Id, LastPurchaseAt, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country, CompanyId, Companies.Name
                                FROM Customers
                                JOIN Persons
                                ON Customers.PersonId = Persons.Id
                                JOIN Addresses
                                ON Persons.AddressId = Addresses.Id
                                JOIN Companies
                                ON Customers.CompanyId = Companies.Id;";
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                customers.Add(ReadCustomer(reader));
            return customers;
        }

        /// <summary>
        /// Inserts a new customer.
        /// </summary>
        public bool CreateCustomer(Customer customer)
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

                customerCmd.CommandText = @"INSERT INTO Customers (PersonId, LastPurchaseAt, CompanyId)
                                            VALUES (@PersonId, @LastPurchaseAt, @CompanyId);";
                customerCmd.Parameters.Add("@PersonId", SqlDbType.Int).Value = personId;
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value = (object?)customer.LastPurchaseAt ?? DBNull.Value;
                customerCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = (object)customer.CompanyId;
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
                int oldAddressId;
                using SqlCommand existing = connection.CreateCommand();
                existing.Transaction = transaction;
                existing.CommandText = @"SELECT p.AddressId
                                         FROM Customers c
                                         INNER JOIN Persons p ON p.Id = c.PersonId
                                         WHERE p.Id = @id";
                existing.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;
                object? result = existing.ExecuteScalar();
                if (result is null)
                {
                    throw new InvalidOperationException($"Kunden med Id {updatedCustomer.Id} findes ikke.");
                }
                oldAddressId = Convert.ToInt32(result);

                int addressId = GetOrCreateAddressId(updatedCustomer.Address, connection, transaction);

                using SqlCommand customerCmd = connection.CreateCommand();
                customerCmd.Transaction = transaction;
                customerCmd.CommandText = @"UPDATE Customers
                                            SET LastPurchaseAt = @LastPurchaseAt,
                                                CompanyId = @CompanyId
                                            FROM Customers c
                                            INNER JOIN Persons p ON p.Id = c.PersonId
                                            WHERE p.Id = @id;";
                customerCmd.Parameters.Add("@LastPurchaseAt", SqlDbType.DateTime2).Value =
                    (object?)updatedCustomer.LastPurchaseAt ?? DBNull.Value;
                customerCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = (object)updatedCustomer.CompanyId;
                customerCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                if (customerCmd.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException($"Kunden med Id {updatedCustomer.Id} findes ikke.");
                }

                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;
                personCmd.CommandText = @"UPDATE Persons
                                          SET FirstName = @FirstName,
                                              LastName = @LastName,
                                              Phone = @Phone,
                                              Email = @Email,
                                              AddressId = @AddressId
                                          WHERE Id = @id;";
                personCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = updatedCustomer.FirstName;
                personCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = updatedCustomer.LastName;
                personCmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = updatedCustomer.Phone;
                personCmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = updatedCustomer.Email;
                personCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;
                personCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                personCmd.ExecuteNonQuery();

                DeleteAddressIfNotReferenced(oldAddressId, connection, transaction);

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
                int personId;
                int oldAddressId;

                using SqlCommand existing = connection.CreateCommand();
                existing.Transaction = transaction;
                existing.CommandText = @"SELECT p.Id, p.AddressId
                                         FROM Customers c
                                         JOIN Persons p ON c.PersonId = p.Id
                                         WHERE c.Id = @id";
                existing.Parameters.Add("@id", SqlDbType.Int).Value = id;
                using (SqlDataReader reader = existing.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new InvalidOperationException($"Kunden med Id {id} findes ikke.");
                    personId = reader.GetInt32(0);
                    oldAddressId = reader.GetInt32(1);
                }

                using SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"DELETE FROM Persons WHERE Id = @id";
                command.Parameters.Add("@id", SqlDbType.Int).Value = personId;
                command.ExecuteNonQuery();

                DeleteAddressIfNotReferenced(oldAddressId, connection, transaction);
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
        /// Email, Street, Number, PostalCode, City, CompanyId.
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
                Country         = reader.GetString(11),
                
                CompanyId       = reader.GetInt32(12),
                Company         = new Company
                {
                    Id = reader.GetInt32(12),
                    Name = reader.GetString(13)
                }
            };
        }
    }

}
