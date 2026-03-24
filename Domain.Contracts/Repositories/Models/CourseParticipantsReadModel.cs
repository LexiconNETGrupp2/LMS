namespace Domain.Contracts.Repositories.Models;

public sealed record CourseParticipantsReadModel
{
    public Guid CourseId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyCollection<CourseParticipantReadModel> Participants { get; init; } = [];
    public IReadOnlyCollection<CourseParticipantRoleReadModel> ParticipantRoles { get; init; } = [];
}
