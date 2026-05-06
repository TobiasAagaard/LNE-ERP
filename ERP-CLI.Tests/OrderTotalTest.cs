using ErpCli.Models;


namespace ERP_CLI.Tests;

public class OrderTotalTest
{
    [Theory]
    [InlineData(100, 2, 200)]   // 2 items at 100 each -> total 200
    [InlineData(50, 5, 250)]    // 5 items at 50 each -> total 250
    [InlineData(20, 0, 0)]      // 0 items at 20 each -> total 0
    
    public void OrderTotal_ReturnsExpectedTotal(double price, int quantity, double expected)
    {
        Product product = new()
        {
            Price = price,    
        };

        OrderLine orderLine = new()
        {
            Product = product,
            Quantity = quantity,
        };

        SalesOrderHeader header = new()
        {
            OrderLineList = new List<OrderLine> { orderLine }
        };

        var result = header.OrderTotal;

        Assert.Equal(expected, result);
    }
    
}