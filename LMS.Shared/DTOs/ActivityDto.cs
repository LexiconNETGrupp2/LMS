namespace LMS.Shared.DTOs;

public sealed record ActivityDto(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    ActivityTypeDto Type,
    ModuleDto Module
);