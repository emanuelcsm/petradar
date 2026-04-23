using Microsoft.AspNetCore.Http;

namespace PetRadar.API.Infrastructure.Validation.Media;

public interface IMediaUploadRequestValidator
{
    void Validate(IFormFile? file);
}
