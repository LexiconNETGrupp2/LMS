namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseDto
{
    public string Name {get; set; } = string.Empty;
    public string Description {get; set; } = string.Empty;
    public DateOnly StartDate {get; set; }
    public string StartDateStr => StartDate.ToString("yyyy-MM-dd");
    public DateOnly EndDate {get; set; }
    public string EndDateStr => EndDate.ToString("yyyy-MM-dd");
    public int NumberOfStudents {get; set; }
    public ICollection<CourseModuleDto> Modules { get; set; } = [];
}
