using ErpCli.Models;
using Microsoft.Data.SqlClient;
namespace ErpCli.Data
{
    public partial class Database
    {
        List<Company> Companies = new List<Company>()
        {
            new Company { Id = 1, Name = "NovoNordisk", Street = "Vinkelvej", Number = "142", PostalCode = "9000", City = "Aalborg", Country = "Danmark", Currency = Currency.DKK },
            new Company { Id = 2, Name = "Microsoft", Street = "Micro Street", Number = "2", PostalCode = "98033", City = "Redmond", Country = "USA", Currency = Currency.USD },
        };

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
            addrCmd.CommandText = @"INSERT INTO Adresses (Street, Number, PostalCode, City, Country)
                                    OUTPUT INSERTED.Id
                                    VALUES (@street, @number, @postalCode, @city, @country) ";
            
        }

        public void UpdateCompany(Company updatedCompany)
        {
            for (int i = 0; i < Companies.Count; i++)
            {
                Company company = Companies[i];
                if (company.Id == updatedCompany.Id)
                {
                    Companies[i] = updatedCompany;
                }
            }
        }
        public void DeleteCompany(int id)
        {
            for (int i = 0; i < Companies.Count; i++)
            {
                Company company = Companies[i];
                if (company.Id == id)
                {
                    Companies.RemoveAt(i);
                }
            }
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
                Currency = (Currency)reader.GetInt32(7)
            };
        }

        private static void BindCompanyParameters(SqlCommand cmd, Company company)
        {
            cmd.Parameters.AddWithValue("@name", (object?)company.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addressId", company.Address != null ? (object)company.Address.Id : DBNull.Value);
            cmd.Parameters.AddWithValue("@currency", (int)company.Currency);
        }
    }

}
