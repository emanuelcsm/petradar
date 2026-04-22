namespace PetRadar.API.Contracts.Identity.Requests;

public sealed record RegisterRequest(
    string Email,
    string Name,
    string Password);
