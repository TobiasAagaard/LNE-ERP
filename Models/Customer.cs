namespace ErpCli.Models
{
    public class Customer : Person
    {
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public DateTime? LastPurchaseAt { get; set; }
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