namespace LMS.Shared.DTOs.ModuleDtos;

public record CreateModuleDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    Guid CourseId
);

public record UpdateModuleDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate
);
