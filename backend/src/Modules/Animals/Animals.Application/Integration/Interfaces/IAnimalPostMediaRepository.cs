namespace Animals.Application.Integration.Interfaces;

public interface IAnimalPostMediaRepository
{
    Task SaveAsync(
        string mediaId,
        string storagePath,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}
