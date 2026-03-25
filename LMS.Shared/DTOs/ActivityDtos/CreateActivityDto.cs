using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Shared.DTOs.ActivityDtos;

public record CreateActivityDto(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    ActivityTypeDto Type,
    Guid ModuleId
);
