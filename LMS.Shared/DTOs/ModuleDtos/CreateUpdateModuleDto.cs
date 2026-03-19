
namespace LMS.Shared.DTOs.ModuleDtos;

public record CreateModuleDto(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid CourseId
);

public record UpdateModuleDto(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate
);
