using Domain.Contracts.Repositories.Models;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<IReadOnlyCollection<Course>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<Course?> GetCourseById(Guid id, CancellationToken token);
    Task<Course?> GetCourseFromUserId(Guid userId, CancellationToken token);
    Task<CourseParticipantsReadModel?> GetCourseParticipantsByUserId(Guid userId, CancellationToken token);
}
