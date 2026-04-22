namespace PetRadar.API.Infrastructure.Options;

public sealed class CursorPaginationOptions
{
    public const string SectionName = "Pagination:Cursor";

    public int DefaultPageSize { get; init; }
    public int MaxPageSize { get; init; }
}