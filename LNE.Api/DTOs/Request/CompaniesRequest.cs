using System.ComponentModel.DataAnnotations;
using LNE.Api.Models;

namespace LNE.Api.Dtos.Request;

public record CreateCompaniesRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public Currencys Currency { get; init; }

    [Required]
    public AddressRequest Address { get; init; } = new();
}

public record UpdateCompaniesRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public Currencys Currency { get; init; }

    [Required]
    public AddressRequest Address { get; init; } = new();
}