namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CreateCourseDto(
    string Title,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate
);
