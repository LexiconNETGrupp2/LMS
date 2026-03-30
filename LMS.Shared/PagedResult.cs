namespace LMS.Shared;

public record PagedResult<T>
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 20;
    public required int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public required IReadOnlyList<T> Items { get; init; } = [];
}
