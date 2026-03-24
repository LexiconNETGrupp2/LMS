namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseModuleDto(
    Guid Id,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<ActivityDto> Activities
)
{
    public int ActivitiesCount => Activities?.Count ?? 0;
    public string StartDateStr => StartDate.ToString("yyyy-MM-dd");
    public string EndDateStr => EndDate.ToString("yyyy-MM-dd");
}
