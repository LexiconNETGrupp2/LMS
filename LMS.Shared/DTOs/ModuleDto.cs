using System.Text.Json.Serialization;

namespace LMS.Shared.DTOs;

public sealed record ModuleDto(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    ICollection<ActivityDto> Activities
);
