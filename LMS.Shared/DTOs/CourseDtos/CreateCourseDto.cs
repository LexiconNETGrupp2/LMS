using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CreateCourseDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<CreateModuleDto> Modules
);
