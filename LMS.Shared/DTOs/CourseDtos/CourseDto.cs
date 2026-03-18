namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<UserDto> Students,
    ICollection<CourseModuleDto> Modules
);
