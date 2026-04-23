namespace Animals.Application.Integration.Interfaces;

public interface IKnownMediaRepository
{
    Task SaveAsync(
        string mediaId,
        string publicUrl,
        string? storagePath = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KnownMediaReadModel>> GetByIdsAsync(
        IReadOnlyCollection<string> mediaIds,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}
