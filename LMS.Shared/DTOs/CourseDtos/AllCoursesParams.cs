using LMS.Shared.DataObjects;

namespace LMS.Shared.DTOs.CourseDtos;

public record AllCoursesParams(
    DateOnly? AfterDate,
    DateOnly? BeforeDate
) : SearchAndSortParam;
