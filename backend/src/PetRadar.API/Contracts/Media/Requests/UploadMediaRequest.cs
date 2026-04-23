using Microsoft.AspNetCore.Http;

namespace PetRadar.API.Contracts.Media.Requests;

public sealed record UploadMediaRequest(IFormFile File);
