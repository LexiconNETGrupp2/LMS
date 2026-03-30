using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uow;
    public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork uow)
    {
        _userManager = userManager;
        _uow = uow;
    }

    public async Task<IReadOnlyCollection<UserDto>> GetAllUsers(CancellationToken ct)
    {
        var users = await _uow.Users.GetAllWithCoursesAsync(ct);
        return users.Select(MapToUserDto).ToList();
    }

    public async Task<UserDto> GetUserById(string id)
    {
        var user = await _uow.Users.GetByIdWithCourseAsync(id, CancellationToken.None);
        if (user == null)
            throw new UserNotFoundException();
        return MapToUserDto(user);
    }

    public async Task DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new UserNotFoundException();
        await _userManager.DeleteAsync(user);
    }

    private static UserDto MapToUserDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CourseId = user.Course?.Id,
        };
    }
}
