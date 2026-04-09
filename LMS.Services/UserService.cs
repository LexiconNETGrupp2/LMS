using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _uow;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork uow)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _uow = uow;
    }

    public async Task<PagedResult<UserDto>> GetAllUsers(AllUsersParams query, CancellationToken ct)
    {
        var users = await _uow.Users.GetAllWithCoursesAsync(query, ct);
        return new PagedResult<UserDto>
        {
            Page = users.Page,
            PageSize = users.PageSize,
            TotalItems = users.TotalItems,
            Items = users.Items.Select(MapToUserDto).ToList(),
        };
    }

    public async Task<UserDto> GetUserById(string id)
    {
        var user = await _uow.Users.GetByIdWithCourseAsync(id, CancellationToken.None)
            ?? throw new UserNotFoundException();
        return MapToUserDto(user);
    }

    public async Task<UserDto> CreateUser(UserRegistrationDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Role))
            throw new BadRequestException("Role is required");

        if (!(await _roleManager.RoleExistsAsync(request.Role)))
            throw new BadRequestException("Role does not exist");

        if (RolesNames.Student.Equals(request.Role) && request.CourseId is null)
            throw new BadRequestException("CourseId is required for students");

        // Since CourseId is optional for teachers, we need to check if it's provided and if it exists
        Course? course = null;
        if (request.CourseId is not null)
        {
            course = await _uow.Courses.GetCourseById(request.CourseId.Value, trackChanges: true, CancellationToken.None);
            if (course is null)
                throw new BadRequestException("Course not found");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            Course = course,
        };

        IdentityResult result;
        result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                throw new BadRequestException("E-postadressen är redan registrerad");

            throw new BadRequestException(string.Join(", ", result.Errors.Select(s => s.Description)));
        }

        result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(s => s.Description)));

        return MapToUserDto(user);
    }

    public async Task DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new UserNotFoundException();
        await _userManager.DeleteAsync(user);
    }

    private static UserDto MapToUserDto(ApplicationUser user)
        => MapToUserDto(user, null);

    private static UserDto MapToUserDto(ApplicationUser user, string? role)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = role,
            CourseId = user.Course?.Id,
        };
    }

    public async Task<UserDto> UpdateUser(string id, UpdateUserDto request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new NotFoundException("Användaren hittades inte");

        ValidateUpdateRequest(request);

        var normalizedEmail = request.Email.Trim();
        var duplicateUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (duplicateUser is not null && duplicateUser.Id != user.Id)
            throw new BadRequestException("E-postadressen är redan registrerad");

        user.Email = normalizedEmail;
        user.UserName = normalizedEmail;
        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new BadRequestException(GetIdentityErrors(updateResult));

        var updatedUser = await _uow.Users.GetByIdWithCourseAsync(user.Id, token)
            ?? throw new NotFoundException("Användaren hittades inte");

        return MapToUserDto(updatedUser);
    }

    private static void ValidateUpdateRequest(UpdateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException("Email krävs");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new BadRequestException("Förnamn krävs");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new BadRequestException("Efternamn krävs");
    }

    private static string GetIdentityErrors(IdentityResult result)
        => string.Join(", ", result.Errors.Select(e => e.Description));
}
