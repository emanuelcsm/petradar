namespace PetRadar.SharedKernel.Pagination;

public sealed record CursorSlice<T>(
    IReadOnlyList<T> Items,
    bool HasNextPage);