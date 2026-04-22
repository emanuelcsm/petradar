namespace PetRadar.API.Infrastructure.Responses;

public sealed record ApiResponse<T>
{
    public T Data { get; init; }
    public bool Success { get; init; }

    public ApiResponse(T data)
    {
        Data = data;
        Success = true;
    }
}
