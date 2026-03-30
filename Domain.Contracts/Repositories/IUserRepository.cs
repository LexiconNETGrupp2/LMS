using Domain.Models.Entities;
using LMS.Shared;

namespace Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task<PagedResult<ApplicationUser>> GetAllWithCoursesAsync(PagedQuery query, CancellationToken ct);
    Task<ApplicationUser?> GetByIdWithCourseAsync(string id, CancellationToken ct);
}
