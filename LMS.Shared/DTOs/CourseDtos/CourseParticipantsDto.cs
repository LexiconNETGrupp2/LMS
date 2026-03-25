
namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseParticipantsDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ICollection<CourseParticipantDto> Students { get; init; } = [];
}
