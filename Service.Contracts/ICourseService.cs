using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IReadOnlyCollection<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token);
    Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token);
    Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token);
    Task<bool> CreateCourse(CreateCourseDto createCourseDto, CancellationToken token);
    Task<bool> UpdateCourse(Guid id, UpdateCourseDto updateCourseDto, CancellationToken token);
}
