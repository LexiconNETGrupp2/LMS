using AutoMapper;
using Domain.Contracts.Queries;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Pagination;
using Microsoft.Extensions.Logging;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IParticipantQuery _participantQuery;
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        IUnitOfWork uow,
        IParticipantQuery participantQuery,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        _uow = uow;
        _participantQuery = participantQuery;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CourseDto> CreateCourse(CreateCourseDto createCourseDto, CancellationToken token)
    {
        Course course = _mapper.Map<Course>(createCourseDto);
        foreach (var module in course.Modules) {
            module.Course = course;
        }
        try {
            _uow.Courses.Create(course);
            await _uow.CompleteAsync(token);
            return _mapper.Map<CourseDto>(course);
        } catch (Exception ex) {
            _logger.LogWarning("Error when adding course {CourseId} to database: {ExMessage}", course.Id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }    

    public async Task<PagedResult<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token)
    {
        var result = await _uow.Courses.GetAllCourses(param, token);
        return new PagedResult<CourseDto>
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            Items = _mapper.Map<List<CourseDto>>(result.Items),
        };
    }

    public async Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token)
    {
        var course = await _uow.Courses.GetCourseById(id, trackChanges: false, token)
            ?? throw new CourseNotFoundException(id);
        
        var userIds = course.Students.Select(u => u.Id);
        if (currentStudentId is not null && !userIds.Contains(currentStudentId))
            throw new UserUnauthorizedException();

        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token)
    {
        var course = await _uow.Courses.GetCourseFromUserId(id, trackChanges: false, token)
            ?? throw new CourseNotFoundException(id);

        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid id, CancellationToken token)
    {
        return await _participantQuery.GetCourseParticipantsByUserId(id, token);
    }

    public async Task UpdateCourse(Guid id, UpdateCourseDto updateCourseDto, CancellationToken token)
    {
        Course? course = await _uow.Courses.GetCourseById(id, trackChanges: false, token, includeAllData: false)
           ?? throw new CourseNotFoundException(id);

        if (updateCourseDto.Name is not null)
        {
            course.Name = updateCourseDto.Name;
        }
        if (updateCourseDto.Description is not null)
        {
            course.Description = updateCourseDto.Description;
        }
        if (updateCourseDto.StartDate is not null)
        {
            if (course.Modules.Count > 0) {
                Module module = course.Modules.OrderBy(m => m.StartDate).First();
                DateOnly earliestDate = module.StartDate;
                if (updateCourseDto.StartDate > earliestDate)
                    throw new BadRequestException($"Den här kursen kan börja senast {earliestDate:yyyy-MM-dd} så att modulen {module.Name} inte startar innan kursen.");
            }
            course.StartDate = (DateOnly)updateCourseDto.StartDate;
        }
        if (updateCourseDto.EndDate is not null)
        {
            if (course.Modules.Count > 0) {
                Module module = course.Modules.OrderBy(m => m.EndDate).Last();
                DateOnly latestDate = module.EndDate;
                if (updateCourseDto.EndDate < latestDate)
                    throw new BadRequestException($"Den här kursen kan sluta tidigast {latestDate:yyyy-MM-dd} så att modulen {module.Name} inte slutar efter kursen.");
            }
            course.EndDate = (DateOnly)updateCourseDto.EndDate;
        }

        try
        {
            _uow.Courses.Update(course);
            await _uow.CompleteAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error when updating course {CourseId}: {ExMessage}", id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task DeleteCourse(Guid id, CancellationToken token)
    {
        Course? course = await _uow.Courses.GetCourseById(id, trackChanges: false, token)
            ?? throw new CourseNotFoundException(id);

        try
        {
            _uow.Courses.Delete(course);
            await _uow.CompleteAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Could not delete course {CourseId}: {ExMessage}", id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token)
    {
        return await _participantQuery.GetStudentsByCourseId(courseId, token);
    }
}
