using ErpCli.Models;

namespace ERP_CLI.Tests;

public class ProfitPercentTests
{
    [Theory]
    [InlineData(100, 50, 100)]   // Price double the cost -> 100% profit
    [InlineData(150, 150, 0)]    // Price equal to cost -> 0% profit
    [InlineData(80, 100, -20)]   // Price less than cost -> -20% (loss)
    public void ProfitPercent_ReturnsExpectedPercentage(decimal price, decimal cost, decimal expected)
    {
        Product product = new Product
        {
            Price = price,
            Cost = cost,
        };

        var result = product.ProfitPercent;

        Assert.Equal(expected, result);
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

    [Fact]
    public void ProfitPercent_WhenPriceIsNull_ReturnsNull()
    {
        Product product = new Product
        {
            Price = null,
            Cost = 50,
        };

        var result = product.ProfitPercent;

        Assert.Null(result);
    }

    [Fact]
    public void ProfitPercent_WhenCostIsNull_ReturnsNull()
    {
        Product product = new Product
        {
            Price = 100,
            Cost = null,
        };

        var result = product.ProfitPercent;

        Assert.Null(result);
    }
}
