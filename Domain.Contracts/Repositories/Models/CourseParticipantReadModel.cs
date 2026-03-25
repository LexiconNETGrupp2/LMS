namespace Domain.Contracts.Repositories.Models;

public sealed record CourseParticipantReadModel
{
    public string Id { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
}
