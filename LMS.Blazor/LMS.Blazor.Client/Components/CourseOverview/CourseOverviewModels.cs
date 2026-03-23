namespace LMS.Blazor.Client.Components.CourseOverview;

public sealed class CourseOverviewViewModel
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DateRange { get; init; } = string.Empty;
    public int ProgressPercent { get; init; }
    public IReadOnlyList<ModuleViewModel> Modules { get; init; } = [];
}

public sealed class ModuleViewModel
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Title { get; init; } = string.Empty;
    public string DateRange { get; init; } = string.Empty;
    public int ProgressPercent { get; init; }
    public bool IsExpanded { get; set; }
    public string? Description { get; init; }
    public IReadOnlyList<ActivityViewModel> Activities { get; init; } = [];
}

public sealed class ActivityViewModel
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Date { get; init; } = string.Empty;
    public bool HasDocument { get; init; }
    public string StatusText { get; init; } = string.Empty;
    public string? StatusIcon { get; init; }
    public string StatusTextClass { get; init; } = "text-success";
}
