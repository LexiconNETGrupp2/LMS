using LMS.Shared.Pagination;

namespace LMS.Shared.DataObjects;

public record SearchAndSortParam : PagedQuery
{
    public string? Search { get; init; }
    public string? OrderBy { get; init; }
    public bool? IsDescending { get; init; }
}
 
