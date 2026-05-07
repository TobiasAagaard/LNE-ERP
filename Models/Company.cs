namespace ErpCli.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Currency Currency { get; set; }
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
    }
}
