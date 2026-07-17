using LNE.Api.Models;

namespace LNE.Api.Dtos.Response;

public record CompaniesResponse (
    int Id,
    string Name,
    Currencys Currency,
    AddressesResponse Address
);