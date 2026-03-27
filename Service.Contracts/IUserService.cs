using LMS.Shared.DTOs.UserDtos;

namespace Service.Contracts;
public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetAllUsers(CancellationToken ct = default);
    Task<UserDto?> GetUserById(string id);
    Task DeleteUser(string id, CancellationToken ct = default);
}
