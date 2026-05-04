namespace ErpCli.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public override string ToString() => FullName ?? "";
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public Address? Address { get; set; }
        public string? Street
        {
            get => Address?.Street;
            set
            {
                if (Address == null)
                {
                    Address = new Address();
                }
                Address.Street = value;
            }
        }
        public string? Number
        {
            get => Address?.Number;
            set
            {
                if (Address == null)
                {
                    Address = new Address();
                }
                Address.Number = value;
            }
        }
        public string? PostalCode
        {
            get => Address?.PostalCode;
            set
            {
                if (Address == null)
                {
                    Address = new Address();
                }
                Address.PostalCode = value;
            }
        }
        public string? City
        {
            get => Address?.City;
            set
            {
                if (Address == null)
                {
                    Address = new Address();
                }
                Address.City = value;
            }
        }
        public string? Country
        {
            get => Address?.Country;
            set
            {
                if (Address == null)
                {
                    Address = new Address();
                }
                Address.Country = value;
            }
        }
    }
}