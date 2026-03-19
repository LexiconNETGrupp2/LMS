namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseDto
{
    public string Name {get; set; } = string.Empty;
    public string Description {get; set; } = string.Empty;
    public DateOnly StartDate {get; set; }
    public DateOnly EndDate {get; set; }
    public int NumberOfStudents {get; set; }
    public ICollection<CourseModuleDto> Modules { get; set; } = [];
}
