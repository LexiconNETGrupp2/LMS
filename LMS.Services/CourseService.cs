using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs;
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
        var courses = await _courseRepository.GetAllCourses();
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }

    public async Task<CourseDto?> GetCourseById(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseDto?> GetCourseByUserId(Guid id)
    {
        var course = await _courseRepository.GetCourseFromUserId(id);
        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }
}
