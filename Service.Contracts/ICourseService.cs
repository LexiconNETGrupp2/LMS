using LMS.Shared.DTOs;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IReadOnlyCollection<CourseDto>> GetAllCourses();
    Task<CourseDto?> GetCourseById(Guid id);
    Task<CourseDto?> GetCourseByUserId(Guid id);
}
