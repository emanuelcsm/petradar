namespace PetRadar.API.Infrastructure.Responses;

public sealed record ApiPagination(
    string? NextPageToken,
    bool HasNextPage);