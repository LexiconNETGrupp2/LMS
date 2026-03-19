namespace LMS.Shared.DTOs.CourseDtos;

public record AllCoursesParams(
    DateOnly? AfterDate,
    DateOnly? BeforeDate
);
