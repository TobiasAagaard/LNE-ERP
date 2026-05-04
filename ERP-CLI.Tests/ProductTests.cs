using ErpCli.Models;

namespace ERP_CLI.Tests;

public class ProductTests
{
    [Fact]
    public void ProfitPercent_WhenPriceIsDoubleCost_Returns100()
    {
        Product product = new Product
        {
            Price = 100,
            Cost = 50,
        };

        var result = product.ProfitPercent;

        Assert.Equal(100, result);
    }

    [Fact]

    public void ProfitPercent_WhenPriceIsEqualToCost_Returns0()
    {
        Product product = new Product
        {
            Price = 150,
            Cost = 150,
        };

        var result = product.ProfitPercent;

        Assert.Equal(0, result);    
    }

    [Fact]
    public void ProfitPercent_WhenPriceIsLessThanCost_ReturnsNegative()
    {
        Product product = new Product
        {
            Price = 80,
            Cost = 100,
        };

        var result = product.ProfitPercent;

        Assert.Equal(-20, result);
    }

    [Fact]
    public void ProfitPercent_WhenDivisionByZero_ReturnsNull()
    {
        Product product = new Product
        {
            Price = 100,
            Cost = 0,
        };

        var result = product.ProfitPercent;

        Assert.Null(result);
    }
}
