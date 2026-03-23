using System.Reflection;

namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CreateCourseDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<Module> Modules
);
