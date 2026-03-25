namespace LMS.Blazor.Client.Components.CourseParticipants;

public sealed class CourseParticipantsViewModel
{
    public string CourseTitle { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<ParticipantViewModel> Participants { get; init; } = [];
}

public sealed class ParticipantViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
