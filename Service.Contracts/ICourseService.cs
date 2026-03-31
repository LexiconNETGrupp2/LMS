using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Pagination;

namespace Service.Contracts;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token);
    Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token);
    Task<CourseDto> CreateCourse(CreateCourseDto createCourseDto, CancellationToken token);
    Task UpdateCourse(Guid id, UpdateCourseDto updateCourseDto, CancellationToken token);
    Task DeleteCourse(Guid id, CancellationToken token);
    Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid id, CancellationToken token);
    Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token);
}
