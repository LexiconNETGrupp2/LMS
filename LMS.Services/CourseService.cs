using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(IUnitOfWork uow, ICourseRepository courseRepository, IMapper mapper)
    {
        _uow = uow;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token)
    {
        IReadOnlyCollection<Course> courses = await _courseRepository.GetAllCourses(param, token);
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }

    public async Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token)
    {
        var course = await _courseRepository.GetCourseById(id, token);
        if (course is null) return null;
        
        var userIds = course.Students.Select(u => u.Id);
        if (currentStudentId is not null && !userIds.Contains(currentStudentId))
            return null;


        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token)
    {
        var course = await _courseRepository.GetCourseFromUserId(id, token);
        if (course is null) return null;
        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }
}
