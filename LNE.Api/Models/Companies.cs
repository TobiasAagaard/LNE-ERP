namespace LNE.Api.Models;

public class Companies
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Currencys Currency { get; set; }

    public int AddressId { get; set; }

    public Addresses Address { get; set; } = null!;
}