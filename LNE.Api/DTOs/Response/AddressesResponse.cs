namespace LNE.Api.Dtos.Response;

public record AddressesResponse (
    int Id,
    string Street,
    string Number,
    string PostalCode,
    string City,
    string Country
);