namespace PetRadar.API.Infrastructure.Responses;

public static class ApiResponseExtensions
{
    public static ApiResponse<T> ToResponse<T>(this T data) => new(data);
}
