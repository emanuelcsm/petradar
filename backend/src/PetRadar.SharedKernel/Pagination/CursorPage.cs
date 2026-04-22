namespace PetRadar.SharedKernel.Pagination;

public sealed record CursorPage<T>(
    IReadOnlyList<T> Data,
    string? NextPageToken,
    bool HasNextPage);