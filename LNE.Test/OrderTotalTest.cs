using ErpCli.Models;


namespace ERP_CLI.Tests;

public class OrderTotalTest
{
    [Theory]
    [InlineData(100, 2, 200)]   // 2 items at 100 each -> total 200
    [InlineData(50, 5, 250)]    // 5 items at 50 each -> total 250
    [InlineData(20, 0, 0)]      // 0 items at 20 each -> total 0
    
    public void OrderTotal_ReturnsExpectedTotal(decimal price, decimal quantity, decimal expected)
    {
       
        SalesOrderHeader header = new()
        {
            OrderLineList = new List<OrderLine>
            {
                new OrderLine { Product = new Product { Price = price }, Quantity = quantity }
            }
        };

        var result = header.OrderTotal;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void OrderTotal_SumsAllOrderLines()
    {
        SalesOrderHeader header = new()
        {
            OrderLineList = new List<OrderLine>
            {
                new OrderLine { Product = new Product { Price = 100 }, Quantity = 2 }, 
                new OrderLine { Product = new Product { Price = 50 }, Quantity = 5 },  
                new OrderLine { Product = new Product { Price = 20 }, Quantity = 0 },  
            }
        };

        var result = header.OrderTotal;

        Assert.Equal(450, result);
    }

}