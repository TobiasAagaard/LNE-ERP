using System.ComponentModel.DataAnnotations;

namespace LNE.Api.Dtos;

public record AddressRequest
{
    [Required, StringLength(100)]
    public string Street { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string Number { get; init; } = string.Empty;

    [Required, StringLength(20)]
    public string PostalCode { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string City { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string Country { get; init; } = string.Empty;
}