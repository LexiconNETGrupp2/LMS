using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public CourseService(IUnitOfWork uow, ICourseRepository courseRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _courseRepository = courseRepository;
        _mapper = mapper;
        _userManager = userManager;
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

    public async Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid id, CancellationToken token)
    {
        var courseParticipants = await _courseRepository.GetCourseWithStudentsFromUserId(id, token);
        if (courseParticipants is null) return null;

        var participants = new List<CourseParticipantDto>();

        foreach(var user in courseParticipants.Students)
        {
            var roles = await _userManager.GetRolesAsync(user);
            participants.Add(new CourseParticipantDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty

            });
        }

        var courseParticipantsDto = new CourseParticipantsDto
        {
            Name = courseParticipants.Name,
            Description = courseParticipants.Description,
            Students = participants
        };

        return courseParticipantsDto;
    }
}
