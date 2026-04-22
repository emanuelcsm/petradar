namespace PetRadar.API.Contracts.Identity.Responses;

public sealed record LoginResponse(
    string Token,
    string UserId,
    string Email,
    string Name);
