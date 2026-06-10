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

        public void CreateCompany(Company company)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);

            int addressId = GetOrCreateAddressId(company.Address, connection, transaction);

            using SqlCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"INSERT INTO Companies (Name, Currency, AddressId)
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

            // Get existing address ID to determine if it can be reused or if a new one must be created.
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
                    throw new InvalidOperationException($"Virksomheden med Id {updatedCompany.Id} findes ikke.");
                }
                oldAddressId = Convert.ToInt32(result);
            }

            // Get or create the new address ID. This will handle both cases where the address is unchanged (reusing the same ID) or updated (creating a new address if necessary).
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
                    throw new InvalidOperationException($"Virksomheden med Id {updatedCompany.Id} findes ikke.");
                }
                DeleteAddressIfNotReferenced(oldAddressId, connection, transaction);
            }
            transaction.Commit();
        }
        public void DeleteCompany(int id )
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);

            int oldAddressId;
            using (SqlCommand existing = connection.CreateCommand())
            {
                existing.Transaction = transaction;
                existing.CommandText = @"SELECT AddressId FROM Companies WHERE Id = @id";
                existing.Parameters.AddWithValue("@id", id);
                object? result = existing.ExecuteScalar();

                if (result is null)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("Virksomheden findes ikke.");
                }
                oldAddressId = Convert.ToInt32(result);
            }

            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"DELETE FROM Persons WHERE CompanyId = @id;
                                        DELETE FROM Companies WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("Virksomheden findes ikke.");
                }
                DeleteAddressIfNotReferenced(oldAddressId, connection, transaction);
            }
            transaction.Commit();
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
