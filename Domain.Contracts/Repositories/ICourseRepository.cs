using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Pagination;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<PagedResult<Course>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<Course?> GetCourseById(Guid id, bool trackChanges, CancellationToken token);
    Task<Course?> GetCourseFromUserId(Guid userId, bool trackChanges, CancellationToken token);
}
