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

    public async Task<IReadOnlyCollection<CourseDto>> GetAllCourses()
    {
        IReadOnlyCollection<Course> courses = await _courseRepository.GetAllCourses();        
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }

    public async Task<CourseDto?> GetCourseById(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course is null) return null;

        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<IReadOnlyCollection<CourseDto>> GetCourseByUserId(Guid id)
    {
        var courses = await _courseRepository.GetCourseFromUserId(id);        
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }
}
