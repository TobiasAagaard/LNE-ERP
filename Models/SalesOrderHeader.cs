namespace ErpCli.Models
{
    public class SalesOrderHeader
    {
        public int OrderNumber { get; set;}
        public DateTime OrderCreatedAt { get; set; }
        public DateTime? OrderCompletedAt { get; set;}
        public OrderStatus Status { get; set; }
        public List<int> OrderLineIdList = new List<int>();
        public List<OrderLine> OrderLineList = new List<OrderLine>();
        public decimal? OrderTotal =>
            OrderLineList.Sum(orderLine => orderLine.Quantity * (orderLine.Product?.Price ?? 0));

        // The company is the debtor that gets billed (required); the contact person
        // is the individual at that company who placed the order (optional).
        public Company? Company { get; set; }
        public Person? ContactPerson { get; set; }

        // Read-only helpers used by the list/details views for display.
        public int CompanyId => Company?.Id ?? 0;
        public string CompanyName => Company?.Name ?? string.Empty;
        public string ContactPersonName => ContactPerson?.FullName ?? string.Empty;

        public enum OrderStatus
        {
            Ingen,
            Oprettet,
            Bekræftet,
            Pakket,
            Færdig
        }
    }
}