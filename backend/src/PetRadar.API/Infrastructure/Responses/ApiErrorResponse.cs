namespace PetRadar.API.Infrastructure.Responses;

public sealed record ApiErrorResponse
{
    public ApiError Error { get; init; }
    public bool Success { get; init; }

    public ApiErrorResponse(ApiError error)
    {
        Error = error;
        Success = false;
    }
}
