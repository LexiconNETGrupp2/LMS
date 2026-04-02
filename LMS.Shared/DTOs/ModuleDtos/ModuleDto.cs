namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleDto(
    Guid Id,
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid CourseId,
    string CourseName
);
