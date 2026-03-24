using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CreateCourseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ICollection<CreateModuleDto> Modules { get; set; } = [];
}
