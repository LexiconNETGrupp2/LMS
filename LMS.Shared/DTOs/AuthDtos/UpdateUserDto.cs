using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.AuthDtos;

public record UpdateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;
}
