using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Repositories.Models;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Constants;
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
        foreach (var module in course.Modules) {
            module.Course = course;
        }
        try {
            _uow.Courses.Create(course);
            await _uow.CompleteAsync(token);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning("Error when adding course {CourseId} to database: {ExMessage}", course.Id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }    

    public async Task<IReadOnlyCollection<CourseDto>> GetAllCourses(AllCoursesParams param, CancellationToken token)
    {
        IReadOnlyCollection<Course> courses = await _uow.Courses.GetAllCourses(param, token);
        var courseDtos = _mapper.Map<IReadOnlyCollection<CourseDto>>(courses);
        return courseDtos ?? [];
    }

    public async Task<CourseDto?> GetCourseById(Guid id, string? currentStudentId, CancellationToken token)
    {
        var course = await _uow.Courses.GetCourseById(id, token)
            ?? throw new CourseNotFoundException(id);
        
        var userIds = course.Students.Select(u => u.Id);
        if (currentStudentId is not null && !userIds.Contains(currentStudentId))
            throw new UserUnauthorizedException();

        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseDto?> GetCourseByUserId(Guid id, CancellationToken token)
    {
        var course = await _uow.Courses.GetCourseFromUserId(id, token)
            ?? throw new CourseNotFoundException(id);

        var courseDto = _mapper.Map<CourseDto>(course);
        return courseDto;
    }

    public async Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid id, CancellationToken token)
    {
        var courseParticipants = await _uow.Courses.GetCourseParticipantsByUserId(id, token)
            ?? throw new CourseNotFoundException(id);

        var roleByUserId = courseParticipants.ParticipantRoles
                        .GroupBy(role => role.UserId)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(role => role.RoleName)
                                          .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
                                          .OrderBy(GetRolePriority)
                                          .ThenBy(roleName => roleName)
                                          .FirstOrDefault() ?? string.Empty);

        return new CourseParticipantsDto
        {
            Name = courseParticipants.Name,
            Description = courseParticipants.Description,
            Students = courseParticipants.Participants
                        .Select(participant => new CourseParticipantDto
                        {
                            Id = participant.Id,
                            FullName = $"{participant.FirstName} {participant.LastName}".Trim(),
                            Email = participant.Email ?? string.Empty,
                            Role = roleByUserId.GetValueOrDefault(participant.Id, string.Empty)
                        })
                        .ToList()
        };
    }

    private static int GetRolePriority(string? roleName) =>
        roleName switch
        {
            RolesNames.Teacher => 0,
            RolesNames.Student => 1,
            _ => 2
        };

    public async Task<bool> UpdateCourse(Guid id, UpdateCourseDto updateCourseDto, CancellationToken token)
    {
        Course? course = await _uow.Courses.GetCourseById(id, token)
            ?? throw new CourseNotFoundException(id);
        
        if (updateCourseDto.Name is not null) {
            course.Name = updateCourseDto.Name;
        }
        if (updateCourseDto.Description is not null) {
            course.Description = updateCourseDto.Description;
        }
        if (updateCourseDto.StartDate is not null) {
            course.StartDate = (DateOnly)updateCourseDto.StartDate;
        }
        if (updateCourseDto.EndDate is not null) {
            course.EndDate = (DateOnly)updateCourseDto.EndDate;
        }

        try {
            _uow.Courses.Update(course);
            await _uow.CompleteAsync(token);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning("Error when updating course {CourseId}: {ExMessage}", id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<bool> DeleteCourse(Guid id, CancellationToken token)
    {
        Course? course = await _uow.Courses.GetCourseById(id, token)
            ?? throw new CourseNotFoundException(id);

        try {
            _uow.Courses.Delete(course);
            await _uow.CompleteAsync(token);
            return true;
        } catch (Exception ex) {
            _logger.LogWarning("Could not delete course {CourseId}: {ExMessage}", id, ex.Message);
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token)
    {
        return await _uow.Courses.GetStudentsByCourseId(courseId, token);
    }
}
