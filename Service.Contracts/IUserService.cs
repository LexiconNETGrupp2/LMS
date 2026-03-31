using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;

namespace Service.Contracts;
public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsers(PagedQuery query, CancellationToken ct = default);
    Task<UserDto> GetUserById(string id);
    Task DeleteUser(string id);
}
