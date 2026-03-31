using Domain.Contracts.Queries;
using LMS.Infractructure.Data;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Queries;

public class ParticipantQuery : IParticipantQuery
{
    private readonly ApplicationDbContext _context;

    public ParticipantQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid userId, CancellationToken token)
    {
        var userIdStr = userId.ToString();

        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Students.Any(student => student.Id == userIdStr))
            .Select(c => new
            {
                c.Name,
                c.Description,
                Participants = c.Students.Select(student => new
                {
                    student.Id,
                    student.FirstName,
                    student.LastName,
                    student.Email
                }).ToList()
            })
            .FirstOrDefaultAsync(token);

        if (course is null)
            return null;

        var participantIds = course.Participants
            .Select(participant => participant.Id)
            .ToList();

        var roleByUserId = participantIds.Count == 0
            ? new Dictionary<string, string>()
            : await (
                from userRole in _context.UserRoles.AsNoTracking()
                join role in _context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                where participantIds.Contains(userRole.UserId)
                group role by userRole.UserId into roleGroup
                select new
                {
                    UserId = roleGroup.Key,
                    RoleName = roleGroup
                        .Select(role => role.Name)
                        .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
                        .OrderBy(roleName =>
                            roleName == RolesNames.Teacher ? 0 :
                            roleName == RolesNames.Student ? 1 : 2)
                        .ThenBy(roleName => roleName)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToDictionaryAsync(entry => entry.UserId, entry => entry.RoleName, token);

        return new CourseParticipantsDto
        {
            Name = course.Name,
            Description = course.Description,
            Students = course.Participants
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

    public async Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token)
    {
        var students = await (
            from course in _context.Courses.AsNoTracking()
            where course.Id == courseId

            from student in course.Students

            join userRole in _context.UserRoles.AsNoTracking()
                on student.Id equals userRole.UserId

            join role in _context.Roles.AsNoTracking()
                on userRole.RoleId equals role.Id

            where role.Name == RolesNames.Student

            select new CourseStudentDto
            {
                Id = student.Id,
                FullName = (student.FirstName + " " + student.LastName).Trim(),
                Email = student.Email ?? string.Empty
            }
        ).ToListAsync(token);

        return students;
    }
}
