using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.AuthDtos;
public record UserRegistrationDto
{
    [Required]
    public string Password { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    public Guid? CourseId { get; init; }

    //Optional if you want to add user to role when you register user
    //It's allready supported in this implementation but only for existing roles
    //UI have to be updated to support this
    public string? Role { get; init; } = string.Empty;
}