using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.Pagination;

public record PagedQuery
{
    public const int MinPage = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;

    [Range(MinPage, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public required int Page { get; init; } = 1;
    [Range(MinPageSize, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100.")]
    public required int PageSize { get; init; } = 20;
}
