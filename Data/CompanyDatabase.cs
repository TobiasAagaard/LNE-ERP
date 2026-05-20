using ErpCli.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ErpCli.Data
{
    public partial class Database
    {
        public Company? GetCompanyById(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT c.Id, c.Name, a.Street, a.Number, a.PostalCode, a.City, a.Country, c.Currency
                                FROM Companies c
                                INNER JOIN Addresses a ON c.AddressId = a.Id
                                WHERE c.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return ReadCompany(reader);
            }
            return null;
        }

        public List<Company> GetAllCompanies()
        {
            List<Company> companies = new();
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT c.Id, c.Name, a.Street, a.Number, a.PostalCode, a.City, a.Country, c.Currency
                                FROM Companies c
                                INNER JOIN Addresses a ON c.AddressId = a.Id";
            
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                companies.Add(ReadCompany(reader));
            }
            return companies;
        }

        public void AddCompany(Company company)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);

            int addressId = GetOrCreateAddressId(company.Address, connection, transaction);

            using SqlCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"INSERT INTO COMPANIES (Name, Currency, AddressId)
                                    VALUES (@name, @currency, @addressId)";
            command.Parameters.AddWithValue("@name", company.Name);
            command.Parameters.AddWithValue("@currency", company.Currency);
            command.Parameters.AddWithValue("@addressId", addressId);
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void UpdateCompany(Company updatedCompany)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);

            // Ensure the company exists and capture its currently linked AddressId.
            int oldAddressId;
            using (SqlCommand existing = connection.CreateCommand())
            {
                existing.Transaction = transaction;
                existing.CommandText = @"SELECT AddressId FROM Companies WHERE Id = @id";
                existing.Parameters.AddWithValue("@id", updatedCompany.Id);
                object? result = existing.ExecuteScalar();
                if (result is null)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException($"Company with Id {updatedCompany.Id} does not exist.");
                }
                oldAddressId = Convert.ToInt32(result);
            }

            // Resolve the address; GetOrCreateAddressId reuses the existing row when unchanged.
            int addressId = GetOrCreateAddressId(updatedCompany.Address, connection, transaction);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"UPDATE Companies
                                        SET Name = @name,
                                            Currency = @currency,
                                            AddressId = @addressId
                                        WHERE Id = @id";
                command.Parameters.AddWithValue("@id", updatedCompany.Id);
                command.Parameters.AddWithValue("@name", updatedCompany.Name);
                command.Parameters.AddWithValue("@currency", updatedCompany.Currency);
                command.Parameters.AddWithValue("@addressId", addressId);
                if (command.ExecuteNonQuery() == 0)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException($"Company with Id {updatedCompany.Id} could not be updated.");
                }
            }

            // If the address changed, delete the old one when no other company references it.
            if (addressId != oldAddressId)
            {
                int references;
                using (SqlCommand countCmd = connection.CreateCommand())
                {
                    countCmd.Transaction = transaction;
                    countCmd.CommandText = @"SELECT COUNT(*) FROM Companies WHERE AddressId = @oldId";
                    countCmd.Parameters.AddWithValue("@oldId", oldAddressId);
                    references = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                if (references == 0)
                {
                    using SqlCommand deleteCmd = connection.CreateCommand();
                    deleteCmd.Transaction = transaction;
                    deleteCmd.CommandText = @"DELETE FROM Addresses WHERE Id = @oldId";
                    deleteCmd.Parameters.AddWithValue("@oldId", oldAddressId);
                    deleteCmd.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
        public void DeleteCompany(int id)
        {
            using SqlConnection connection = GetConnection();
            using SqlCommand command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM Companies WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        private Company ReadCompany(SqlDataReader reader)
        {
            return new Company
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Street = reader.GetString(2),
                Number = reader.GetString(3),
                PostalCode = reader.GetString(4),
                City = reader.GetString(5),
                Country = reader.GetString(6),
                Currency = (Currency)Enum.Parse(typeof(Currency), reader.GetString(7))
            };
        }
    }

}
