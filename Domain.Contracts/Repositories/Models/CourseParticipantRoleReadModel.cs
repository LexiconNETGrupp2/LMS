namespace Domain.Contracts.Repositories.Models;

public sealed record CourseParticipantRoleReadModel
{
    public string UserId { get; init; } = string.Empty;
    public string? RoleName { get; init; }
}
