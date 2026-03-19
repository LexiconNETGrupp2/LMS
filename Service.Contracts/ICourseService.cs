using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IReadOnlyCollection<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token);
    Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token);
}
