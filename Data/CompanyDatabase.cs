using ErpCli.Models;
using ErpCli.Helpers;
using Microsoft.Data.SqlClient;

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
            using SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand addrCmd = connection.CreateCommand();
            addrCmd.Transaction = transaction;
            addrCmd.CommandText = @"INSERT INTO Addresses (Street, Number, PostalCode, City, Country)
                                    OUTPUT INSERTED.Id
                                    VALUES (@street, @number, @postalCode, @city, @country) ";
            BindAddressParameters(addrCmd, company);
            int addressId = (int)addrCmd.ExecuteScalar();

            SqlCommand compCmd = connection.CreateCommand();
            compCmd.Transaction = transaction;
            compCmd.CommandText = @"INSERT INTO Companies (Name, Currency, AddressId)
                                    VALUES (@name, @currency, @addressId)";
            BindCompanyParameters(compCmd, company, addressId);
            compCmd.ExecuteNonQuery();
        }

        public void UpdateCompany(Company updatedCompany)
        {
            using SqlConnection connection = GetConnection();
            using SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"UPDATE Addresses
                                SET Street = @street,
                                    Number = @number,
                                    PostalCode = @postalCode,
                                    City = @city,
                                    Country = @country
                                FROM Addresses address
                                INNER JOIN Companies company ON company.AddressId = address.Id
                                WHERE company.Id = @id;

                                UPDATE Companies
                                SET Name = @name,
                                    Currency = @currency
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", updatedCompany.Id);
            cmd.Parameters.AddWithValue("@name", updatedCompany.Name);
            cmd.Parameters.AddWithValue("@currency", updatedCompany.Currency);
            BindAddressParameters(cmd, updatedCompany);
            cmd.ExecuteNonQuery();
            cmd.Transaction.Commit();
          
            
        }
        public void DeleteCompany(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE company
                                FROM Companies company
                                WHERE company.Id = @id;
                                DELETE address
                                FROM Addresses address
                                LEFT JOIN Companies company ON company.AddressId = address.Id
                                WHERE company.Id IS NULL";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
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

        private static void BindAddressParameters(SqlCommand cmd, Company company)
        {
            cmd.Parameters.AddWithValue("@street", company.Street);
            cmd.Parameters.AddWithValue("@number", company.Number);
            cmd.Parameters.AddWithValue("@postalCode", company.PostalCode);
            cmd.Parameters.AddWithValue("@city", company.City);
            cmd.Parameters.AddWithValue("@country", company.Country);
        }
        private static void BindCompanyParameters(SqlCommand cmd, Company company, int addressId)
        {
            cmd.Parameters.AddWithValue("@name", company.Name);
            cmd.Parameters.AddWithValue("@currency", company.Currency);
            cmd.Parameters.AddWithValue("@addressId", addressId);
        }
    }

}
