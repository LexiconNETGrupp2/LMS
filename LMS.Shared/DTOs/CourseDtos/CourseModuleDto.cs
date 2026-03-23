namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseModuleDto(
    Guid Id,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<ActivityDto> Activities
);
