namespace ErpCli.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public decimal? ProfitPercent => (Price.HasValue && Cost.HasValue && Cost.Value != 0) ? ((Price.Value - Cost.Value) / Cost.Value) * 100 : (decimal?)null;
        public string? Location { get; set; }
        public decimal StockQuantity { get; set; }
        public Unit Unit { get; set; }
    }
    public enum Unit
    {
        Styk, Timer, Meter
    }
}