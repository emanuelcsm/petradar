namespace PetRadar.API.Infrastructure.Responses;

public static class ApiResponseExtensions
{
    public static ApiResponse<T> ToResponse<T>(this T data) => new(data);

    public static PagedApiResponse<T> ToPagedResponse<T>(
        this IReadOnlyList<T> data,
        string? nextPageToken,
        bool hasNextPage)
        => new(data, new ApiPagination(nextPageToken, hasNextPage));
}
