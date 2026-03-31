using Domain.Contracts.Repositories.Models;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Pagination;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<PagedResult<Course>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<Course?> GetCourseById(Guid id, CancellationToken token);
    Task<Course?> GetCourseByIdTracked(Guid id, CancellationToken token);
    Task<Course?> GetCourseFromUserId(Guid userId, CancellationToken token);
    Task<CourseParticipantsReadModel?> GetCourseParticipantsByUserId(Guid userId, CancellationToken token);
    Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token);
}
