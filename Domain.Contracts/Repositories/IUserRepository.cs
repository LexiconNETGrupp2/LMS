using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<ApplicationUser>> GetAllWithCoursesAsync(CancellationToken ct);
    Task<ApplicationUser?> GetByIdWithCourseAsync(string id, CancellationToken ct);
}
