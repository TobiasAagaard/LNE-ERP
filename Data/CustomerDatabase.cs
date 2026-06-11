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
        public Person? GetCustomerById(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT  Persons.Id, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country, Persons.CompanyId, Companies.Name
                                FROM Persons
                                JOIN Addresses
                                ON Persons.AddressId = Addresses.Id
                                JOIN Companies
                                ON Persons.CompanyId = Companies.Id
                                WHERE Persons.Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                return ReadCustomer(reader);
            return null;
        }

        /// <summary>
        /// Returns all customers (incl. person and address) from the Customers table. Empty list if the table is empty.
        /// </summary>
        public List<Person> GetAllCustomers()
        {
            List<Person> customers = new();
            using SqlConnection connection = GetConnection();
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT  Persons.Id, FirstName, LastName, Phone, Email, Street, Number, PostalCode, City, Country, Persons.CompanyId, Companies.Name
                                FROM Persons
                                JOIN Addresses
                                ON Persons.AddressId = Addresses.Id
                                JOIN Companies
                                ON Persons.CompanyId = Companies.Id;";
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                customers.Add(ReadCustomer(reader));
            return customers;
        }

        /// <summary>
        /// Inserts a new customer.
        /// </summary>
        public bool CreateCustomer(Person customer)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int addressId = GetCompanyAddressId(customer.CompanyId, connection, transaction);

                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;

                personCmd.CommandText = @"INSERT INTO Persons (FirstName, LastName, Phone, Email, AddressId, CompanyId)
                                        VALUES (@FirstName, @LastName, @Phone, @Email, @AddressId, @CompanyId);
                                        SELECT SCOPE_IDENTITY();";

                personCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = customer.FirstName;
                personCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = customer.LastName;
                personCmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = customer.Phone;
                personCmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = customer.Email;
                personCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;
                personCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = customer.CompanyId;
                personCmd.ExecuteScalar();

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
        public bool UpdateCustomer(Person updatedCustomer)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

           
            try
            {
                int oldAddressId;
                using SqlCommand existing = connection.CreateCommand();
                existing.Transaction = transaction;
                existing.CommandText = @"SELECT AddressId
                                         FROM Persons
                                         WHERE Id = @id";
                existing.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;
                object? result = existing.ExecuteScalar();
                if (result is null)
                {
                    throw new InvalidOperationException($"Kunden med Id {updatedCustomer.Id} findes ikke.");
                }
                oldAddressId = Convert.ToInt32(result);

                int addressId = GetCompanyAddressId(updatedCustomer.CompanyId, connection, transaction);

                using SqlCommand personCmd = connection.CreateCommand();
                personCmd.Transaction = transaction;
                personCmd.CommandText = @"UPDATE Persons
                                          SET FirstName = @FirstName,
                                              LastName = @LastName,
                                              Phone = @Phone,
                                              Email = @Email,
                                              AddressId = @AddressId,
                                              CompanyId = @CompanyId
                                          WHERE Id = @id;";
                personCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = updatedCustomer.FirstName;
                personCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = updatedCustomer.LastName;
                personCmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = updatedCustomer.Phone;
                personCmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = updatedCustomer.Email;
                personCmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = addressId;
                personCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = updatedCustomer.CompanyId;
                personCmd.Parameters.Add("@id", SqlDbType.Int).Value = updatedCustomer.Id;

                if (personCmd.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException($"Kunden med Id {updatedCustomer.Id} findes ikke.");
                }

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
                existing.CommandText = @"SELECT Id, AddressId
                                         FROM Persons
                                         WHERE Id = @id";
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
        /// the SELECT statements in this file: Id, FirstName, LastName, Phone, Email,
        /// Street, Number, PostalCode, City, Country, CompanyId, CompanyName.
        /// </summary>
        private static Person ReadCustomer(SqlDataReader reader)
        {
            return new Person
            {
                Id              = reader.GetInt32(0),

                FirstName       = reader.GetString(1),
                LastName        = reader.GetString(2),
                Phone           = reader.GetString(3),
                Email           = reader.GetString(4),

                Street          = reader.GetString(5),
                Number          = reader.GetString(6),
                PostalCode      = reader.GetString(7),
                City            = reader.GetString(8),
                Country         = reader.GetString(9),

                Company         = new Company
                {
                    Id = reader.GetInt32(10),
                    Name = reader.GetString(11)
                }
            };
        }
    }

}
