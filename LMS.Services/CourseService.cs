using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.Extensions.Logging;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(IUnitOfWork uow, IMapper mapper, ILogger<CourseService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> CreateCourse(CreateCourseDto createCourseDto, CancellationToken token)
    {
        Course course = _mapper.Map<Course>(createCourseDto);
        _uow.CourseRepository.Create(course);
        await _uow.CompleteAsync(token);
        return true;
    }

    public async Task<IReadOnlyCollection<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token)
    {
        IReadOnlyCollection<Course> courses = await _uow.CourseRepository.GetAllCourses(param, token);
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }

    public async Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token)
    {
        var course = await _uow.CourseRepository.GetCourseById(id, token);
        if (course is null) return null;
        
        var userIds = course.Students.Select(u => u.Id);
        if (currentStudentId is not null && !userIds.Contains(currentStudentId))
            return null;


        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token)
    {
        var course = await _uow.CourseRepository.GetCourseFromUserId(id, token);
        if (course is null) return null;
        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }
}
