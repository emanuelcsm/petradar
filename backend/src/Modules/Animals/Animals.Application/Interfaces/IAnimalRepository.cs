using Animals.Domain.Entities;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Interfaces;

public interface IAnimalRepository
{
    Task<AnimalPost?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task SaveAsync(AnimalPost animalPost, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnimalPost>> GetNearbyAsync(
        GeoLocation center,
        double radiusKm,
        CancellationToken cancellationToken = default);
}