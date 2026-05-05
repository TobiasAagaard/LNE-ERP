namespace ErpCli.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public Product? Product { get; set;}
        public string? Name => Product?.Name;
        public double Quantity { get; set; }
    }
}