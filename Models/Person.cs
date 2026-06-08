using System.Data.SqlTypes;

namespace ErpCli.Models
{
    public class Person
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int AddressId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public override string ToString() => FullName;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public Address Address { get; set; } = new();
        public string Street
        {
            get => Address.Street;
            set => Address.Street = value ?? string.Empty;
        }
        public string Number
        {
            get => Address.Number;
            set => Address.Number = value ?? string.Empty;
        }
        public string PostalCode
        {
            get => Address.PostalCode;
            set => Address.PostalCode = value ?? string.Empty;
        }
        public string City
        {
            get => Address.City;
            set => Address.City = value ?? string.Empty;
        }
        public string Country
        {
            get => Address.Country;
            set => Address.Country = value ?? string.Empty;
        }
        public Company? Company { get; set; }
        public string CompanyName
        {
            get => Company?.Name ?? string.Empty;
            set
            {
                if (Company == null)
                {
                    Company = new Company();
                }
                Company.Name = value ?? string.Empty;
            }
        }

    }
}
