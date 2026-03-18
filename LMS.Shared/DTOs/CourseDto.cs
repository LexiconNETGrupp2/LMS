namespace LMS.Shared.DTOs;

public sealed record CourseDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<UserDto> Students,
    ICollection<ModuleDto> Modules
);
