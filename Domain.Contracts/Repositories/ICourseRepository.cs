using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<IReadOnlyCollection<Course>> GetAllCourses();
    Task<Course?> GetCourseFromUserId(Guid UserId);
}
