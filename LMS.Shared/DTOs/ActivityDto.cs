using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Shared.DTOs;

public sealed record ActivityDto(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    ActivityTypeDto Type,
    CourseModuleDto Module
);