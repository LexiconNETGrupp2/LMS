namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    Guid CourseId,
    string CourseName
);
