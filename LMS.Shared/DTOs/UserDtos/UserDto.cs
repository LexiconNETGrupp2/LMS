namespace LMS.Shared.DTOs.UserDtos;

public sealed record UserDto
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public Guid? CourseId { get; init; }
}
