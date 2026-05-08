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
        public double? OrderTotal =>
            OrderLineList.Sum(orderLine => orderLine.Product?.Price * orderLine.Quantity ?? 0);
        public Customer? customer { get; set; }
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