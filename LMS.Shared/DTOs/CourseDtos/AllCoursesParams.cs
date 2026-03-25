namespace LMS.Shared.DTOs.CourseDtos;

public record AllCoursesParams(
    string? Search,
    DateOnly? AfterDate,
    DateOnly? BeforeDate
);
