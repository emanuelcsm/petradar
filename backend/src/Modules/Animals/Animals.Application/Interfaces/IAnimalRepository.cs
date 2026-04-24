using Animals.Domain.Entities;
using PetRadar.SharedKernel.Pagination;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Interfaces;

public interface IAnimalRepository
{
    Task<AnimalPost?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task SaveAsync(AnimalPost animalPost, CancellationToken cancellationToken = default);
    Task<CursorSlice<AnimalPost>> GetNearbyAsync(
        GeoLocation center,
        double radiusKm,
        DateTime? createdBeforeUtc,
        string? idBefore,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}