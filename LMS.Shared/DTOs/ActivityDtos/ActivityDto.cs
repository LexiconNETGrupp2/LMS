using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Shared.DTOs.ActivityDtos;

public sealed record ActivityDto(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    ActivityTypeDto Type,
    Guid ModuleId
);
