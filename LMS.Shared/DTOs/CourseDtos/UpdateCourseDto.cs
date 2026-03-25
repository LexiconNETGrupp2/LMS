namespace LMS.Shared.DTOs.CourseDtos;

public sealed record UpdateCourseDto(
    string? Name,
    string? Description,
    DateOnly? StartDate,
    DateOnly? EndDate
);
