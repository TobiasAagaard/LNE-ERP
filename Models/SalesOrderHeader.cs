namespace ErpCli.Models
{
    public class SalesOrderHeader
    {
        public int OrderNumber { get; set;}
        public DateTime OrderCreatedAt { get; set; }
        public DateTime? OrderCompletedAt { get; set;}
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public List<int> OrderLineIdList = new List<int>();
        public List<OrderLine> OrderLineList = new List<OrderLine>();
        public decimal? OrderTotal =>
            OrderLineList.Sum(orderLine => orderLine.Quantity * (orderLine.Product?.Price ?? 0));

        public Company? Company { get; set; }
        public Person? ContactPerson { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";

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