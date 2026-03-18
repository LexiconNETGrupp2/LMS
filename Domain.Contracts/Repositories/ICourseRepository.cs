using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<IReadOnlyCollection<Course>> GetAllCourses();
    Task<Course?> GetCourseById(Guid id);
    Task<IReadOnlyCollection<Course>> GetCourseFromUserId(Guid userId);
}
