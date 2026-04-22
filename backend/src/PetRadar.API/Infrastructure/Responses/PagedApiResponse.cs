namespace PetRadar.API.Infrastructure.Responses;

public sealed record PagedApiResponse<T>
{
    public IReadOnlyList<T> Data { get; init; }
    public ApiPagination Pagination { get; init; }
    public bool Success { get; init; }

    public PagedApiResponse(IReadOnlyList<T> data, ApiPagination pagination)
    {
        Data = data;
        Pagination = pagination;
        Success = true;
    }
}