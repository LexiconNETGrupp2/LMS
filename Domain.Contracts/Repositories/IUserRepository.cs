using Domain.Models.Entities;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;

namespace Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task<PagedResult<ApplicationUser>> GetAllWithCoursesAsync(AllUsersParams query, CancellationToken ct);
    Task<ApplicationUser?> GetByIdWithCourseAsync(string id, CancellationToken ct);
}
