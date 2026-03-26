
namespace LMS.Shared.DTOs.ModuleDtos;

public record CreateModuleDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid CourseId
);

public record UpdateModuleDto(
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate
);
