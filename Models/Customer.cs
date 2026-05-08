namespace ErpCli.Models
{
    public class Customer : Person
    {
        public int CustomerId { get; set; }
        public DateTime? LastPurchaseAt { get; set; }
    }
}