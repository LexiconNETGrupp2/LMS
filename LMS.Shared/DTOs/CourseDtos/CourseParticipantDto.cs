
namespace LMS.Shared.DTOs.CourseDtos;

public sealed record CourseParticipantDto
{
    public string Id { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role {  get; init; } = string.Empty;
}
