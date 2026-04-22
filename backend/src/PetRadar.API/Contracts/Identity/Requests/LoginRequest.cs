namespace PetRadar.API.Contracts.Identity.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);
