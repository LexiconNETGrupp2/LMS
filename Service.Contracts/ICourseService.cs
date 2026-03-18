using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IReadOnlyCollection<CourseDto>> GetAllCourses();
    Task<CourseDto?> GetCourseById(Guid id);
    Task<IReadOnlyCollection<CourseDto>> GetCourseByUserId(Guid id);
}
