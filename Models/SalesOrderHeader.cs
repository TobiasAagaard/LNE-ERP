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

        public Company? Company { get; set; }
        public Person? ContactPerson { get; set; }

        public int CompanyId => Company?.Id ?? throw new InvalidOperationException("Firmaet skal være valgt");
        public string CompanyName => Company?.Name ?? throw new InvalidOperationException("Firmaet skal være valgt");
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